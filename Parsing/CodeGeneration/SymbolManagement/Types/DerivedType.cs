using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class DerivedType : BasicType
    {
        public readonly BasicType UnderlyingType;

        internal DerivedType(BasicType underlying, params string[] names) : this(underlying.OpName, underlying.Wideness, underlying, names) { }

        internal DerivedType(float wide, BasicType underlying, params string[] names) : this(underlying.OpName, wide, underlying, names) { }
        internal DerivedType(string opName, BasicType underlying, params string[] names) : this(opName, underlying.Wideness, underlying, names) { }


        internal DerivedType(string opName, float wide, BasicType underlying, params string[] names) : base(opName, wide, names)
        {
            UnderlyingType = underlying;
        }

    }
}
