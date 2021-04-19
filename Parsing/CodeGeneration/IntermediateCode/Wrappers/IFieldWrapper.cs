using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IFieldWrapper
    {
        bool IsStatic { get; }

        string Name { get; }

        CodeType Type { get; }
    }

    class InterFieldWrapper : IFieldWrapper
    {

        private InterField _field;

        public InterFieldWrapper(InterField field) { _field = field;  }

        public bool IsStatic => _field.IsStatic;

        public string Name => _field.Name;

        public virtual CodeType Type => _field.Type;
    }

    class GenericInterFieldWrapper : InterFieldWrapper
    {
        private InterGenericType _type;
        private CodeType _fieldType;

        public GenericInterFieldWrapper(InterField field, InterGenericType type) : base(field)
        {
            _type = type;

            _fieldType = InterGenericType.FinalizeInterGenericType(field.Type, _type.GenericParameters);
        }

        public override CodeType Type => _fieldType;

    }

    class FieldInfoWrapper : IFieldWrapper
    {

        private FieldInfo _field;
        private IntermediateBuilder _context;

        public FieldInfoWrapper(FieldInfo field, IntermediateBuilder context)
        {
            _field = field;
            _context = context;
        }

        public bool IsStatic => _field.IsStatic;

        public string Name => _field.Name;

        public virtual CodeType Type => _context.ToCodeType(_field.FieldType);

    }

    class GenericFieldInfoWrapper : FieldInfoWrapper
    {
        private GenericType _type;
        private CodeType _fieldType;

        public GenericFieldInfoWrapper(FieldInfo field, IntermediateBuilder context, GenericType type) : base(field, context)
        {
            _type = type;

            _fieldType = GenericType.FinalizeGenericType(field.FieldType, context, _type.GenericParameters);
        }

        public override CodeType Type => _fieldType;
    }
}
