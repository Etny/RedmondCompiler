using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterNew : InterOp
    {

        private InterInstOperand[] _parameters;
        private string _typeName;
        private CodeType _type;

        private IMethodWrapper _constructor;

        public InterNew(string typeName, InterInstOperand[] parameters)
        {
            _typeName = typeName;
            _parameters = parameters;
        }

        public override void Bind(IntermediateBuilder context)
        {
            foreach (var p in _parameters)
                p.Bind(context);

            _type = context.ResolveType(_typeName);

            _constructor = context.FindMostApplicableConstructor(_type as UserType, _parameters);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            for (int i = 0; i < _parameters.Length; i++)
            {
                _parameters[i].Emit(builder);
                if (_constructor.Arguments[i] != _parameters[i].Type) builder.EmitOpCode(_constructor.Arguments[i].ConvCode);
            }

            builder.EmitOpCode(OpCodes.Newobj, _constructor.FullSignature);

            builder.ShrinkStack(_parameters.Length);
        }

        public override CodeType GetResultType()
            => _type;
    }
}
