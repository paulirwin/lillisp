using System;
using System.Collections.Generic;
using System.Text;

namespace Lillisp.Core.Expressions;

public static class ProcessContextExpressions
{
    public static object? GetEnvironmentVariable(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("get-environment-variable requires one argument");
        }

        return args[0] switch
        {
            string s => Environment.GetEnvironmentVariable(s),
            StringBuilder sb => Environment.GetEnvironmentVariable(sb.ToString()),
            _ => throw new ArgumentException("name must be a string")
        };
    }

    public static object? EmergencyExit(object?[] args)
    {
        if (args.Length == 0)
        {
            Environment.Exit(0);
            return null;
        }

        if (args[0] is bool b)
        {
            if (b)
            {
                Environment.Exit(0);
                return null;
            }

            Environment.Exit(-1);
            return null;
        }

        try
        {
            int code = Convert.ToInt32(args[0]);
            Environment.Exit(code);
        }
        catch
        {
            Environment.Exit(-1);
        }

        return null;
    }

    public static object? GetEnvironmentVariables(object?[] args)
    {
        var list = new List<Pair>();

        var enumerator = Environment.GetEnvironmentVariables().GetEnumerator();

        while (enumerator.MoveNext())
        {
            list.Add(new Pair(enumerator.Key, enumerator.Value));
        }

        return List.FromNodes(list);
    }

    public static object? CommandLine(object?[] args)
    {
        return List.FromNodes(Environment.GetCommandLineArgs());
    }
}