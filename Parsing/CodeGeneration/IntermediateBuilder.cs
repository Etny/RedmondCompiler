﻿using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.CodeGeneration.References;
using System.Collections.Immutable;
using System.Diagnostics;

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

        public InterMethod AddMethod(string name, string returnType, CodeSymbol[] vars)
        {
            var method = new InterMethod(name, returnType, vars, CurrentType);
            CurrentMethod = method;
            CurrentType.AddMethod(method);

            for (int i = 0; i < vars.Length; i++)
            {
                vars[i].Location = new IndexedSymbolLocation(IndexedSymbolLocation.IndexedSymbolLocationType.Argument, i);
                _tables.Peek().AddSymbol(vars[i]);
            }

            return method;
        }

        public InterInst AddInstruction(InterInst inst)
        {
            CurrentMethod.AddInstruction(inst);
            return inst;
        }
        public CodeSymbol AddLocalSymbol(string name, string type, object value = null)
        {
            CodeSymbol sym = new CodeSymbol(name, type, new IndexedSymbolLocation(IndexedSymbolLocation.IndexedSymbolLocationType.Local, CurrentMethod.Locals.Count), value);
            CurrentMethod.Locals.Add(sym);

            _tables.Peek().AddSymbol(sym);

            return sym;
        }

        public InterField AddField(string name, string type)
        {
            InterField field = new InterField(name, type, new InterUserType(CurrentType));
            _tables.Peek().AddSymbol(field.Symbol);
            CurrentType.AddField(field);
            return field;
        }

        public void AddImport(string nameSpace)
            => ImportedNamespaces = ImportedNamespaces.Add(nameSpace);

        public void AddReference(IAssemblyReference reference)
            => AssemblyReferences = AssemblyReferences.Add(reference);

        public CodeType ResolveType(string name)
        {
            var ctype = CodeType.ByName(name);
            if (ctype != null) return ctype;

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


        public IMethodWrapper FindClosestFunction(string name, CodeType owner, params CodeType[] args)
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
                    var argType = args[i];
                    var funcType = f.Arguments[i];

                    if (argType != funcType && (argType.GetWiderType(funcType) == null || argType.GetWiderType(funcType) == argType)) { canConvert = false; break; }

                    diff += funcType.Wideness - argType.Wideness;
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
