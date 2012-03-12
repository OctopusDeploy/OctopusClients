using System;
using OctopusTools.Infrastructure;

// ReSharper disable CheckNamespace
public static class TestCommandExtensions
// ReSharper restore CheckNamespace
{
    public static void Execute(this ICommand command, params string[] args)
    {
        command.Options.Parse(args);

        command.Execute();
    } 
}