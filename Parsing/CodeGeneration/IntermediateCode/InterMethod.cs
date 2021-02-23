using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethod : IInterMember
    {
        public ImmutableList<string> Flags = ImmutableList<string>.Empty;

        public ImmutableList<InterInst> Instructions = ImmutableList<InterInst>.Empty;

        public readonly string Name, ReturnType;
        public readonly InterType Owner;

        public InterMethod(string name, string returnType, InterType owner)
        {
            Name = name;
            ReturnType = returnType;
            Owner = owner;
        }

        public string Signature => Owner.FullName + "." + Name;
        public bool IsInstance => !Flags.Contains("static");
        public bool IsVirtual => Flags.Contains("virtual") || Flags.Contains("override");

        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);

        public void AddInstruction(InterInst inst)
            => Instructions = Instructions.Add(inst);

        public void Emit(IlBuilder builder, IntermediateBuilder context)
        {
            builder.EmitString(".method");
            foreach (string flag in Flags) builder.EmitString(" " + flag);
            builder.EmitLine($" {ReturnType} {Name}");
            builder.EmitLine("{");
            builder.Output.AddIndentation();

            foreach (var inst in Instructions)
                inst.Emit(builder, context);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
            builder.EmitLine("");
        }
    }
}
