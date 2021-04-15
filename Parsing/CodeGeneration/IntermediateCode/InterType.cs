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

        public readonly ResolutionContext NamespaceContext;
        public readonly string Name;
        public CodeType BaseType;

        public readonly TypeName BaseTypeName;

        public readonly InterInitializer Initializer;
        public readonly List<InterConstructor> Constructors = new List<InterConstructor>();

        public readonly int GenericParametersCount;

        //TODO: add interface support
        public InterType(string name, ResolutionContext names, TypeName baseTypeName, int genericParamCount = 0)
        {
            Name = name;
            NamespaceContext = names;
            BaseTypeName = baseTypeName;
            GenericParametersCount = genericParamCount;

            //Constructor = new InterMethodSpecialName(".ctor", new CodeSymbol[] { }, this);
            Initializer = new InterInitializer(this, new List<string>());
        }

        public string FullName => string.Join('.', NamespaceContext.NamespaceHierarchy.Add(Name)) 
                                    + (GenericParametersCount == 0 ? "" : "`" + GenericParametersCount);

        public bool IsAbstract => Flags.Contains("abstract");

        public bool IsGeneric => GenericParametersCount > 0;

        public void AddFlag(string flag)
        {
            if(flag == "static")
            {
                AddFlag("sealed");
                AddFlag("abstract");
                return;
            }

            if (Flags.Contains(flag)) return;
            Flags = Flags.Add(flag);
        }


        public void AddMethod(InterMethod method)
            => Methods = Methods.Add(method);

        public void AddConstructor(InterConstructor method)
            => Constructors.Add(method);


        public void AddField(InterField field)
            => Fields = Fields.Add(field);

        public void Emit(IlBuilder builder)
        {
            string genericParams =
                IsGeneric ? '<' + string.Join(',', NamespaceContext.GenericParameters) + '>' : "";

            builder.EmitLine($".class private {string.Join(' ', Flags)} auto ansi beforefieldinit {FullName}{genericParams} extends {BaseType.ShortName}");
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
            BaseType = builder.ResolveType(BaseTypeName);
            if (Constructors.Count == 0) Constructors.Add(new InterConstructor(this));

            
           
        }

        public void BindSubMembers(IntermediateBuilder builder)
        {
            foreach (var field in Fields)
                field.Bind(builder);

            Initializer.Bind(builder);

            foreach (var constructor in Constructors)
                constructor.Bind(builder);

            foreach (var method in Methods)
                method.Bind(builder);
        }

            public void BindSubSubMembers(IntermediateBuilder builder)
        {
            Initializer.BindSubMembers(builder);

            foreach (var constructor in Constructors)
                constructor.BindSubMembers(builder);

            foreach (var method in Methods)
                method.BindSubMembers(builder);
        }
    }
}
