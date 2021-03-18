using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.CodeGeneration.References;
using System.Collections.Immutable;
using System.Diagnostics;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;

namespace Redmond.Parsing.CodeGeneration
{
    class IntermediateBuilder
    {
        public List<InterType> Types = new List<InterType>();

        public InterType CurrentType = null;
        public InterMethod CurrentMethod = null;

        private Stack<SymbolTable> _tables;

        public ImmutableList<IAssemblyReference> AssemblyReferences { get; protected set; } = ImmutableList<IAssemblyReference>.Empty;
        public ImmutableList<string> ImportedNamespaces { get; protected set; } = ImmutableList<string>.Empty;


        private string assemblyName = "Redmond";

        public IntermediateBuilder(Stack<SymbolTable> tables)
        {
            AddReference(new CoreAssemblyReference());  

            var t = Assembly.Load("System.Reflection.Primitives");
            var ts = t.GetType("OpCode");
            var type = typeof(System.Reflection.Emit.OpCode);

            _tables = tables;
        }

        public InterType AddType(InterType type)
        {
            CurrentType = type;
            CurrentMethod = null;
            Types.Add(type);
            return type;
        }

        public InterMethod AddMethod(string name, string returnType, ArgumentSymbol[] vars, List<string> flags)
        {
            var method = new InterMethod(name, returnType, vars, CurrentType, flags);
            CurrentMethod = method;
            CurrentType.AddMethod(method);

            foreach(var arg in vars)
                _tables.Peek().AddSymbol(arg);

            if(method.IsInstance)
                _tables.Peek().AddSymbol(method.ThisPointer);

            return method;
        }

        public InterMethod AddConstructor(ArgumentSymbol[] vars, List<string> flags)
        {
            var method = new InterMethodSpecialName(".ctor", vars, CurrentType, flags);
            CurrentMethod = method;
            CurrentType.AddConstructor(method);

            foreach (var arg in vars)
                _tables.Peek().AddSymbol(arg);

            return method;
        }

        public InterInst AddInstruction(InterInst inst)
        {
            CurrentMethod.AddInstruction(inst);
            return inst;
        }
        public LocalSymbol AddLocal(string name, string type, object value = null)
        {
            LocalSymbol sym = new LocalSymbol(name, type, CurrentMethod.Locals.Count, value);
            CurrentMethod.Locals.Add(sym);

            _tables.Peek().AddSymbol(sym);

            return sym;
        }

        public InterField AddField(string name, string type, string access, List<string> keywords)
        {
            InterField field = new InterField(name, type, access, keywords, new InterUserType(CurrentType));
            _tables.Peek().AddSymbol(field.Symbol);
            CurrentType.AddField(field);
            return field;
        }

        public void AddImport(string nameSpace)
            => ImportedNamespaces = ImportedNamespaces.Add(nameSpace);

        public void AddReference(IAssemblyReference reference)
            => AssemblyReferences = AssemblyReferences.Add(reference);

        //TODO: Improve this
        public CodeType ResolveType(string name)
        {
            var ctype = CodeType.ByName(name);
            if (ctype != null) return ctype;

            foreach(InterType type in Types)
            {
                if (type.FullName == name) return new InterUserType(type);
            }

            foreach (string ns in ImportedNamespaces)
            {
                foreach (var a in AssemblyReferences)
                {
                    var type = a.ResolveType(ns + "." + name);
                    if (type == null) type = a.ResolveType(name);
                    if(type != null) return new UserType(type);
                }
            }

            return null;
        }

        public CodeType ResolveType(Type type)
        {
            if (CodeType.ByName(type.Name.ToLower()) != null) return CodeType.ByName(type.Name.ToLower());

            return new UserType(type);
        }


        public IMethodWrapper FindClosestFunction(string name, CodeType owner, params CodeValue[] args)
        {
            var type = owner as UserType;
            Debug.Assert(owner is UserType);

            List<IMethodWrapper> applicableFunctions = new List<IMethodWrapper>();

            foreach (var f in type.GetFunctions(this))
                if (f.Name == name && f.ArgumentCount == args.Length)
                    applicableFunctions.Add(f);

            Debug.Assert(applicableFunctions.Count > 0);

            IMethodWrapper closest = null;
            float lowestDifference = -1;

            foreach(var f in applicableFunctions)
            {
                bool canConvert = true;
                float diff = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    var argType = args[i].Type;
                    var funcType = f.Arguments[i];

                    if (argType != funcType && (argType.GetWiderType(funcType) == null || argType.GetWiderType(funcType) == argType)) { canConvert = false; break; }

                    if (funcType != argType) diff++;
                }

                if (!canConvert) continue;

                if(lowestDifference < 0 || diff < lowestDifference)
                {
                    lowestDifference = diff;
                    closest = f;
                }
            }

            Debug.Assert(closest != null);

            return closest;
        }

        public IMethodWrapper FindMostApplicableConstructor(UserType type, params CodeValue[] args)
        {
            //TODO: Merge this part with FindClosestFunction
            IMethodWrapper closest = null;
            float lowestDifference = -1;

            foreach (var f in type.GetConstructors(this))
            {
                if (f.ArgumentCount != args.Length) continue;

                bool canConvert = true;
                float diff = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    var argType = args[i].Type;
                    var funcType = f.Arguments[i];

                    if (argType != funcType && (argType.GetWiderType(funcType) == null || argType.GetWiderType(funcType) == argType)) { canConvert = false; break; }

                    if (funcType != argType) diff++;
                }

                if (!canConvert) continue;

                if (lowestDifference < 0 || diff < lowestDifference)
                {
                    lowestDifference = diff;
                    closest = f;
                }
            }

            Debug.Assert(closest != null);

            return closest;
        }
        public void Emit(IlBuilder builder)
        {
            foreach (InterType t in Types)
                t.Bind(this);

            EmitCLRHeader(builder);

            foreach (InterType t in Types)
                t.Emit(builder);
        }

        private void EmitCLRHeader(IlBuilder builder)
        {
            builder.EmitLine($".assembly {assemblyName} {{}}");

            builder.EmitLine();

            foreach(var reference in AssemblyReferences)
                builder.EmitLine($".assembly extern {reference.Name} {{}}");

            builder.EmitLine(".module TestModule");

            builder.EmitLine();


        }

    }
}
