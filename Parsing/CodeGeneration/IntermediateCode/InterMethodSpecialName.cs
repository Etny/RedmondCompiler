using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethodSpecialName : InterMethod
    {

        public InterMethodSpecialName(string name, ArgumentSymbol[] args, InterType owner, List<string> flags) : base(name, "void", args, owner, flags)
        {
            AddFlag("rtspecialname");
            AddFlag("specialname");

            if (name == ".ctor")
            {
                AddFlag("instance");
            }
            else if (name == ".cctor")
                AddFlag("static");
        }

        public override void Bind(IntermediateBuilder builder)
        {
            base.Bind(builder);

            if (Name == ".ctor")
            {
                if (Owner.BaseType == CodeType.Object)
                    AddInstruction(new InterCall(new ConstructorInfoWrapper(typeof(object).GetConstructor(new Type[0]), builder), new CodeValue[0], false, ThisPointer), 0);
            }
        }

    }
}
