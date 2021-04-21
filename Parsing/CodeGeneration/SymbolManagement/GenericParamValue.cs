using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class GenericParamValue : CodeValue
    {
        private CodeValue _owner;
        private int _index;

        public GenericParamValue(CodeValue owner, int index = 0)
        {
            _owner = owner;
            _index = index;
        }

        public override void Bind(IntermediateBuilder context)
        {
            _owner.Bind(context);

            if (Type != null) return;

            CodeType gen = _owner.Type;

            if (gen is ArrayType)
                Type = (gen as ArrayType).TypeOf;
            else if (gen is IGenericType)
                Type = (gen as IGenericType).GetGenericParameters()[_index];
            else
                Type = CodeType.Void;
        }
    }
}
