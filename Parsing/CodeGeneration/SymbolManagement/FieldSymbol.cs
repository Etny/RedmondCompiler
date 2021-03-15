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

        public bool IsStatic = false;

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
            ID = field.Name;
        }

        public override void BindType(IntermediateBuilder context)
        {
            _owner?.BindType(context);

            if (Type != null) return;

            UserType type = (_owner != null ? _owner.Type : _owningType) as UserType;

            if (_owningType == null) _owningType = type;

            IFieldWrapper match = null;

            if (_field == null)
            {
                foreach (var f in type.GetFields(context))
                    if (f.Name == ID) { match = f; break; }
            }
            else
                match = new InterFieldWrapper(_field);

            Debug.Assert(match != null);

            Type = match.Type;
            IsStatic = match.IsStatic;
        }

        public override void Push(IlBuilder builder)
        {
            if (!IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldfld, Type.Name, $"{_owningType.SpecName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsfld, Type.Name, $"{_owningType.SpecName}::{ID}");

        }

        public override void PushAddress(IlBuilder builder)
        {
            if (!IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldflda, Type.Name, $"{_owningType.SpecName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsflda, Type.Name, $"{_owningType.SpecName}::{ID}");
        }

        public override void Store(IlBuilder builder, CodeValue store)
        {
            if (!IsStatic)
            {
                builder.PushValue(_owner);
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stfld, Type.Name, $"{_owningType.SpecName}::{ID}");
            }
            else
            {
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stsfld, Type.Name, $"{_owningType.SpecName}::{ID}");
            }
        }

    }
}
