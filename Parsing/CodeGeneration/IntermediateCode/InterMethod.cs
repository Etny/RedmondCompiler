using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;
using System.Reflection.Emit;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Output;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class InterMethod : IInterMember
    {
        public ImmutableList<string> Flags = ImmutableList<string>.Empty;

        public ImmutableList<InterInst> Instructions = ImmutableList<InterInst>.Empty;

        public readonly string Name, ReturnTypeName;
        public CodeType ReturnType;
        public readonly ArgumentSymbol ThisPointer = null;
        public readonly ArgumentSymbol[] Arguments;
        public readonly InterType Owner;

        public List<CodeSymbol> Locals = new List<CodeSymbol>();
        public int Args => Arguments.Length;

        public InterMethod(string name, string returnTypeName, ArgumentSymbol[] args, InterType owner, List<string> flags)
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

        public void AddFlag(string flag)
            => Flags = Flags.Add(flag);

        public void AddInstruction(InterInst inst)
        {
            Instructions = Instructions.Add(inst);
            inst.SetOwner(this);
        }

        public virtual void Emit(IlBuilder builder)
        {
            builder.EmitString(".method");
            foreach (string flag in Flags) builder.EmitString(" " + flag);
            builder.EmitLine($" {ReturnType.Name} {Name}({ArgList}) cil managed");
            builder.EmitLine("{");

            builder.Output.AddIndentation();

            //TODO: Allow for selection of entrypoint
            if (Name == "Main") builder.EmitLine(".entrypoint");

            int startLoc = builder.Output.ReserveLocation();


            if(Locals.Count > 0)
            {
                string types = "";
                foreach (var t in Locals) types += t.Type.Name + ",";
                builder.EmitLine(".locals init (" + types[..^1] + ")");
            }

            foreach (var inst in Instructions)
                inst.Emit(builder);

            if(Instructions.Count == 0 || !(Instructions[^1] is InterRet)) builder.EmitOpCode(OpCodes.Ret);
            builder.Output.WriteStringAtLocation(startLoc, ".maxstack " + builder.GetMaxStack());
            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
            builder.EmitLine("");
        }

        public void Bind(IntermediateBuilder builder)
        {
            ReturnType = builder.ResolveType(ReturnTypeName);

            foreach (var a in Arguments)
                a.BindType(builder);

            foreach (var l in Locals)
                l.BindType(builder);
        }

        public void BindSubMembers(IntermediateBuilder builder)
        {
            foreach (var inst in Instructions)
                inst.Bind(builder);
        }
        
    }
}
