using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class BoxedType : UserType
    {

        public BasicType BaseType { get; protected set; }

        public BoxedType(Type type) : base(type) 
        { 
            _specialCases.Add(type, this);
        }

        public void SetBaseType(BasicType type)
        {
            BaseType = type;
            StoredType = type;
        }

        public override bool Equals(object obj)
            => (obj is BasicType basic && basic == BaseType) || base.Equals(obj);

        public override AssignType CanAssignTo(CodeType fieldType)
        {
            if (BaseType.Equals(fieldType)) return AssignType.CanAssign;
              
            return base.CanAssignTo(fieldType);
        }

        public override void ConvertTo(CodeValue val, IlBuilder builder, CodeType to)
        {
            if (to == BaseType)
            {
                builder.PushValue(val);
                builder.EmitOpCode(OpCodes.Unbox_Any, Name);
            }
            else
                base.ConvertTo(val, builder, to);
        }
    }
}
