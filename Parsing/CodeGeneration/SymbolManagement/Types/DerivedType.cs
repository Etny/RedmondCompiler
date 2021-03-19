using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class DerivedType : BasicType
    {
        public readonly BasicType UnderlyingType;

        internal DerivedType(float wide, BasicType underlying, BoxedType boxed, params string[] names) : this(underlying.OpName, wide, underlying, boxed, names) { }

        internal DerivedType(string opName, float wide, BasicType underlying, BoxedType boxed, params string[] names) : base(opName, wide, boxed, names)
        {
            UnderlyingType = underlying;
        }

    }
}
