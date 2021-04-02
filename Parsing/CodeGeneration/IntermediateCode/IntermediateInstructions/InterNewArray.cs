using Redmond.Common;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterNewArray : InterOp
    {

        private TypeName _type = TypeName.Unknown;
        private CodeValue _rank = null;
        private CodeValue[] _entries = null;
        private CodeType _typeof;

        public InterNewArray(TypeName type, CodeValue rank)
        {
            _type = type;
            _rank = rank;
        }

        public InterNewArray(TypeName type, CodeValue[] entries)
        {
            _type = type;
            _entries = entries;
        }

        public InterNewArray(CodeValue[] entries)
        {
            _entries = entries;
        }


        public override void Bind(IntermediateBuilder context)
        {
            base.Bind(context);

            if(_rank == null)
            {
                foreach (var e in _entries) e.Bind(context);
                _rank = new CodeValue(BasicType.Int32, _entries.Length);
            } 
            
            _rank.Bind(context);



            _typeof = _type == TypeName.Unknown ? new ArrayType(_entries[0].Type) : context.ResolveType(_type);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            builder.PushValue(_rank);

            builder.EmitOpCode(OpCodes.Newarr, (_typeof as ArrayType).TypeOf.Name);

            for(int i = 0; i < _entries.Length; i++)
            {
                builder.EmitOpCode(OpCodes.Dup);

                builder.PushValue(new CodeValue(BasicType.Int32, i));
                builder.PushValue(_entries[i]);
                builder.EmitOpCode(OpCodeUtil.GetOpcode("Stelem_" + _typeof.OpName));
            }
        }

        public override CodeType GetResultType()
            => _typeof;
    }
}
