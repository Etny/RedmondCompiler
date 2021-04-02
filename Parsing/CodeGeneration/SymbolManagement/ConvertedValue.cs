using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ConvertedValue : CodeValue
    {

        public readonly CodeValue Original;
        private InterCall _convertCall;
        private InterMethod _owner;

        public ConvertedValue(CodeValue orignal, CodeType convert, InterMethod owner)
        {
            Original = orignal;
            Type = convert;
            _owner = owner;
        }

        public override void Bind(IntermediateBuilder context)
        {
            Original.Bind(context);
            var method = Type.GetConversionMethod(context, Original);
            if (method == null) return;
            _convertCall = new InterCall(method, true, Original);
            _convertCall.SetOwner(_owner);
            _convertCall.Bind(context);
        }

        public override string ToString()
            => Original.ToString() + " Converted To " + Type.Name;

        public override void Push(IlBuilder builder)
        {
            if (_convertCall == null)
                Type.ConvertTo(Original, builder, Type);
            else
                _convertCall.Emit(builder);
        }
    }
}
