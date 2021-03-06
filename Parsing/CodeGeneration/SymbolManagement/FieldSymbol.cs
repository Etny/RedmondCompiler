﻿using Redmond.Parsing.CodeGeneration.IntermediateCode;
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
        private TypeName _owningTypeName = TypeName.Unknown;
        private InterField _field = null;

        private bool _initFromInterField = false;

        public IFieldWrapper Field { get; protected set; }
        public FieldSymbol(CodeValue owner, string name) : base()
        {
            _owner = owner;
            ID = name;
        }

        public FieldSymbol(CodeValue owner, TypeName owningTypeName, string name) : base()
        {
            _owner = owner;
            _owningTypeName = owningTypeName;
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
            if (_owningTypeName != TypeName.Unknown) 
                _owningType = UserType.ToUserType(context.ResolveType(_owningTypeName));

            if (Field != null) { Type = Field.Type; return; }

            UserType type = (_owningType != null ? _owningType : _owner.Type) as UserType;

            if (_owningType == null) _owningType = type;

            IFieldWrapper matchField = null;

            var t = type;

            while (matchField == null && t != CodeType.Object)
            {
                foreach (var f in t.GetFields(context))
                    if (f.Name == ID) { matchField = f; break; }
                if (matchField == null) t = t.GetBaseType();
            }

            _owningType = t;

            //Debug.Assert(matchField != null);

            if (matchField == null) return;

            Type = matchField.Type;
            Field = matchField;
        }

        public bool HasOwner() => _owner != null;

        public void SetOwner(CodeValue owner)
        {
            if (Field.IsStatic) return;
            _owner = owner;
        }

        private string ReferenceName => _owningType.IsGeneric ? _owningType.Name : _owningType.ShortName;

        public override void Push(IlBuilder builder)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldfld, Type.ArgumentName, $"{ReferenceName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsfld, Type.ArgumentName, $"{ReferenceName}::{ID}");

        }

        public override void PushAddress(IlBuilder builder)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.EmitOpCode(OpCodes.Ldflda, Type.ArgumentName, $"{ReferenceName}::{ID}");
            }
            else
                builder.EmitOpCode(OpCodes.Ldsflda, Type.ArgumentName, $"{ReferenceName}::{ID}");
        }

        public override void Store(IlBuilder builder, CodeValue store)
        {
            if (!Field.IsStatic)
            {
                builder.PushValue(_owner);
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stfld, Type.ArgumentName, $"{ReferenceName}::{ID}");
            }
            else
            {
                builder.PushValue(store);
                builder.EmitOpCode(OpCodes.Stsfld, Type.ArgumentName, $"{ReferenceName}::{ID}");
            }
        }

    }
}
