using System.Collections.Generic;

namespace Lillisp.Core;

public class SyntaxRule
{
    public SyntaxRule(Symbol keyword, IList<Node> patternNodes, Node templateNode)
    {
        Keyword = keyword;
        PatternNodes = patternNodes;
        TemplateNode = templateNode;
    }

    public Symbol Keyword { get; }

    public IList<Node> PatternNodes { get; }

    public Node TemplateNode { get; }

    public override string ToString() => $"(({Keyword}{(PatternNodes.Count > 0 ? " " : "")}{string.Join(' ', PatternNodes)}) {TemplateNode})";
    
    public bool TryTransform(Scope syntaxScope, Node[] args, out Node? node)
    {
        node = null;
        
        IDictionary<Symbol, Node?>? replacements = null;
        IList<Node>? restArgs = null;
        var argQueue = new Queue<Node>(args);

        foreach (var patternNode in PatternNodes)
        {
            var isRestPattern = patternNode is Symbol { Value: "..." };

            int count = isRestPattern ? argQueue.Count : 1;

            for (int j = count; j > 0; j--)
            {
                var arg = argQueue.Count == 0 ? null : argQueue.Dequeue();

                if (!NodeMatches(patternNode, arg))
                    return false;

                if (isRestPattern)
                {
                    restArgs ??= new List<Node>();

                    if (arg != null)
                    {
                        restArgs.Add(arg);
                    }
                }
                else if (patternNode is Symbol patternVariable)
                {
                    replacements ??= new Dictionary<Symbol, Node?>();
                    replacements[patternVariable] = arg;
                }
            }
        }

        if (argQueue.Count > 0)
            return false;

        node = TransformNode(syntaxScope, TemplateNode, replacements, restArgs);

        return true;
    }

    private static Node? TransformNode(Scope syntaxScope, Node node, IDictionary<Symbol, Node?>? replacements, IList<Node>? restArgs)
    {
        return node switch
        {
            Symbol { Value: "..." } => new SyntaxRestArgs(restArgs ?? new List<Node>()),
            Symbol symbol => replacements != null && replacements.TryGetValue(symbol, out var replacementNode) ? replacementNode : new SyntaxBinding(symbol, syntaxScope),
            Pair pair => TransformPair(syntaxScope, replacements, restArgs, pair),
            _ => node,
        };
    }

    private static Node? TransformPair(Scope syntaxScope, IDictionary<Symbol, Node?>? replacements, IList<Node>? restArgs, Pair pair)
    {
        object? car = TransformPairCar(syntaxScope, replacements, restArgs, pair);

        while (car == null && pair.Cdr is Pair carCdrPair)
        {
            pair = carCdrPair;
            car = TransformPairCar(syntaxScope, replacements, restArgs, pair);
        }

        var cdr = pair.Cdr switch
        {
            Pair cdrPair => TransformPair(syntaxScope, replacements, restArgs, cdrPair),
            Node cdrNode => TransformNode(syntaxScope, cdrNode, replacements, restArgs),
            _ => pair.Cdr
        };

        return car switch
        {
            SyntaxRestArgs sr => List.SpreadCar(sr, cdr),
            null when cdr == null => Nil.Value,
            null => new Pair(cdr, Nil.Value),
            _ => new Pair(car, cdr ?? Nil.Value)
        };
    }

    private static object? TransformPairCar(Scope syntaxScope, IDictionary<Symbol, Node?>? replacements, IList<Node>? restArgs, Pair pair)
    {
        return pair.Car is Node node ? TransformNode(syntaxScope, node, replacements, restArgs) : pair.Car;
    }

    private static bool NodeMatches(Node node, object? arg)
    {
        return node switch
        {
            SyntaxLiteral literal => literal.Symbol.Equals(arg),
            Atom atom => atom.Equals(arg),
            Symbol => true, // pattern variable
            Pair pair => arg is Pair pair2 && Equals(pair.Car, pair2.Car) && Equals(pair.Cdr, pair2.Cdr),
            _ => false
        };
    }
}