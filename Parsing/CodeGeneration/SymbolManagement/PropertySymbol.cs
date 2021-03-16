using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class PropertySymbol : CodeSymbol
    {

        private CodeValue _owner = null;
        private UserType _owningType = null;
        private InterField _field = null;

        public IPropertyWrapper Property { get; protected set; }

        private InterCall _get, _set;

        public PropertySymbol(CodeValue owner, string name) : base()
        {
            _owner = owner;
            ID = name;
        }

        public PropertySymbol(UserType owner, string name) : base()
        {
            _owningType = owner;
            ID = name;
        }

        public void SetOwner(CodeValue owner)
        {
            if (Property.IsStatic) return;
            _owner = owner;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _owner?.Bind(context);

            if (Type != null) return;

            UserType type = (_owner != null ? _owner.Type : _owningType) as UserType;

            if (_owningType == null) _owningType = type;

            IPropertyWrapper matchProperty = null;

            foreach (var f in type.GetProperties(context))
                if (f.Name == ID) { matchProperty = f; break; }

            Debug.Assert(matchProperty != null);

            Type = matchProperty.Type;
            Property = matchProperty;

            if (Property.CanRead)
            {
                _get = new InterCall(Property.GetFunction, new CodeValue[0], true, _owner);
                _get.SetOwner(context.CurrentMethod);
            }

            if (Property.CanWrite)
            {
                _set = new InterCall(Property.SetFunction, new CodeValue[1], true, _owner);
                _set.SetOwner(context.CurrentMethod);
            }
        }

        public override void Push(IlBuilder builder)
        {
            _get.Emit(builder);
        }


        public override void PushAddress(IlBuilder builder)
            => Debug.Assert(false);

        public override void Store(IlBuilder builder, CodeValue store)
        {
            _set.SetParameter(store);
            _set.Emit(builder);
        }

    }
}
