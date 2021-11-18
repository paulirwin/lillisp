﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lillisp.Core.Macros
{
    public static class EmitMacros
    {
        public static object? DefineRecord(LillispRuntime runtime, Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("defrecord requires at least one argument");
            }

            if (args[0] is not Symbol recordName)
            {
                throw new ArgumentException("defrecord's first argument must be the record name");
            }

            var assy = scope.AssemblyBuilder ?? AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Lillisp.User"), AssemblyBuilderAccess.RunAndCollect);

            var module = assy.DefineDynamicModule(Guid.NewGuid().ToString("N"));

            var type = module.DefineType(recordName.Value, TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass);

            //var equatable = typeof(IEquatable<>).MakeGenericType(type);
            //type.AddInterfaceImplementation(equatable);

            var props = new List<DynamicPropertyInfo>();

            foreach (var member in args.Skip(1))
            {
                CreatePropertyWithField(scope, type, props, member);
            }

            GenerateConstructor(type, props);

            //var equalityContract = GenerateEqualityContractProperty(type);

            var appendString = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(string) })!;
            var appendChar = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(char) })!;
            
            GenerateToStringMethod(recordName, type, appendString, appendChar);

            var newType = type.CreateType();

            scope.Define(recordName.Value, newType);

            return newType;
        }

        private static void GenerateToStringMethod(Symbol recordName, TypeBuilder type, MethodInfo appendString, MethodInfo appendChar)
        {
            var toString = type.DefineMethod(nameof(object.ToString), MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(string), Type.EmptyTypes);
            var gen = toString.GetILGenerator();

            var sbLocal = gen.DeclareLocal(typeof(StringBuilder));

            gen.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(Type.EmptyTypes)!);
            gen.Emit(OpCodes.Stloc_0); // sb
            gen.Emit(OpCodes.Ldloc_0); // sb
            gen.Emit(OpCodes.Ldstr, recordName.Value);
            gen.Emit(OpCodes.Callvirt, appendString);
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ldloc_0); // sb
            gen.Emit(OpCodes.Ldstr, " { ");
            gen.Emit(OpCodes.Callvirt, appendString);
            gen.Emit(OpCodes.Pop);
            
            // TODO: implement PrintMembers method
            //gen.Emit(OpCodes.Ldarg_0); // this
            //gen.Emit(OpCodes.Ldloc_0); // sb
            //gen.Emit(OpCodes.Callvirt, printMembers);

            //var falseLabel = gen.DefineLabel();

            //gen.Emit(OpCodes.Brfalse_S, falseLabel);
            //gen.Emit(OpCodes.Ldloc_0); // sb
            //gen.Emit(OpCodes.Ldc_I4_S, 32); // 0x20, ' '
            //gen.Emit(OpCodes.Callvirt, appendChar);
            //gen.Emit(OpCodes.Pop);

            //gen.MarkLabel(falseLabel);
            gen.Emit(OpCodes.Ldloc_0); // sb
            gen.Emit(OpCodes.Ldc_I4_S, 125); // 0x7d, '}'
            gen.Emit(OpCodes.Callvirt, appendChar);
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ldloc_0); // sb
            gen.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(object.ToString), Type.EmptyTypes)!);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateProperty(TypeBuilder type, string name, Type propertyType, FieldInfo backingField, out MethodBuilder getter, out MethodBuilder setter)
        {
            var propBuilder = type.DefineProperty(name, PropertyAttributes.None, propertyType, Type.EmptyTypes);

            getter = type.DefineMethod($"get_{name}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, propertyType, Type.EmptyTypes);
            getter.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, new object?[0]));

            var gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0); // this
            gen.Emit(OpCodes.Ldfld, backingField);
            gen.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getter);

            setter = type.DefineMethod($"set_{name}", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, CallingConventions.HasThis, null, new[] { propertyType });
            setter.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, new object?[0]));
            setter.DefineParameter(0, ParameterAttributes.None, "value");

            gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0); // this
            gen.Emit(OpCodes.Ldarg_1); // value
            gen.Emit(OpCodes.Stfld, backingField);
            gen.Emit(OpCodes.Ret);

            propBuilder.SetSetMethod(setter);
        }

        private static PropertyInfo GenerateEqualityContractProperty(TypeBuilder type)
        {
            var equalityContract = type.DefineProperty("EqualityContract", PropertyAttributes.None, typeof(Type), Type.EmptyTypes);
            
            var getter = type.DefineMethod("get_EqualityContract", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName, CallingConventions.HasThis);
            getter.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, new object?[0]));

            var gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldtoken, type);
            gen.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) })!);
            gen.Emit(OpCodes.Ret);

            equalityContract.SetGetMethod(getter);

            return equalityContract;
        }

        private static void CreatePropertyWithField(Scope scope, TypeBuilder type, List<DynamicPropertyInfo> props, object? member)
        {
            string name = "";
            Type fieldType = typeof(object);

            if (member is Symbol memsym)
            {
                name = memsym.Value;
            }
            else if (member is Pair { Car: Symbol mempairsym, Cdr: Pair { Car: Symbol memtypesym } })
            {
                name = mempairsym.Value;
                // TODO: support generics here with the arity parameter (null below)
                fieldType = Interop.ResolveSymbol(scope, memtypesym.Value, null) as Type ?? throw new InvalidOperationException($"Expression for field {name} did not resolve to a type");
            }

            var fieldName = $"<{name}>k__BackingField";

            var field = type.DefineField(fieldName, fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
            field.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, new object?[0]));
            field.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new Type[] { typeof(DebuggerBrowsableState) })!, new object?[] { DebuggerBrowsableState.Never }));

            GenerateProperty(type, name, fieldType, field, out var getter, out var setter);

            props.Add(new DynamicPropertyInfo(name, fieldType, field, getter, setter));
        }

        private static void GenerateConstructor(TypeBuilder type, List<DynamicPropertyInfo> props)
        {
            var ctor = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, props.Select(i => i.PropertyType).ToArray());

            for (int i = 0; i < props.Count; i++)
            {
                ctor.DefineParameter(i, ParameterAttributes.None, props[i].Name);
            }

            var ctorGenerator = ctor.GetILGenerator();

            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];

                ctorGenerator.Emit(OpCodes.Ldarg_0); // this

                var opcode = i switch
                {
                    0 => OpCodes.Ldarg_1,
                    1 => OpCodes.Ldarg_2,
                    2 => OpCodes.Ldarg_3,
                    _ => OpCodes.Ldarg_S
                };

                if (opcode == OpCodes.Ldarg_S)
                    ctorGenerator.Emit(opcode, i + 1);
                else
                    ctorGenerator.Emit(opcode);

                ctorGenerator.Emit(OpCodes.Stfld, prop.BackingField);
            }

            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
            ctorGenerator.Emit(OpCodes.Ret);
        }

        private record DynamicPropertyInfo(string Name, Type PropertyType, FieldInfo BackingField, MethodInfo Getter, MethodInfo Setter);
    }
}
