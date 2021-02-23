using Redmond.Parsing.CodeGeneration.IntermediateCode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

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

        public InterMethod AddMethod(string name, string returnType)
        {
            var method = new InterMethod(name, returnType, CurrentType);
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

        public void AddImport(Assembly assembly)
            => imports.Add(assembly);

        public InterMethod FromSignature(string sig)
            => _localFunctions[sig];

        public InterMethod FindClosestWithSignature(string sig, InterType owner)
        {
            if (_localFunctions.ContainsKey(owner.FullName + "." + sig))
                return _localFunctions[owner.FullName + "." + sig];

            //TODO: improve this
            if (sig.Contains(".")) {
                string typeName = sig[0..^sig.LastIndexOf('.')];
                string funcName = sig[sig.LastIndexOf('.')..sig.IndexOf('(')];

                foreach (var a in imports)
                {
                    foreach (Type t in (Type[])from type in a.GetTypes() where type.Name == typeName)
                    {
                        if(t.GetMethod(funcName) != null)
                        {
                            //return that
                        }
                    }
                }
            }
            else
            {
                string funcName = sig[..sig.IndexOf('(')];

                foreach (var a in imports)
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.GetMethod(funcName) != null)
                        {
                            //return that
                        }
                    }
                }
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
