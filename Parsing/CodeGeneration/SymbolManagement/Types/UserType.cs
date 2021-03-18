using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class UserType : CodeType
    {
        public static UserType ToUserType(CodeType type)
        {
            var user = type as UserType;
            if (type != null) return user;
            return user; //TODO: add boxing;
        }


        private Type _type;
        public bool ValueType => _valuetype;
        protected bool _valuetype;

        protected UserType() : base("", "") {}

        public UserType(Type type) : this()
        {
            _type = type;

            _valuetype = type.IsValueType;

            Name = $"{(_valuetype ? "valuetype" : "class")} [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }

        public virtual string SpecName => $"[{_type.Module.Assembly.GetName().Name}]{_type.FullName}";

        public override bool Equals(object obj)
           => obj is UserType type && type.Name == Name;

        public override CodeType GetWiderType(CodeType otherType)
        {
            var other = otherType as UserType;

            if (other == null) return null;

            //TODO: check inheritance
            return null;
        }

        public virtual IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.GetMethods())
                yield return new MethodInfoWrapper(f, context);
        }

        public virtual IEnumerable<IMethodWrapper> GetConstructors(IntermediateBuilder context)
        {
            foreach (var f in _type.GetConstructors())
                yield return new ConstructorInfoWrapper(f, context);
        }

        public virtual IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _type.GetFields())
                yield return new FieldInfoWrapper(f, context);
        }

        public virtual IEnumerable<IPropertyWrapper> GetProperties(IntermediateBuilder context)
        {
            foreach (var f in _type.GetProperties())
                yield return new PropertyInfoWrapper(f, context);
        }

        public override void Convert(CodeValue val, IlBuilder builder)
        {
            throw new NotImplementedException();
        }
    }

    class InterUserType : UserType
    {
        private InterType _type;

        public InterUserType(InterType type) : base()
        {
            _type = type;

            Name = $"class {_type.FullName}";
        }

        public override string SpecName => $"{_type.FullName}";

        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.Methods)
                    yield return new InterMethodWrapper(f, context);
        }

        public override IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _type.Fields)
                yield return new InterFieldWrapper(f);
        }

        public override IEnumerable<IMethodWrapper> GetConstructors(IntermediateBuilder context)
        {
            foreach (var f in _type.Constructors)
                yield return new InterMethodWrapper(f, context);
        }
    }

}
