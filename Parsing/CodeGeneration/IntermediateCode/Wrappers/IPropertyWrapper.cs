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

        protected PropertyInfo _property;
        protected IntermediateBuilder _context;

        public PropertyInfoWrapper(PropertyInfo property, IntermediateBuilder context)
        {
            _property = property;
            _context = context;
        }

        public bool CanRead => _property.CanRead;
        public bool CanWrite => _property.CanWrite;

        public virtual IMethodWrapper GetFunction => new MethodInfoWrapper(_property.GetGetMethod(), _context);
        public virtual IMethodWrapper SetFunction => new MethodInfoWrapper(_property.GetSetMethod(), _context);


        public bool IsStatic => _property.GetGetMethod().IsStatic;

        public string Name => _property.Name;

        public virtual CodeType Type => _context.ToCodeType(_property.PropertyType);

    }

    class GenericPropertyInfoWrapper : PropertyInfoWrapper
    {
        private GenericType _type;

        public GenericPropertyInfoWrapper(PropertyInfo property, IntermediateBuilder context, GenericType type) : base(property, context)
        {
            _type = type;
        }

        public override IMethodWrapper GetFunction => new GenericMethodInfoWrapper(_property.GetGetMethod(), _context, _type);
        public override IMethodWrapper SetFunction => new GenericMethodInfoWrapper(_property.GetSetMethod(), _context, _type);

        public override CodeType Type {
            get
            {
                if (!_property.PropertyType.IsGenericParameter)
                    return _context.ToCodeType(_property.PropertyType);
                else
                    return new GenericParameterType(_type.GenericParameters[_property.PropertyType.GenericParameterPosition], _property.PropertyType.GenericParameterPosition);
            }
        }


    }
}
