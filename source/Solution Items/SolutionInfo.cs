using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyDescription("Tools for Octopus, an opinionated deployment solution for .NET applications")]
[assembly: AssemblyCompany("Octopus Deploy")]
[assembly: AssemblyProduct("Octopus, an opinionated deployment solution for .NET applications")]
[assembly: AssemblyCopyright("Copyright © Octopus Deploy 2011")]
[assembly: AssemblyCulture("")]
#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]