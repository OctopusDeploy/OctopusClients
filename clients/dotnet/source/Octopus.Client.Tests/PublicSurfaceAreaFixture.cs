﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Assent;
using Assent.Namers;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NUnit.Framework;
using Octopus.Client.Tests.Extensions;

namespace Octopus.Client.Tests
{
    [TestFixture]
    public class PublicSurfaceAreaFixture
    {
        [Test]
        public void ThePublicSurfaceAreaShouldNotRegress()
        {
            var lines = typeof(OctopusRequest).GetTypeInfo().Assembly
                .ExportedTypes
                .Select(t => t.GetTypeInfo())
                .GroupBy(t => t.Namespace)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .SelectMany(g => FormatNamespace(g.Key, g))
                .ToArray();

            var framework = FormatDotNetVersion(RuntimeInformation.FrameworkDescription);
            try
            {
                var received = string.Join("\r\n", lines);
                this.Assent(
                    received,
                    new Configuration().UsingNamer(new PostfixNamer(framework))
                );
            }
            catch (AssentFailedException e)
            {
                using (var teamCityArtifactsWriter = new TeamCityServiceMessages().CreateWriter())
                {
                    teamCityArtifactsWriter.PublishArtifact(e.ReceivedFileName);
                    teamCityArtifactsWriter.PublishArtifact(e.ApprovedFileName);
                }
                throw;
            }
        }

        string FormatDotNetVersion(string frameworkDescription)
        {
            if (Regex.IsMatch(frameworkDescription, @"\.NET \d{1,2}\.\d{1,3}\.\d{1,3}"))
                return ".NETCore";
            else
                return string.Concat(RuntimeInformation.FrameworkDescription.Split(' ').Take(2));
        }

        IEnumerable<object> FormatNamespace(string name, IEnumerable<TypeInfo> types)
        {
            return name.InArray()
                .Concat("{".InArray())
                .Concat(types.OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase).SelectMany(FormatType).Select(l => "  " + l))
                .Concat("}".InArray());
        }

        IEnumerable<string> FormatType(TypeInfo type)
        {
            if (type.IsEnum)
            {
                return $"{type.Name}".InArray()
                        .Concat("{")
                        .Concat(Enum.GetValues(type.AsType()).Cast<Enum>().Select(v => $"    {v} = {(int)(object)v}"))
                        .Concat("}");
            }

            var kind = type.IsInterface
                ? "interface"
                : type.IsAbstract
                    ? "abstract class"
                    : "class";

            var interfaces = type.GetInterfaces();
            if (type.BaseType != null && type.BaseType.Name != typeof(object).Name) 
                interfaces = interfaces.Concat(new []{ type.BaseType }).ToArray();
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var fields = members.OfType<FieldInfo>().ToArray();
            var ctors = members.OfType<ConstructorInfo>().ToArray();
            var properties = members.OfType<PropertyInfo>().ToArray();
            var events = members.OfType<EventInfo>().ToArray();
            var methods = members.OfType<MethodInfo>().ToArray();
            var types = members.OfType<TypeInfo>().ToArray();
            var other = members.Except(methods).Except(properties).Except(fields).Except(ctors).Except(events).Except(types).ToArray();

            var body = fields.Select(f => $"{Static(f.IsStatic)}{f.FieldType} {f.Name}")
                .Concat(events.Select(e => $"event {FormatTypeName(e.EventHandlerType)} {e.Name}"))
                .Concat(ctors.SelectMany(FormatCtor))
                .Concat(properties.Select(p => $"{FormatProperty(p)}"))
                .Concat(methods.SelectMany(FormatMethods))
                .Concat(other.Select(o => $"UNKNOWN {o.GetType().Name} {o.Name}"))
                .Concat(types.SelectMany(FormatType));

            return
                $"{kind} {type.Name}".InArray()
                    .Concat(interfaces.Select(i => $"  {FormatTypeName(i)}"))
                    .Concat("{")
                    .Concat(body.Select(l => "  " + l))
                    .Concat("}");
        }

        string Static(bool isStatic) => isStatic ? "static " : "";

        string FormatProperty(PropertyInfo p)
        {
            var accessors = new List<string>();
            if (p.GetMethod?.IsPublic == true)
                accessors.Add("get;");
            if (p.SetMethod?.IsPublic == true)
                accessors.Add("set;");

            var isStatic = p.GetMethod?.IsStatic ?? p.SetMethod?.IsStatic ?? false;

            return $"{Static(isStatic)}{FormatTypeName(p.PropertyType)} {p.Name} {{ {string.Join(" ", accessors)} }}";
        }

        IEnumerable<string> FormatCtor(ConstructorInfo c)
        {
            if (c.IsStatic)
                return new string[0];

            var parameters = c.GetParameters().Select(p => FormatTypeName(p.ParameterType));
            return $"{c.Name}({parameters.CommaSeperate()})".InArray();
        }

        IEnumerable<string> FormatMethods(MethodInfo m)
        {
            if (m.IsSpecialName)
                return new string[0];

            var properties = m.GetParameters().Select(p => $"{FormatTypeName(p.ParameterType)}").ToArray();

            return $"{Static(m.IsStatic)}{FormatTypeName(m.ReturnType)} {m.Name}({properties.CommaSeperate()})".InArray();
        }

        string FormatTypeName(Type type, bool shortName = false)
        {
            if (type == typeof(void))
                return "void";

            var name = type.Name;

            if (!shortName && type.Namespace.StartsWith("Octopus"))
                name = type.Namespace + "." + name;

            if (!type.GetTypeInfo().IsGenericType)
                return name;

            name = name.Substring(0, name.IndexOf('`'));
            var args = type.GetGenericArguments().Select(a => FormatTypeName(a, true));
            return $"{name}<{args.CommaSeperate()}>";
        }
    }
}