using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                builder.EmitLine($"ldfld {Type.Name} {_owningType.SpecName}::{ID}");
                builder.ExpandStack(1);
            }
            else
            {
                builder.EmitLine($"ldsfld {Type.Name} {_owningType.SpecName}::{ID}");
                builder.ExpandStack(1);
            }


        }

        public override void Store(IlBuilder builder)
        {
            if (!IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitLine($"stfld {Type.Name} {_owningType.SpecName}::{ID}");
                builder.ShrinkStack(2); //Shrink by 1 because this also removed the owner
            }
            else
            {
                builder.EmitLine($"stssfld {Type.Name} {_owningType.SpecName}::{ID}");
                builder.ShrinkStack(1);
            }
        }

    }
}
