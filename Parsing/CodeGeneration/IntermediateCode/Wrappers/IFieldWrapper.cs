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

            if (field.Type is GenericParameterType)
            {
                var par = field.Type as GenericParameterType;
                _fieldType = new GenericParameterType(_type.GenericParameters[par.Index], par.Index);
            }
            else
                _fieldType = field.Type;
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

            if (field.FieldType.IsGenericParameter)
            {
                _fieldType = new GenericParameterType(type.GenericParameters[field.FieldType.GenericParameterPosition], field.FieldType.GenericParameterPosition);
            }
            else
                _fieldType = context.ToCodeType(field.FieldType);
        }

        public override CodeType Type => _fieldType;
    }
}
