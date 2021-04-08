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
            TypeOf = type.StoredType;
            _type = typeof(Array);
            Name  = TypeOf.Name + "[]";
            ShortName += TypeOf.ShortName + "[]";
        }

        public override bool Equals(object obj)
            => obj is ArrayType array && TypeOf == array.TypeOf;

        public override AssignType CanAssignTo(CodeType fieldType)
        {
            var other = fieldType as ArrayType;
            if (other == null) return AssignType.CannotAssign;

            if (Equals(other)) return AssignType.CanAssign;
            return TypeOf.CanAssignTo(other.TypeOf) == AssignType.CanAssign ? AssignType.CanAssign : AssignType.CannotAssign; //CanConvert does NOT work for arrays!
        }
    }
}
