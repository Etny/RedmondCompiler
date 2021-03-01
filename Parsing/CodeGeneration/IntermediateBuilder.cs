using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Redmond.Parsing.CodeGeneration.SymbolManagement;

namespace Redmond.Parsing.CodeGeneration
{
    class IntermediateBuilder
    {
        public List<InterType> Types = new List<InterType>();

        public InterType CurrentType = null;
        public InterMethod CurrentMethod = null;

        private Dictionary<string, InterMethod> _localFunctions = new Dictionary<string, InterMethod>();
        private List<Assembly> imports = new List<Assembly>();
        private Stack<SymbolTable> _tables;

        private string assemblyName = "Redmond";

        public IntermediateBuilder(Stack<SymbolTable> tables)
        {
            AddImport(Assembly.Load("System.Runtime"));

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
            CurrentType.AddMember(method);
            _localFunctions.Add(method.Signature, method);

            for (int i = 0; i < vars.Length; i++)
            {
                vars[i].Location = new CodeSymbolLocation(CodeSymbolLocationType.Argument, i);
                _tables.Peek().AddSymbol(vars[i]);
            }

            return method;
        }

        public InterInst AddInstruction(InterInst inst)
        {
            CurrentMethod.AddInstruction(inst);
            inst.SetOwner(CurrentMethod);
            return inst;
        }

        //TODO: Add field support

        public CodeSymbol AddLocalSymbol(string name, string type, object value = null)
        {
            CodeSymbol sym = new CodeSymbol(name, type, new CodeSymbolLocation(CodeSymbolLocationType.Local, CurrentMethod.Locals.Count), value);
            CurrentMethod.Locals.Add(sym);

            _tables.Peek().AddSymbol(sym);

            return sym;
        }

        public void AddImport(Assembly assembly)
            => imports.Add(assembly);

        public CodeType ResolveType(string name)
        {
            var ctype = CodeType.ByName(name);
            if (ctype != null) return ctype;

            foreach(var a in imports)
            {
                var type = a.GetType(name);
                if (type != null) return new UserType(type);
            }

            return null;
        }

        public CodeType ResolveType(Type type)
        {
            if (CodeType.ByName(type.Name.ToLower()) != null) return CodeType.ByName(type.Name.ToLower());

            return new UserType(type);
        }

        public InterMethod FromSignature(string sig)
            => _localFunctions[sig];

        public IMethodWrapper FindClosestFunction(string name, InterType owner, params CodeType[] args)
        {
            if (_localFunctions.ContainsKey(owner.FullName + "." + name))
                return new InterMethodWrapper(_localFunctions[owner.FullName + "." + name], this);

            //TODO: improve this
            if (name.Contains(".")) {
                string typeName = name[0..name.LastIndexOf('.')];
                string funcName = name[(name.LastIndexOf('.')+1)..];

                foreach (var a in imports)
                    foreach (Type t in (from type in a.GetTypes() where type.Name == typeName select type).ToArray())
                        foreach(var m in t.GetMethods())
                            if (m.Name == funcName)
                                return new MethodInfoWrapper(m, this);
            }
            else
            {
                string funcName = name[(name.LastIndexOf('.')+1)..name.LastIndexOf('(')];

                foreach (var a in imports)
                    foreach (Type t in a.GetTypes())
                        foreach (var m in t.GetMethods())
                            if (m.Name == funcName)
                                return new MethodInfoWrapper(m, this);
            }

            return null;
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

            foreach(var import in imports)
                builder.EmitLine($".assembly extern {import.ManifestModule.Name.Replace(".dll", "")} {{}}");

            builder.EmitLine(".module TestModule");

            builder.EmitLine();


        }

    }
}
