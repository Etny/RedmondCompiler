using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethodSpecialName : InterMethod
    {

        public InterMethodSpecialName(string name, ArgumentSymbol[] args, InterType owner, List<string> flags) : base(name, new TypeName("void"), args, owner, flags)
        {
            

            if (name == ".cctor")
            {
                AddFlag("static");
                AddFlag("rtspecialname");
                AddFlag("specialname");


                ReturnType = new InterUserType(owner);
            }
        }


        public override void Emit(IlBuilder builder)
        {
            if (Name == ".cctor" && Instructions.Count == 0) return;
            base.Emit(builder);
        }

        public override void Bind(IntermediateBuilder builder)
        {
            base.Bind(builder);
            if (Name == ".cctor") 
            {
                foreach (var field in Owner.Fields)
                {
                    if (!field.IsStatic) continue;
                    AddInstruction(new InterCopy(field.Symbol, field.Initializer));
                }
            }
        }

    }
}
