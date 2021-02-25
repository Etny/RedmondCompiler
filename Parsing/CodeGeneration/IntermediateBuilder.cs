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

        public IntermediateBuilder()
        {
            
        }

        public InterType AddType(InterType type)
        {
            CurrentType = type;
            CurrentMethod = null;
            Types.Add(type);
            return type;
        }

        public InterMethod AddMethod(string name, string returnType, string[] vars)
        {
            var method = new InterMethod(name, returnType, vars, CurrentType);
            CurrentMethod = method;
            CurrentType.AddMember(method);
            _localFunctions.Add(method.Signature, method);
            return method;
        }

        public InterInst AddInstruction(InterInst inst)
        {
            CurrentMethod.AddInstruction(inst);
            inst.SetOwner(CurrentMethod);
            return inst;
        }

        //TODO: Add field support
        public CodeSymbol AddSymbol(SymbolTable table, string name, string type, object value = null)
        {
            CodeSymbol sym = new CodeSymbol(name, type, new CodeSymbolLocation(CodeSymbolLocationType.Local, CurrentMethod.Locals++), value);

            table.AddSymbol(sym);

            return sym;
        }

        public CodeSymbol AddArguments(SymbolTable table, string name, string type, int index, object value = null)
        {
            CodeSymbol sym = new CodeSymbol(name, type, new CodeSymbolLocation(CodeSymbolLocationType.Argument, index), value);

            table.AddSymbol(sym);

            return sym;
        }

        public void AddImport(Assembly assembly)
            => imports.Add(assembly);

        public InterMethod FromSignature(string sig)
            => _localFunctions[sig];

        public IMethodWrapper FindClosestWithSignature(string sig, InterType owner)
        {
            if (_localFunctions.ContainsKey(owner.FullName + "." + sig))
                return new InterMethodWrapper(_localFunctions[owner.FullName + "." + sig]);

            //TODO: improve this
            if (sig.Contains(".")) {
                string typeName = sig[0..sig.LastIndexOf('.')];
                string funcName = sig[(sig.LastIndexOf('.')+1)..sig.LastIndexOf('(')];

                foreach (var a in imports)
                    foreach (Type t in (from type in a.GetTypes() where type.Name == typeName select type).ToArray())
                        foreach(var m in t.GetMethods())
                            if (m.Name == funcName)
                                return new MethodInfoWrapper(m);
            }
            else
            {
                string funcName = sig[(sig.LastIndexOf('.')+1)..sig.LastIndexOf('(')];

                foreach (var a in imports)
                    foreach (Type t in a.GetTypes())
                        foreach (var m in t.GetMethods())
                            if (m.Name == funcName)
                                return new MethodInfoWrapper(m);
            }

            return null;
        }


        public void Emit(IlBuilder builder)
        {
            foreach (InterType t in Types)
                t.Emit(builder, this);
        }

    }
}
