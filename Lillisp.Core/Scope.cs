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

        public IDictionary<string, object?> Env { get; } = new Dictionary<string, object?>();

        public object? this[string key]
        {
            get => Resolve(key);
            set => Env[key] = value;
        }

        public object? Resolve(string key)
        {
            return Env.TryGetValue(key, out var value) ? value : Parent?.Resolve(key);
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
            if (!Env.ContainsKey(key))
            {
                if (Parent == null)
                    throw new ArgumentException($"Variable {key} has not yet been defined");

                Parent.Set(key, value);
            }

            Env[key] = value;
        }

        public Scope CreateChildScope() => new(this);
    }
}
