using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class FieldOrPropertySymbol : CodeSymbol
    {

        private FieldSymbol _field;
        private PropertySymbol _property;

        public bool IsField { get; protected set; } = false;
        public bool IsProperty { get; protected set; } = false;

        public static bool IsFieldOrProperty(CodeValue value)
            => value is FieldSymbol || value is PropertySymbol || value is FieldOrPropertySymbol;

        public static FieldOrPropertySymbol ToFieldOrPropertySymbol(CodeValue value)
        {
            if (value is FieldSymbol) return new FieldOrPropertySymbol(value as FieldSymbol);
            if (value is PropertySymbol) return new FieldOrPropertySymbol(value as PropertySymbol);

            return value as FieldOrPropertySymbol;
        }

        private FieldOrPropertySymbol(FieldSymbol field) : base()
        {
            _field = field;
            IsField = true;
            ID = field.ID;

            Type = field.Type;
        }

        private FieldOrPropertySymbol(PropertySymbol property) : base()
        {
            _property = property;
            IsProperty = true;
            ID = property.ID;

            Type = property.Type;
        }

        public FieldOrPropertySymbol(CodeValue owner, string name) : base()
        {
            _field = new FieldSymbol(owner, name);
            _property = new PropertySymbol(owner, name);
            ID = name;
        }

        public FieldOrPropertySymbol(CodeValue owner, TypeName owningTypeName, string name) : base()
        {
            _field = new FieldSymbol(owner, owningTypeName, name);
            _property = new PropertySymbol(owner, name);
            ID = name;
        }

        public FieldOrPropertySymbol(UserType owner, string name) : base()
        {
            _field = new FieldSymbol(owner, name);
            _property = new PropertySymbol(owner, name);
            ID = name;
        }

        public bool IsStatic
        {
            get
            {
                if (IsField)
                    return _field.Field.IsStatic;
                else
                    return _property.Property.IsStatic;
            }
        }

        public bool HasOwner()
        {
            if (IsField)
                return _field.HasOwner();
            else
                return _property.HasOwner();
        }

        public void SetOwner(CodeValue owner)
        {
            if (IsField)
                _field.SetOwner(owner);
            else
                _property.SetOwner(owner);
        }

        public override void Bind(IntermediateBuilder context)
        {
            _field.Bind(context);

            if (_field.Type == null)
            {
                IsProperty = true;
                _property.Bind(context);
                Type = _property.Type;
            }
            else
            {
                IsField = true;
                Type = _field.Type;
            }
        }

        public override void Push(IlBuilder builder)
        {
            if (IsField)
                _field.Push(builder);
            else
                _property.Push(builder);
        }

        public override void PushAddress(IlBuilder builder)
        {
            if (IsField)
                _field.PushAddress(builder);
            else
                _property.PushAddress(builder);
        }

        public override void Store(IlBuilder builder, CodeValue source)
        {
            if (IsField)
                _field.Store(builder, source);
            else
                _property.Store(builder, source);
        }
    }
}
