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

        for (int i = 0; i < PatternNodes.Count; i++)
        {
            var patternNode = PatternNodes[i];
            var arg = args.Length > i ? args[i] : null;

            if (!NodeMatches(patternNode, arg))
                return false;

            if (patternNode is Symbol patternVariable)
            {
                replacements ??= new Dictionary<Symbol, Node?>();
                replacements[patternVariable] = arg;
            }
        }

        node = TransformNode(syntaxScope, TemplateNode, replacements);

        return true;
    }

    private static Node? TransformNode(Scope syntaxScope, Node node, IDictionary<Symbol, Node?>? replacements)
    {
        return node switch
        {
            Symbol symbol => replacements != null && replacements.TryGetValue(symbol, out var replacementNode) ? replacementNode : new SyntaxBinding(symbol, syntaxScope),
            Pair pair => TransformPair(syntaxScope, replacements, pair),
            _ => node,
        };
    }

    private static Node? TransformPair(Scope syntaxScope, IDictionary<Symbol, Node?>? replacements, Pair pair)
    {
        object? car = TransformPairCar(syntaxScope, replacements, pair);

        while (car == null && pair.Cdr is Pair carCdrPair)
        {
            pair = carCdrPair;
            car = TransformPairCar(syntaxScope, replacements, pair);
        }

        var cdr = pair.Cdr switch
        {
            Pair cdrPair => TransformPair(syntaxScope, replacements, cdrPair),
            Node cdrNode => TransformNode(syntaxScope, cdrNode, replacements),
            _ => pair.Cdr
        };

        return car switch
        {
            null when cdr == null => Nil.Value,
            null => new Pair(cdr, Nil.Value),
            _ => new Pair(car, cdr ?? Nil.Value)
        };
    }

    private static object? TransformPairCar(Scope syntaxScope, IDictionary<Symbol, Node?>? replacements, Pair pair)
    {
        return pair.Car is Node node ? TransformNode(syntaxScope, node, replacements) : pair.Car;
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