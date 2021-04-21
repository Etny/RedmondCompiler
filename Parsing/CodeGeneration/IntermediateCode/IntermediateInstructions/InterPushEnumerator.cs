using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions
{
    class InterPushEnumerator : InterOp
    {
        private CodeType _type = null, _valType;
        private CodeValue _owner, _param;
        private InterCall _call;

        public InterPushEnumerator(CodeValue collection, CodeValue type)
        {
            _owner = collection;
            _param = type;
        }

        public override void Bind(IntermediateBuilder context)
        {
            if (_type != null) return;
            base.Bind(context);

            _owner.Bind(context);
            _param.Bind(context);

            _valType = _param.Type;
            _type = GenericType.NewGenericType(UserType.ToUserType(context.ResolveType(new BasicTypeName("System.Collections.Generic.IEnumerator`1"))), new CodeType[] { _valType });
            CodeType ownerType = GenericType.NewGenericType(UserType.ToUserType(context.ResolveType(new BasicTypeName("System.Collections.Generic.IEnumerable`1"))), new CodeType[] { _valType });

            _call = new InterCall("GetEnumerator", new CodeValue[0], true, _owner) { ThisPointerTypeOverride = ownerType };
            _call.Bind(context);
        }

        public override void Emit(IlBuilder builder)
        {
            base.Emit(builder);
            _call.Emit(builder);
        }

        public override CodeType GetResultType() => _type;
    }
}
