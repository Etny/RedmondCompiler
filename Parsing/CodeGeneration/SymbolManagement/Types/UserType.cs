using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class UserType : CodeType
    {
        public static UserType ToUserType(CodeType type)
        {
            if (type is UserType) return type as UserType;
            if (type is BasicType) return (type as BasicType).BoxedType;
            return null;
        }

        protected static Dictionary<Type, UserType> _specialCases = new Dictionary<Type, UserType>();

        private static List<string> _userTypes = new List<string>();

        protected Type _type;
        public bool ValueType => _valuetype;
        protected bool _valuetype;

        protected UserType() : this("") {}

        protected UserType(params string[] names) : base(names) { OpName = "Ref"; }

        public static UserType NewUserType(Type t) 
        {
            if (_specialCases.ContainsKey(t)) return _specialCases[t];
            //if (t.IsArray) return new ArrayType(context.ResolveType(t.GetElementType()));
            else return new UserType(t);
        }

        public UserType(Type type) : this()
        {
            _type = type;

            _valuetype = type.IsValueType;

            Name = $"{(_valuetype ? "valuetype" : "class")} [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
            ShortName = $"[{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }

        public virtual Assembly GetAssembly() => _type.Assembly;

        public override bool Equals(object obj)
           => obj is UserType type && type.Name == Name;

        public override int GetHashCode()
        {
            if (!_userTypes.Contains(Name)) _userTypes.Add(Name);
            return _userTypes.IndexOf(Name);
        }

        public override CodeType GetWiderType(CodeType otherType)
        {
            var other = otherType as UserType;

            if (other == null) return null;

            return CanAssignTo(other) == AssignType.CanAssign ? this : other;
        }

        public override AssignType CanAssignTo(CodeType fieldType)
        {
            var other = fieldType as UserType;
            if (other == null) return AssignType.CannotAssign; 

            if (Equals(other)) return AssignType.CanAssign;

            UserType t = this;


            while (t != null && !t.Equals(other))
                t = t.GetBaseType();

            if (t != null) return AssignType.CanAssign;
            else return AssignType.CannotAssign;
        }
        public virtual UserType GetBaseType()
        {
            if (_type == typeof(object)) return null;
            return NewUserType(_type.BaseType);
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

        public virtual IEnumerable<IPropertyWrapper> GetIndexers(IntermediateBuilder context)
        {
            foreach (var f in _type.GetProperties())
            {
                if(f.GetIndexParameters().Length > 0)
                    yield return new PropertyInfoWrapper(f, context);
            }
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

        public virtual IMethodWrapper GetOperatorOverload(Operator op, IntermediateBuilder context)
        {
            foreach (var w in GetFunctions(context))
                if (w.Name == op.GetOverloadName())
                    return w;

            return null;
        }

        public override void ConvertFrom(CodeValue val, IlBuilder builder)
        {
            throw new NotImplementedException();
        }

        public virtual NamespaceContext GetNamespaceContext() => new NamespaceContext(_type.Namespace);
    }

    class InterUserType : UserType
    {
        private InterType _intertype;

        public InterUserType(InterType type) : base()
        {
            _intertype = type;

            Name = $"class {_intertype.FullName}";
            ShortName = $"{_intertype.FullName}";
        }

        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Methods)
                    yield return new InterMethodWrapper(f, context);
        }

        public override IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Fields)
                yield return new InterFieldWrapper(f);
        }

        public override IEnumerable<IMethodWrapper> GetConstructors(IntermediateBuilder context)
        {
            foreach (var f in _intertype.Constructors)
                yield return new InterMethodWrapper(f, context);
        }

        public override IEnumerable<IPropertyWrapper> GetProperties(IntermediateBuilder context)
        {
            return new List<IPropertyWrapper>();
        }

        public override NamespaceContext GetNamespaceContext() => _intertype.NamespaceContext;

    }

}
