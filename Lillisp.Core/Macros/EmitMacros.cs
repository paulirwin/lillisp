using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lillisp.Core.Macros;

public static class EmitMacros
{
    private static readonly MethodInfo appendString = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(string) })!;
    private static readonly MethodInfo appendChar = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(char) })!;
    private static readonly MethodInfo appendObject = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(object) })!;
    private static readonly MethodInfo objectToString = typeof(object).GetMethod(nameof(object.ToString), Type.EmptyTypes)!;

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

        var type = CreateTypeBuilder(scope, recordName.Value, TypeAttributes.Public | TypeAttributes.BeforeFieldInit | TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass);

        var equatable = typeof(IEquatable<>).MakeGenericType(type);
        type.AddInterfaceImplementation(equatable);

        var props = new List<DynamicPropertyInfo>();

        foreach (var member in args.Skip(1))
        {
            CreatePropertyWithField(scope, type, props, member);
        }

        GenerateConstructor(type, props);

        var equalityContract = GenerateEqualityContractProperty(type);

        var equatableEquals = GenerateIEquatableEqualsMethod(type, equalityContract, props);

        GenerateEqualsOverrideMethod(type, equatableEquals);

        var opEquality = GenerateEqualityOperator(type, equatableEquals);

        GenerateInequalityOperator(type, opEquality);

        var printMembers = GeneratePrintMembersMethod(type, props);

        GenerateToStringMethod(recordName, type, printMembers);

        var newType = type.CreateType();

        scope.Define(recordName.Value, newType);

        return newType;
    }

    private static TypeBuilder CreateTypeBuilder(Scope scope, string typeName, TypeAttributes attributes, Type? parent = null)
    {
        var module = CreateModuleBuilder(scope);

        return module.DefineType(typeName, attributes, parent);
    }

    private static ModuleBuilder CreateModuleBuilder(Scope scope)
    {
        var assy = scope.AssemblyBuilder ?? AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Lillisp.User"), AssemblyBuilderAccess.RunAndCollect);

        return assy.DefineDynamicModule(Guid.NewGuid().ToString("N"));
    }

    public static object? DefineEnum(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("defenum requires at least one argument");
        }

        if (args[0] is not Symbol enumName)
        {
            throw new ArgumentException("defenum's first argument must be the record name");
        }

        var module = CreateModuleBuilder(scope);

        var type = module.DefineEnum(enumName.Value, TypeAttributes.Public, typeof(int));

        int value = 0;

        foreach (var arg in args.Skip(1))
        {
            if (arg is not Symbol argSymbol)
            {
                throw new InvalidOperationException("Field definitions must be a symbol");
            }

            type.DefineLiteral(argSymbol.Value, value++);
        }

        var newType = type.CreateType();

        scope.Define(enumName.Value, newType);

        return newType;
    }

    private static void GenerateInequalityOperator(TypeBuilder type, MethodBuilder opEquality)
    {
        var op = type.DefineMethod("op_Inequality", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName, CallingConventions.Standard, typeof(bool), new Type[] { type, type });
        op.DefineParameter(1, ParameterAttributes.None, "left");
        op.DefineParameter(2, ParameterAttributes.None, "right");

        var gen = op.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0); // left
        gen.Emit(OpCodes.Ldarg_1); // right
        gen.Emit(OpCodes.Call, opEquality);
        gen.Emit(OpCodes.Ldc_I4_0);
        gen.Emit(OpCodes.Ceq);
        gen.Emit(OpCodes.Ret);
    }

    private static void GenerateEqualsOverrideMethod(TypeBuilder type, MethodBuilder equatableEquals)
    {
        var objectEquals = typeof(object).GetMethod(nameof(object.Equals), BindingFlags.Public | BindingFlags.Instance);
        var equals = type.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(bool), new[] { typeof(object) });
        equals.DefineParameter(1, ParameterAttributes.None, "obj");

        var gen = equals.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0); // this
        gen.Emit(OpCodes.Ldarg_1); // obj
        gen.Emit(OpCodes.Isinst, type);
        gen.Emit(OpCodes.Callvirt, equatableEquals);
        gen.Emit(OpCodes.Ret);
    }

    private static MethodBuilder GenerateIEquatableEqualsMethod(TypeBuilder type, PropertyInfo equalityContract, IList<DynamicPropertyInfo> props)
    {
        var equatableEquals = type.DefineMethod(nameof(IEquatable<object>.Equals), MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, typeof(bool), new Type[] { type });
        equatableEquals.DefineParameter(1, ParameterAttributes.None, "other");

        var gen = equatableEquals.GetILGenerator();

        var trueLabel = gen.DefineLabel();
        var falseLabel = gen.DefineLabel();
        var branchToRetLabel = gen.DefineLabel();
        var retLabel = gen.DefineLabel();

        gen.Emit(OpCodes.Ldarg_0); // this
        gen.Emit(OpCodes.Ldarg_1); // other
        gen.Emit(OpCodes.Beq, trueLabel);

        gen.Emit(OpCodes.Ldarg_1); // other
        gen.Emit(OpCodes.Brfalse, falseLabel);

        gen.Emit(OpCodes.Ldarg_0); // this
        gen.Emit(OpCodes.Callvirt, equalityContract.GetMethod!);
        gen.Emit(OpCodes.Ldarg_1); // other
        gen.Emit(OpCodes.Callvirt, equalityContract.GetMethod!);

        var typeOpEquality = typeof(Type).GetMethod("op_Equality", new[] { typeof(Type), typeof(Type) });
        gen.Emit(OpCodes.Call, typeOpEquality!);
        gen.Emit(OpCodes.Brfalse, falseLabel);

        bool first = true;

        foreach (var prop in props)
        {
            var eqComparer = typeof(EqualityComparer<>).MakeGenericType(prop.PropertyType);
            var eqDefault = eqComparer.GetProperty(nameof(EqualityComparer<object>.Default), BindingFlags.Public | BindingFlags.Static);
            var eqEquals = eqComparer.GetMethod(nameof(EqualityComparer<object>.Equals), BindingFlags.Public | BindingFlags.Instance, new[] { prop.PropertyType, prop.PropertyType });

            if (!first)
                gen.Emit(OpCodes.Brfalse_S, falseLabel);

            gen.Emit(OpCodes.Call, eqDefault!.GetGetMethod()!);
            gen.Emit(OpCodes.Ldarg_0); // this
            gen.Emit(OpCodes.Ldfld, prop.BackingField);
            gen.Emit(OpCodes.Ldarg_1); // other
            gen.Emit(OpCodes.Ldfld, prop.BackingField);
            gen.Emit(OpCodes.Callvirt, eqEquals!);                

            first = false;
        }

        gen.Emit(OpCodes.Br_S, branchToRetLabel);
            
        gen.MarkLabel(falseLabel);
        gen.Emit(OpCodes.Ldc_I4_0);
            
        gen.MarkLabel(branchToRetLabel);
        gen.Emit(OpCodes.Br_S, retLabel);
            
        gen.MarkLabel(trueLabel);
        gen.Emit(OpCodes.Ldc_I4_1);
            
        gen.MarkLabel(retLabel);
        gen.Emit(OpCodes.Ret);

        return equatableEquals;
    }

    private static MethodBuilder GenerateEqualityOperator(TypeBuilder type, MethodInfo equatableEquals)
    {
        var op = type.DefineMethod("op_Equality", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.SpecialName, CallingConventions.Standard, typeof(bool), new Type[] { type, type });
        op.DefineParameter(1, ParameterAttributes.None, "left");
        op.DefineParameter(2, ParameterAttributes.None, "right");

        var gen = op.GetILGenerator();

        var trueLabel = gen.DefineLabel();
        var falseLabel = gen.DefineLabel();
        var branchToRetLabel = gen.DefineLabel();
        var retLabel = gen.DefineLabel();

        gen.Emit(OpCodes.Ldarg_0); // left
        gen.Emit(OpCodes.Ldarg_1); // right
        gen.Emit(OpCodes.Beq_S, trueLabel);
        gen.Emit(OpCodes.Ldarg_0); // left
        gen.Emit(OpCodes.Brfalse_S, falseLabel);
        gen.Emit(OpCodes.Ldarg_0); // left
        gen.Emit(OpCodes.Ldarg_1); // right
        gen.Emit(OpCodes.Callvirt, equatableEquals);
        gen.Emit(OpCodes.Br_S, branchToRetLabel);

        gen.MarkLabel(falseLabel);
        gen.Emit(OpCodes.Ldc_I4_0);

        gen.MarkLabel(branchToRetLabel);
        gen.Emit(OpCodes.Br_S, retLabel);

        gen.MarkLabel(trueLabel);
        gen.Emit(OpCodes.Ldc_I4_1);

        gen.MarkLabel(retLabel);
        gen.Emit(OpCodes.Ret);

        return op;
    }

    private static MethodBuilder GeneratePrintMembersMethod(TypeBuilder type, IList<DynamicPropertyInfo> props)
    {
        var printMembers = type.DefineMethod("PrintMembers", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.HasThis, typeof(bool), new[] { typeof(StringBuilder) });
        printMembers.DefineParameter(1, ParameterAttributes.None, "builder");

        var gen = printMembers.GetILGenerator();
            
        var valueTypes = props.Select(i => i.PropertyType).Where(i => i.IsValueType).Distinct().ToList();

        foreach (var valueType in valueTypes)
        {
            gen.DeclareLocal(valueType);
        }

        gen.Emit(OpCodes.Call, typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.EnsureSufficientExecutionStack), Type.EmptyTypes)!);

        bool isFirst = true;

        foreach (var prop in props)
        {
            gen.Emit(OpCodes.Ldarg_1); // builder
            gen.Emit(OpCodes.Ldstr, $"{(isFirst ? "" : ", ")}{prop.Name} = ");
            gen.Emit(OpCodes.Callvirt, appendString);
            gen.Emit(OpCodes.Pop);

            gen.Emit(OpCodes.Ldarg_1); // builder
            gen.Emit(OpCodes.Ldarg_0); // this
            gen.Emit(OpCodes.Call, prop.Getter);

            int valueTypeIndex = valueTypes.IndexOf(prop.PropertyType);

            if (valueTypeIndex >= 0)
            {
                gen.Emit(OpCodes.Stloc_S, valueTypeIndex); // store value type to local
                gen.Emit(OpCodes.Ldloca_S, valueTypeIndex); // load address as managed pointer to local
                gen.Emit(OpCodes.Constrained, prop.PropertyType); // constrain value type to virtual call site type, or box
                gen.Emit(OpCodes.Callvirt, objectToString); // call Object.ToString() on constrained/boxed value
                gen.Emit(OpCodes.Callvirt, appendString);
            }
            else
            {
                gen.Emit(OpCodes.Callvirt, appendObject); // reference type, no need to constrain/box, can just call the Object overload
            }

            gen.Emit(OpCodes.Pop);

            isFirst = false;
        }

        gen.Emit(OpCodes.Ldc_I4_1);
        gen.Emit(OpCodes.Ret);

        return printMembers;
    }

    private static void GenerateToStringMethod(Symbol recordName, TypeBuilder type, MethodInfo printMembers)
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

        gen.Emit(OpCodes.Ldarg_0); // this
        gen.Emit(OpCodes.Ldloc_0); // sb
        gen.Emit(OpCodes.Callvirt, printMembers);

        var falseLabel = gen.DefineLabel();

        gen.Emit(OpCodes.Brfalse_S, falseLabel);
        gen.Emit(OpCodes.Ldloc_0); // sb
        gen.Emit(OpCodes.Ldc_I4_S, 32); // 0x20, ' '
        gen.Emit(OpCodes.Callvirt, appendChar);
        gen.Emit(OpCodes.Pop);

        gen.MarkLabel(falseLabel);
        gen.Emit(OpCodes.Ldloc_0); // sb
        gen.Emit(OpCodes.Ldc_I4_S, 125); // 0x7d, '}'
        gen.Emit(OpCodes.Callvirt, appendChar);
        gen.Emit(OpCodes.Pop);
        gen.Emit(OpCodes.Ldloc_0); // sb
        gen.Emit(OpCodes.Callvirt, objectToString);
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
        setter.DefineParameter(1, ParameterAttributes.None, "value");

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
            
        var getter = type.DefineMethod("get_EqualityContract", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName, CallingConventions.HasThis, typeof(Type), Type.EmptyTypes);
        getter.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, new object?[0]));

        var getTypeFromHandle = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Public | BindingFlags.Static, new[] { typeof(RuntimeTypeHandle) })!;

        var gen = getter.GetILGenerator();

        gen.Emit(OpCodes.Ldtoken, type);
        gen.Emit(OpCodes.Call, getTypeFromHandle);
        gen.Emit(OpCodes.Ret);

        equalityContract.SetGetMethod(getter);

        return equalityContract;
    }

    private static void CreatePropertyWithField(Scope scope, TypeBuilder type, List<DynamicPropertyInfo> props, object? member)
    {
        string name = "";
        var fieldType = typeof(object);

        if (member is Symbol memsym)
        {
            name = memsym.Value;
        }
        else if (member is Pair { Car: Symbol mempairsym, Cdr: Pair { Car: Symbol memtypesym } })
        {
            name = mempairsym.Value;
            // TODO: support generics here with the arity parameter (null below)
            if (!Interop.TryResolveSymbol(scope, memtypesym.Value, null, out var resolvedValue)
                || resolvedValue is not Type resolvedType) 
                throw new InvalidOperationException($"Expression for field {name} did not resolve to a type");

            fieldType = resolvedType;
        }

        var fieldName = $"<{name}>k__BackingField";

        var field = type.DefineField(fieldName, fieldType, FieldAttributes.Private | FieldAttributes.InitOnly);
        field.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, Array.Empty<object?>()));
        field.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) })!, new object?[] { DebuggerBrowsableState.Never }));

        GenerateProperty(type, name, fieldType, field, out var getter, out var setter);

        props.Add(new DynamicPropertyInfo(name, fieldType, field, getter, setter));
    }

    private static void GenerateConstructor(TypeBuilder type, List<DynamicPropertyInfo> props)
    {
        var ctor = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, props.Select(i => i.PropertyType).ToArray());

        for (int i = 0; i < props.Count; i++)
        {
            ctor.DefineParameter(i + 1, ParameterAttributes.None, props[i].Name);
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