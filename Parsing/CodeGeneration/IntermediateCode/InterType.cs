using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterType
    {
        public ImmutableList<string> Flags = ImmutableList<string>.Empty;
        public ImmutableList<IInterMember> Members = ImmutableList<IInterMember>.Empty;

        public readonly string Name, BaseType;

        //TODO: add interface support
        public InterType(string name, string baseType = "[System.Runtime]System.Object")
        {
            Name = name;
            BaseType = baseType;
        }

        public string FullName => Name;

        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);


        public void AddMember(IInterMember member)
            => Members = Members.Add(member);

        public void Emit(IlBuilder builder)
        {
            builder.EmitLine($".class private auto ansi beforefieldinit {Name} extends {BaseType}");
            builder.EmitLine("{");
            builder.EmitLine("");
            builder.Output.AddIndentation();

            foreach (var member in Members)
                member.Emit(builder);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
            builder.EmitLine("");
        }

        public void Bind(IntermediateBuilder builder)
        {
            foreach (var member in Members)
                member.Bind(builder);
        }
    }
}
