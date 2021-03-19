using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class FieldSymbol : CodeSymbol
    {

        private CodeValue _owner = null;
        private UserType _owningType = null;
        private InterField _field = null;

        private bool _initFromInterField = false;

        public IFieldWrapper Field { get; protected set; }
        public FieldSymbol(CodeValue owner, string name) : base()
        {
            _owner = owner;
            ID = name;
        }

        public FieldSymbol(UserType owner, string name) : base()
        {
            _owningType = owner;
            ID = name;
        }
        public FieldSymbol(InterField field) : base()
        {
            _field = field;
            _owningType = field.Owner;
            Field = new InterFieldWrapper(field);
            ID = field.Name;

            _initFromInterField = true;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _owner?.Bind(context);

            if (Field != null) { Type = Field.Type; return; }

            UserType type = (_owner != null ? _owner.Type : _owningType) as UserType;

            if (_owningType == null) _owningType = type;

            IFieldWrapper matchField = null;

            foreach (var f in type.GetFields(context))
                if (f.Name == ID) { matchField = f; break; }

            //Debug.Assert(matchField != null);

            if (matchField == null) return;

            Type = matchField.Type;
            Field = matchField;
        }

        public void SetOwner(CodeValue owner)
        {
            if (Field.IsStatic) return;
            _owner = owner;
        }
        public override void Push(IlBuilder builder)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldfld, Type.Name, $"{_owningType.ShortName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsfld, Type.Name, $"{_owningType.ShortName}::{ID}");

        }

        public override void PushAddress(IlBuilder builder)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldflda, Type.Name, $"{_owningType.ShortName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsflda, Type.Name, $"{_owningType.ShortName}::{ID}");
        }

        public override void Store(IlBuilder builder, CodeValue store)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stfld, Type.Name, $"{_owningType.ShortName}::{ID}");
            }
            else
            {
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stsfld, Type.Name, $"{_owningType.ShortName}::{ID}");
            }
        }

    }
}
