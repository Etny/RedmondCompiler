using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class NullValue : CodeValue
    {

        public NullValue() { Type = CodeType.Null; }

        public override string ToString() => "Null Value";

        public override void Push(IlBuilder builder)
            => builder.EmitOpCode(OpCodes.Ldnull);
    }
}
