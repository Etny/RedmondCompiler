using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterNew : InterOp
    {

        private CodeValue[] _parameters;
        private string _typeName;
        private CodeType _type;

        private IMethodWrapper _constructor;

        public InterNew(string typeName, CodeValue[] parameters)
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


            for (int i = 0; i < _parameters.Length; i++)
            {
                if (_constructor.Arguments[i] == _parameters[i].Type) continue;

                _parameters[i] = new ConvertedValue(_parameters[i], _constructor.Arguments[i]);
                _parameters[i].Bind(context);
            }

        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);

            foreach (var p in _parameters)
                builder.PushValue(p);

            builder.EmitOpCode(OpCodes.Newobj, _constructor.FullSignature);

            builder.ShrinkStack(_parameters.Length);
            builder.ExpandStack(1);
        }

        public override CodeType GetResultType()
            => _type;
    }
}
