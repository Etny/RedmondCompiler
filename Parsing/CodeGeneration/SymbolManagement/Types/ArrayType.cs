using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class ArrayType : UserType
    {
        public readonly CodeType TypeOf;
        public ArrayType(CodeType type) : base()
        {
            TypeOf = type;
            _type = typeof(Array);
            Name  = TypeOf.Name + "[]";
            ShortName += TypeOf.ShortName + "[]";
        }

        public override bool Equals(object obj)
            => obj is ArrayType array && TypeOf == array.TypeOf;
    }
}
