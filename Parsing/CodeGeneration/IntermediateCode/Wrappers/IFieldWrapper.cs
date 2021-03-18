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

        public CodeType Type => _field.Type;
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

        public CodeType Type => _context.ResolveType(_field.FieldType);

    }
}
