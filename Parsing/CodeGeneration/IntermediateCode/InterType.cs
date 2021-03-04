using Redmond.Parsing.CodeGeneration.SymbolManagement;
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

        public readonly string Name;
        public readonly CodeType BaseType;

        //TODO: add interface support
        public InterType(string name, CodeType baseType = null)
        {
            Name = name;
            BaseType = baseType ?? CodeType.Object;
        }

        public string FullName => Name;

        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);


        public void AddMember(IInterMember member)
            => Members = Members.Add(member);

        public void Emit(IlBuilder builder)
        {
            builder.EmitLine($".class private auto ansi beforefieldinit {Name} extends {BaseType.Name}");
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
