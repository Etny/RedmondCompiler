using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethodSpecialName : InterMethod
    {

        public InterMethodSpecialName(string name, ArgumentSymbol[] args, InterType owner, List<string> flags) : base(name, new TypeName("void"), args, owner, flags)
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
                    AddInstruction(new InterCall(new ConstructorInfoWrapper(typeof(object).GetConstructor(new Type[0]), builder), false, ThisPointer), 0);

                foreach (var field in Owner.Fields)
                {
                    if (field.IsStatic) continue;
                    AddInstruction(new InterCopy(field.Symbol, field.Initializer), 0);
                }
            }
            else if (Name == ".cctor") ;
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
