using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class StringType : CodeType
    {
        private static Dictionary<CodeType, IMethodWrapper> convCalls = new Dictionary<CodeType, IMethodWrapper>();

        public StringType() : base("string") { }

        public override void Convert(CodeValue val, IlBuilder builder)
        {
            var type = UserType.ToUserType(val.Type);

            Debug.Assert(type != null);

            //TODO: Add non-symbol support

            var call = new InterCall(convCalls[val.Type], true, val);
            call.Emit(builder);
            //Don't shrink stack, value is popped and then string is pushed
        }

        public override void BindConversion(IntermediateBuilder context, CodeValue from)
        {
            if (convCalls.ContainsKey(from.Type)) return;

            UserType user = UserType.ToUserType(from.Type);
            
            IMethodWrapper match = null;

            foreach(var f in user.GetFunctions(context))
                if(f.Name == "ToString") { match = f; break; }

            Debug.Assert(match != null);

            convCalls.Add(from.Type, match);
        }


        public override OpCode GetPushCode() => OpCodes.Ldstr;
        public override CodeType GetWiderType(CodeType otherType)
        {
            return this;
        }
    }

    class VoidType : CodeType
    {
        public VoidType() : base("void") { }

        public override void Convert(CodeValue val, IlBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override CodeType GetWiderType(CodeType otherType)
        {
            throw new NotImplementedException();
        }
    }
}
