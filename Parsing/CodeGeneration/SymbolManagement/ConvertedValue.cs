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

        public override void Bind(IntermediateBuilder context)
        {
            Original.Bind(context);
            Type.BindConversion(context, Original);
        }

        public override string ToString()
            => Original.ToString() + " Converted To " + Type.Name;

        public override void Push(IlBuilder builder)
        {
            Type.Convert(Original, builder);
        }
    }
}
