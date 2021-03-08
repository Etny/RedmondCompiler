using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class FieldSymbolLocation : CodeSymbolLocation
    {

        public readonly InterField Field;

        public FieldSymbolLocation(InterField field)
        {
            Field = field;
        }

        public override void Push(IlBuilder builder)
        {
            builder.EmitLine($"ldfld {Field.Owner.Name}::{Field.Name}");
        }

        public override void Store(IlBuilder builder)
        {
            builder.EmitLine($"stfld {Field.Owner.Name}::{Field.Name}");
        }

    }
}
