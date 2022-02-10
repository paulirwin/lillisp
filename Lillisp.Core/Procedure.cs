namespace Lillisp.Core;

public class Procedure : IInvokable
{
    public Procedure(string text, Node parameters, Node[] body)
    {
        Text = text;
        Parameters = parameters;
        Body = body;
    }

    public string Text { get; }

    public Node Parameters { get; }

    public Node[] Body { get; }

    public override string ToString() => Text;

    public object? Invoke(LillispRuntime runtime, Scope scope, object?[] args) => Invoke(runtime, scope, args, false);

    public object? Invoke(LillispRuntime runtime, Scope scope, object?[] args, bool disableTailCalls)
    {
        var childScope = scope.CreateChildScope();

        if (Parameters is Symbol pSymbol)
        {
            childScope.Define(pSymbol.Value, List.FromNodes(args));
        }
        else if (Parameters is Pair { IsList: true } parms)
        {
            var list = parms.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i) is not Symbol symbol)
                {
                    throw new ArgumentException($"Unhandled parameter node type: {list[i]?.GetType().ToString() ?? "null"}");
                }

                if (args.Length > i)
                {
                    var arg = args[i];

                    childScope.Define(symbol.Value, arg);
                }
            }
        }
        else if (Parameters is Pair { IsList: false } improperParms)
        {
            var list = improperParms.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (list.ElementAt(i) is not Symbol symbol)
                {
                    throw new ArgumentException($"Unhandled parameter node type: {list[i]?.GetType().ToString() ?? "null"}");
                }

                if (i == list.Count - 1)
                {
                    childScope.Define(symbol.Value, List.FromNodes(args.Skip(i)));

                    break;
                }

                if (args.Length > i)
                {
                    var arg = args[i];

                    childScope.Define(symbol.Value, arg);
                }
            }
        }

        object? result = Nil.Value;

        for (int i = 0; i < Body.Length; i++)
        {
            var bodyNode = Body[i];

            if (i < Body.Length - 1)
            {
                result = runtime.Evaluate(childScope, bodyNode);
            }
            else
            {
                result = bodyNode is Pair pair && !disableTailCalls ? LillispRuntime.TailCall(childScope, pair) : runtime.Evaluate(childScope, bodyNode);
            }
        }

        return result;
    }
}