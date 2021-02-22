using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethod : IInterMember
    {
        public ImmutableList<string> Flags = ImmutableList<string>.Empty;

        public ImmutableList<IInterInst> Instructions = ImmutableList<IInterInst>.Empty;

        public readonly string Name, ReturnType;

        public InterMethod(string name, string returnType)
        {
            Name = name;
            ReturnType = returnType;
        }


        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);

        public void AddInstruction(IInterInst inst)
            => Instructions = Instructions.Add(inst);

        public void Emit(IlBuilder builder)
        {
            builder.EmitString(".method");
            foreach (string flag in Flags) builder.EmitString(" " + flag);
            builder.EmitLine($" {ReturnType} {Name}");
            builder.EmitLine("{");
            builder.Output.AddIndentation();

            foreach (var inst in Instructions)
                inst.Emit(builder);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
        }
    }
}
