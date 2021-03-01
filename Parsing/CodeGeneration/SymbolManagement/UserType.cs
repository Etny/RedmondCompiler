using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class UserType : CodeType
    {

        private Type _type;

        public UserType(Type type) : base("", -1, "")
        {
            _type = type;

            Name = $"class [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }

    }
}
