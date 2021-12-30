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
    
    public bool TryTransform(Node[] args, out Node? node)
    {
        node = null;

        if (args.Length != PatternNodes.Count)
            return false; // TODO: remove this, support ellipsis patterns

        IDictionary<Symbol, Node>? replacements = null;

        for (int i = 0; i < PatternNodes.Count; i++)
        {
            var patternNode = PatternNodes[i];
            var arg = args[i];

            if (!NodeMatches(patternNode, arg))
                return false;

            if (patternNode is Symbol patternVariable)
            {
                replacements ??= new Dictionary<Symbol, Node>();
                replacements[patternVariable] = arg;
            }
        }

        node = TransformNode(TemplateNode, replacements);

        return true;
    }

    private static Node TransformNode(Node node, IDictionary<Symbol, Node>? replacements)
    {
        return node switch
        {
            Symbol symbol => replacements != null && replacements.TryGetValue(symbol, out var replacementNode) ? replacementNode : symbol,
            Pair { Car: Node pairCar, Cdr: Node pairCdr }=> new Pair(TransformNode(pairCar, replacements), TransformNode(pairCdr, replacements)),
            _ => node,
        };
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