using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterInitializer : InterMethodSpecialName
    {

        public InterInitializer(InterType owner, List<string> flags) : base(".cctor", new ArgumentSymbol[] { }, owner, flags)
        {
            AddFlag("static");

            ReturnType = new InterUserType(owner);
        }

        public override void Emit(IlBuilder builder)
        {
            if (Instructions.Count == 0) return;
            base.Emit(builder);
        }

        public override void BindSubMembers(IntermediateBuilder builder)
        {
            foreach (var field in Owner.Fields)
            {
                if (!field.IsStatic) continue;
                AddInstruction(new InterCopy(field.Symbol, field.Initializer));
            }

            base.BindSubMembers(builder);
        }


    }
}
