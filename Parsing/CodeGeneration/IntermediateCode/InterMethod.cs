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
        public readonly string[] VarTypes;
        public readonly InterType Owner;

        public int Locals = 0;
        public int Args => VarTypes.Length;

        public InterMethod(string name, string returnType, string[] varTypes, InterType owner)
        {
            Name = name;
            ReturnType = returnType;
            VarTypes = varTypes;
            Owner = owner;
        }

        public string Signature => Owner.FullName + "." + Name + "(" + string.Join(',', VarTypes) + ")";
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
