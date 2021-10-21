using System;
using System.Collections.Generic;

namespace Lillisp.Core
{
    public class Scope
    {
        public Scope()
        {
            InteropNamespaces = new HashSet<string>(Interop.DefaultNamespaces);
        }

        public Scope(Scope parent)
        {
            Parent = parent;
            // TODO: implement copy on write or parent traversal for namespaces
            InteropNamespaces = new HashSet<string>(parent.InteropNamespaces);
        }

        public Scope? Parent { get; }

        public ISet<string> InteropNamespaces { get; }

        public Procedure? ExceptionHandler { get; set; }

        public IDictionary<string, object?> Env { get; } = new Dictionary<string, object?>();

        public object? this[string key] => Resolve(key);

        public object? Resolve(string key)
        {
            var scope = this;

            while (scope != null)
            {
                if (scope.Env.TryGetValue(key, out var value))
                    return value;

                scope = scope.Parent;
            }

            return null;
        }

        public void AddAllFrom<TValue>(IReadOnlyDictionary<string, TValue> dict)
        {
            foreach (var (key, value) in dict)
            {
                Env[key] = value;
            }
        }

        public void Define(string key, object? value)
        {
            if (Env.ContainsKey(key))
            {
                throw new ArgumentException($"Variable {key} has already been defined");
            }

            Env[key] = value;
        }

        public void Set(string key, object? value)
        {
            var scope = this;

            while (true)
            {
                if (scope.Env.ContainsKey(key))
                {
                    if (scope.Parent == null)
                    {
                        throw new InvalidOperationException("Global scope variables are immutable");
                    }

                    scope.Env[key] = value;
                    break;
                }

                scope = scope.Parent ?? throw new ArgumentException($"{key} has not been defined");
            }
        }

        public Scope CreateChildScope() => new(this);
    }
}
