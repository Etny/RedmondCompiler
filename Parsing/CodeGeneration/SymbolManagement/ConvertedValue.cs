using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ConvertedValue : CodeValue
    {

        public readonly CodeValue Original;
        public ConvertedValue(CodeValue orignal, CodeType convert)
        {
            Original = orignal;
            Type = convert;
        }

        public override void BindType(IntermediateBuilder context)
        {
            if (Type != null) return;

            Original.BindType(context);
        }

        public override string ToString()
            => Original.ToString() + " Converted To " + Type.Name;

        public override void Push(IlBuilder builder)
        {
            Original.Push(builder);
            builder.EmitOpCode(Type.ConvCode);
        }
    }
}
