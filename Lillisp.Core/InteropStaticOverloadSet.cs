using System;
using System.Reflection;

namespace Lillisp.Core
{
    public class InteropStaticOverloadSet
    {
        public InteropStaticOverloadSet(Type declaringType, string methodName, MemberInfo[] overloads)
        {
            DeclaringType = declaringType;
            MethodName = methodName;
            Overloads = overloads;
        }

        public Type DeclaringType { get; }

        public string MethodName { get; }

        public MemberInfo[] Overloads { get; }

        public object? Invoke(object?[] args)
        {
            return DeclaringType.InvokeMember(MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
        }
    }
}
