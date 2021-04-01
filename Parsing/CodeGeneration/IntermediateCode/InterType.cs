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
        public ImmutableList<InterMethod> Methods = ImmutableList<InterMethod>.Empty;
        public ImmutableList<InterField> Fields = ImmutableList<InterField>.Empty;


        public readonly string Name;
        public readonly UserType BaseType;

        public readonly InterMethodSpecialName Initializer;
        public readonly List<InterMethodSpecialName> Constructors = new List<InterMethodSpecialName>();


        //TODO: add interface support
        public InterType(string name, UserType baseType = null)
        {
            Name = name;
            BaseType = baseType ?? (UserType)CodeType.Object;

            //Constructor = new InterMethodSpecialName(".ctor", new CodeSymbol[] { }, this);
            Initializer = new InterMethodSpecialName(".cctor", new ArgumentSymbol[] { }, this, new List<string>());
        }

        public string FullName => Name;

        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);


        public void AddMethod(InterMethod method)
            => Methods = Methods.Add(method);

        public void AddConstructor(InterMethodSpecialName method)
            => Constructors.Add(method);


        public void AddField(InterField field)
            => Fields = Fields.Add(field);

        public void Emit(IlBuilder builder)
        {
            builder.EmitLine($".class private auto ansi beforefieldinit {Name} extends {BaseType.ShortName}");
            builder.EmitLine("{");
            builder.EmitLine();
            builder.Output.AddIndentation();

            foreach (var field in Fields)
                field.Emit(builder);

            builder.EmitLine();

            Initializer.Emit(builder);

            foreach (var c in Constructors)
                c.Emit(builder);

            foreach (var method in Methods)
                method.Emit(builder);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
            builder.EmitLine("");
        }

        public void Bind(IntermediateBuilder builder)
        {
            foreach (var field in Fields)
                field.Bind(builder);

            Initializer.Bind(builder);

            foreach (var constructor in Constructors)
                constructor.Bind(builder);

            foreach (var method in Methods)
                method.Bind(builder);

           
        }

        public void BindSubMembers(IntermediateBuilder builder)
        {
            Initializer.BindSubMembers(builder);

            foreach (var constructor in Constructors)
                constructor.BindSubMembers(builder);

            foreach (var method in Methods)
                method.BindSubMembers(builder);
        }
    }
}
