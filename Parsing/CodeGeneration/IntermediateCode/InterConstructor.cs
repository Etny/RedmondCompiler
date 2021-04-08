using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterConstructor : InterMethodSpecialName
    {


        private readonly bool _callThis;
        private CodeValue[] _baseArgs = new CodeValue[0];

        public InterConstructor(InterType owner) : this(new ArgumentSymbol[] { }, owner, new List<string>(), false) { }

        public InterConstructor(ArgumentSymbol[] args, InterType owner, List<string> flags, bool callThis) : base(".ctor", args, owner, flags)
        {
            AddFlag("rtspecialname");
            AddFlag("specialname");
            AddFlag("instance");

            _callThis = callThis;

            ReturnType = new InterUserType(owner);
        }

        public void SetBaseArgs(CodeValue[] args)
            => _baseArgs = args;

        public override void Bind(IntermediateBuilder builder)
        {
            base.Bind(builder);
        }

        public override void BindSubMembers(IntermediateBuilder builder)
        {

            foreach (var a in _baseArgs) a.Bind(builder);

            IMethodWrapper baseCon
                = builder.FindMostApplicableConstructor(_callThis ? new InterUserType(Owner) : UserType.ToUserType(Owner.BaseType), _baseArgs);


            AddInstruction(new InterCall(baseCon, _baseArgs, false, ThisPointer), 0);


            foreach (var field in Owner.Fields)
            {
                if (field.IsStatic) continue;
                AddInstruction(new InterCopy(field.Symbol, field.Initializer), 0);
            }

            base.BindSubMembers(builder);
        }

    }
}
