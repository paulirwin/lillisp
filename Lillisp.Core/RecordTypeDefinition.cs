using System.Collections.Generic;

namespace Lillisp.Core;

public record RecordTypeDefinition(Symbol Name, Symbol ConstructorName, Symbol PredicateName)
{
    public IList<Symbol> ConstructorParameters { get; set; } = new List<Symbol>();

    public IList<RecordFieldDefinition> Fields { get; set; } = new List<RecordFieldDefinition>();
}