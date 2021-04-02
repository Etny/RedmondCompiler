using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.SymbolManagement
{
    class StringType : UserType
    {
        private static Dictionary<CodeType, IMethodWrapper> convCalls = new Dictionary<CodeType, IMethodWrapper>();

        public StringType() : base("string") { _type = typeof(string); _specialCases.Add(typeof(string), this); }
        public override void ConvertFrom(CodeValue val, IlBuilder builder)
        {
            var type = UserType.ToUserType(val.Type);

            Debug.Assert(type != null);

            //TODO: Add non-symbol support

            var call = new InterCall(convCalls[val.Type], true, val);
            call.Emit(builder);
            //Don't shrink stack, value is popped and then string is pushed
        }

        public override IMethodWrapper GetConversionMethod(IntermediateBuilder context, CodeValue from)
        {
            UserType user = UserType.ToUserType(from.Type);

            IMethodWrapper match = null;

            foreach (var f in user.GetFunctions(context))
                if (f.Name == "ToString") { match = f; break; }

            Debug.Assert(match != null);

            return match;
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
        public override IMethodWrapper GetOperatorOverload(Operator op, IntermediateBuilder context)
        {
            if (op.Type == Operator.OperatorType.Add)
                return new MethodInfoWrapper(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }), context);
            else
                return base.GetOperatorOverload(op, context);
        }


        public override IEnumerable<IMethodWrapper> GetFunctions(IntermediateBuilder context)
        {
            foreach (var f in typeof(string).GetMethods())
                yield return new MethodInfoWrapper(f, context);
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

        public override AssignType CanAssignTo(CodeType fieldType) => AssignType.CannotAssign;

        public override void ConvertFrom(CodeValue val, IlBuilder builder)
        {
            throw new NotImplementedException();
        }

        public override CodeType GetWiderType(CodeType otherType) => otherType;
    }

    class ObjectType : UserType
    {
        public ObjectType() : base("object") 
        {
            _type = typeof(object); 
            _specialCases.Add(_type, this);

           //Name = $"{(_valuetype ? "valuetype" : "class")} [{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
            ShortName = $"[{_type.Module.Assembly.GetName().Name}]{_type.FullName}";
        }
    }
}
