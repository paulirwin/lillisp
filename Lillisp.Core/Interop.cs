using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lillisp.Core
{
    public static class Interop
    {
        public static readonly string[] DefaultNamespaces = { "System" };

        public static object? ResolveSymbol(Scope scope, string symbol)
        {
            string? staticMember = null;

            if (symbol.Contains('/'))
            {
                var parts = symbol.Split('/');
                symbol = parts[0];
                staticMember = parts[1];
            }

            var type = FindType(scope, symbol);

            if (type == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(staticMember))
            {
                var memberInfo = type.GetMember(staticMember, BindingFlags.Public | BindingFlags.Static);

                if (memberInfo.Length == 1)
                {
                    if (memberInfo[0] is FieldInfo field)
                    {
                        return field.GetValue(null);
                    }
                    
                    if (memberInfo[0] is PropertyInfo { CanWrite: false } roProp)
                    {
                        return roProp.GetValue(null);
                    }

                    return memberInfo[0];
                }
                else if (memberInfo.Length > 1)
                {
                    // HACK: right now we don't support overloading
                    throw new AmbiguousMatchException($"More than one static member was found on .NET type {type} matching symbol {staticMember}.");
                }
            }

            return type;
        }

        private static Type? FindType(Scope scope, string name)
        {
            var matches = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assyType = assembly.GetType(name);

                if (assyType != null)
                {
                    matches.Add(assyType);
                }

                var assyMatches = scope.InteropNamespaces
                    .Select(i => $"{i}.{name}")
                    .Select(i => assembly.GetType(i))
                    .Where(i => i != null)
                    .ToList();

                matches.AddRange(assyMatches!);
            }
            
            if (matches.Count >= 1)
            {
                return matches[0];
            }
            
            //if (matches.Count > 1)
            //{
            //    throw new AmbiguousMatchException($"More than one .NET type matched the symbol: {string.Join(", ", matches)}");
            //}

            return null;
        }

    }
}