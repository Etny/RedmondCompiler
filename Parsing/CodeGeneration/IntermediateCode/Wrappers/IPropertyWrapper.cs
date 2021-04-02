using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IPropertyWrapper
    {
        bool CanRead { get; }
        bool CanWrite { get; }

        IMethodWrapper GetFunction { get; }

        IMethodWrapper SetFunction { get; }

        bool IsStatic { get; }

        string Name { get; }

        CodeType Type { get; }
    }

    class PropertyInfoWrapper : IPropertyWrapper
    {

        private PropertyInfo _property;
        private IntermediateBuilder _context;

        public PropertyInfoWrapper(PropertyInfo property, IntermediateBuilder context)
        {
            _property = property;
            _context = context;
        }

        public bool CanRead => _property.CanRead;
        public bool CanWrite => _property.CanWrite;

        public IMethodWrapper GetFunction => new MethodInfoWrapper(_property.GetGetMethod(), _context);
        public IMethodWrapper SetFunction => new MethodInfoWrapper(_property.GetSetMethod(), _context);


        public bool IsStatic => _property.GetGetMethod().IsStatic;

        public string Name => _property.Name;

        public CodeType Type => _context.ToCodeType(_property.PropertyType);

    }
}
