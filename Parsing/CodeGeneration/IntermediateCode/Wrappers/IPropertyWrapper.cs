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

    class InterPropertyWrapper : IPropertyWrapper
    {
        protected InterProperty _property;
        protected IntermediateBuilder _context;
        public InterPropertyWrapper(InterProperty property, IntermediateBuilder context)
        {
            _property = property;
            _context = context;
        }

        public bool CanRead => _property.Get != null;

        public bool CanWrite => _property.Set != null;

        public virtual IMethodWrapper GetFunction => new InterMethodWrapper(_property.Get, _context);
        public virtual IMethodWrapper SetFunction => new InterMethodWrapper(_property.Set, _context);

        public bool IsStatic => _property.IsStatic;

        public string Name => _property.Name;

        public virtual CodeType Type => _property.Type;
    }

    class GenericInterPropertyWrapper : InterPropertyWrapper
    {
        private InterGenericType _type;

        private CodeType _propertyType;

        public GenericInterPropertyWrapper(InterProperty property, IntermediateBuilder context, InterGenericType type)
            : base(property, context)
        {
            _type = type;

            _propertyType = _type.InstantiateGenericType(property.Type);

        }

        public override IMethodWrapper GetFunction => new GenericInterMethodWrapper(_property.Get, _context, _type);
        public override IMethodWrapper SetFunction => new GenericInterMethodWrapper(_property.Set, _context, _type);


        public override CodeType Type => _propertyType;
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
            get => _type.InstantiateGenericType(_property.PropertyType, _context);
        }

        


    }
}
