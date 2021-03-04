using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class UserType : CodeType
    {

        private Type _type;

        protected UserType() : base("", -1, "") {}

        public UserType(Type type) : this()
        {
            _type = type;

            Name = $"class [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }

        public virtual IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.GetMethods())
                yield return new MethodInfoWrapper(f, context);
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

        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in _type.Members)
                if (f is InterMethod)
                    yield return new InterMethodWrapper(f as InterMethod, context);
        }
    }

}
