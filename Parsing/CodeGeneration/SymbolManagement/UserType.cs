using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class UserType : CodeType
    {

        private Type _type;
        protected bool _valuetype;

        protected UserType() : base("", -1, "") {}

        public UserType(Type type) : this()
        {
            _type = type;

            _valuetype = type.IsValueType;

            Name = $"{(_valuetype ? "valuetype" : "class")} [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }

        public virtual string SpecName => $"[{_type.Module.Assembly.GetName().Name}]{_type.FullName}";

        public virtual IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.GetMethods())
                yield return new MethodInfoWrapper(f, context);
        }

        public virtual IEnumerable<IFieldWrapper> GetFields(IntermediateBuilder context)
        {
            foreach (var f in _type.GetFields())
                yield return new FieldInfoWrapper(f, context);
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
    }

}
