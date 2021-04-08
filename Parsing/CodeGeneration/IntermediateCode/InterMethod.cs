using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;
using System.Reflection.Emit;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.IO;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethod : IInterMember
    {
        public ImmutableList<string> Flags = ImmutableList<string>.Empty;

        public ImmutableList<InterInst> Instructions = ImmutableList<InterInst>.Empty;

        public readonly string Name;
        public readonly TypeName ReturnTypeName;
        public CodeType ReturnType;
        public readonly ArgumentSymbol ThisPointer = null;
        public readonly ArgumentSymbol[] Arguments;
        public readonly InterType Owner;

        public List<CodeSymbol> Locals = new List<CodeSymbol>();
        public int Args => Arguments.Length;

        public LabelManager LabelManager = new LabelManager();
        public string NextLabel { get { LabelManager.LabelNext = true; return LabelManager.CurrentLabel; } }

        public InterMethod(string name, TypeName returnTypeName, ArgumentSymbol[] args, InterType owner, List<string> flags)
        {
            Name = name;
            ReturnTypeName = returnTypeName;
            Arguments = args;
            Owner = owner;

            foreach (string flag in flags) AddFlag(flag);

            if (IsInstance)
            {
                foreach (var arg in args) arg.Index++;
                ThisPointer = new ArgumentSymbol("this", new InterUserType(owner), 0);
            }
        }


        private string ArgList => string.Join(',', from a in Arguments select a.Type.Name);


        public string Signature => $"{Owner.FullName}.{Name}";

        public string CallSignature => $"{(IsInstance ? "instance " : "")}{ReturnType.Name} {Owner.FullName}::{Name}({ArgList})";
        public bool IsInstance => !IsStatic;
        public bool IsVirtual => Flags.Contains("virtual") || Flags.Contains("override");

        public bool IsStatic => Flags.Contains("static");
        public bool IsAbstract => Flags.Contains("abstract");

        public void AddFlag(string flag)
        { 
            //if(Name[0] != '.')
            //{
            //    if (flag == "abstract") AddFlag("newlot");
            //}

            if (Flags.Contains(flag)) return;
            Flags = Flags.Add(flag);
        }

        public void AddInstruction(InterInst inst)
        {
            Instructions = Instructions.Add(inst);
            inst.SetOwner(this);

            if (!LabelManager.LabelNext) return;
            inst.Label = LabelManager.NextLabel;
        }

        public void AddInstruction(InterInst inst, int index)
        {
            Instructions = Instructions.Insert(index, inst);
            inst.SetOwner(this);

            if (!LabelManager.LabelNext) return;
            inst.Label = LabelManager.NextLabel;
        }

        public virtual void Emit(IlBuilder builder)
        {
            builder.EmitString(".method");
            foreach (string flag in Flags) builder.EmitString(" " + flag);
            builder.EmitLine($" {ReturnType.Name} {Name}({ArgList}) cil managed");
            builder.EmitLine("{");

            if (!IsAbstract && Instructions.Count > 0)
            {
                builder.Output.AddIndentation();

                //TODO: Allow for selection of entrypoint
                if (Name == "Main") builder.EmitLine(".entrypoint");

                int startLoc = builder.Output.ReserveLocation();

                if (Locals.Count > 0)
                {
                    string types = "";
                    foreach (var t in Locals) types += t.Type.Name + ",";
                    builder.EmitLine(".locals init (" + types[..^1] + ")");
                }

                foreach (var inst in Instructions)
                    inst.Emit(builder);

                builder.Output.WriteStringAtLocation(startLoc, ".maxstack " + builder.GetMaxStack());
                builder.Output.ReduceIndentation();
            }

            builder.EmitLine("}");
            builder.EmitLine("");
        }

        public virtual void Bind(IntermediateBuilder builder)
        {
            ReturnType = builder.ResolveType(ReturnTypeName);

            if (IsVirtual || IsAbstract)
            {
                var inBase = builder.FindClosestFunction(Name, Owner.BaseType, Arguments, true);
                if (inBase == null) AddFlag("newslot");
            }


            if (!IsStatic) AddFlag("instance");

            foreach (var a in Arguments)
                a.Bind(builder);

            foreach (var l in Locals)
                l.Bind(builder);
        }

        public virtual void BindSubMembers(IntermediateBuilder builder)
        {
            if (Instructions.Count > 0 && !(Instructions[^1] is InterRet)) AddInstruction(new InterRet());

            foreach (var inst in Instructions)
                inst.Bind(builder);
        }

    }
}
