using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterNewArray : InterOp
    {

        private string _type;
        private CodeValue _rank;
        private CodeType _typeof;

        public InterNewArray(string type, CodeValue rank)
        {
            _type = type;
            _rank = rank;
        }

        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);

            _rank.Bind(context);

            _typeof = context.ResolveType(_type);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            builder.PushValue(_rank);

            builder.EmitOpCode(OpCodes.Newarr, _typeof.Name);
        }

        public override CodeType GetResultType()
            => new ArrayType(_typeof);
    }
}
