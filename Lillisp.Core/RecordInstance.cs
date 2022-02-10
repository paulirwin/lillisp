namespace Lillisp.Core;

public record RecordInstance(RecordTypeDefinition RecordType)
{
    public IDictionary<Symbol, object?> Fields { get; set; } = new Dictionary<Symbol, object?>();
}