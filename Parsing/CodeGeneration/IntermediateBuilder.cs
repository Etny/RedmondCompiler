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
        public Stack<string> Namespaces = new Stack<string>();

        private Stack<SymbolTable> _tables;

        private AssemblyReferenceTracker ReferenceTracker = new AssemblyReferenceTracker();
        public ImmutableList<AssemblyReference> AssemblyReferences { get; protected set; } = ImmutableList<AssemblyReference>.Empty;
        public ImmutableList<string> ImportedNamespaces { get; protected set; } = ImmutableList<string>.Empty;

        private List<InterBranch> _breakInstruction = new List<InterBranch>();

        private string assemblyName = "Redmond";

        public IntermediateBuilder(Stack<SymbolTable> tables)
        {
            AddReference(new CoreAssemblyReference());  

            var t = Assembly.Load("System.Reflection.Primitives");
            var ts = t.GetType("OpCode");
            var type = typeof(System.Reflection.Emit.OpCode);

            _tables = tables;
        }


        public ResolutionContext CurrentNamespaceContext
        {
            get
            {
                if (CurrentType != null)
                    return CurrentType.NamespaceContext;
                else
                    return new ResolutionContext(Namespaces);
            }
        }

        public InterType AddType(InterType type)
        {
            CurrentType = type;
            CurrentMethod = null;
            Types.Add(type);
            return type;
        }

        public void BeginNamespace(string name)
        {
            Namespaces.Push(name);
        }

        public void EndNameSpace()
        {
            Namespaces.Pop();
        }

        public InterBranch[] GetBreaks()
        {
            var temp = _breakInstruction.ToArray();
            _breakInstruction.Clear();
            return temp;
        }

        public void AddBreakStatement()
        {
            var b = new InterBranch();
            AddInstruction(b);
            _breakInstruction.Add(b);
        }

        public InterMethod AddMethod(string name, TypeName returnType, ArgumentSymbol[] vars, List<string> flags)
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

        public InterConstructor AddConstructor(ArgumentSymbol[] vars, List<string> flags, bool callThis)
        {
            var method = new InterConstructor(vars, CurrentType, flags, callThis);
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
        public LocalSymbol AddLocal(string name, TypeName type, object value = null)
        {
            LocalSymbol sym = new LocalSymbol(name, type, CurrentMethod.Locals.Count, value);
            CurrentMethod.Locals.Add(sym);

            _tables.Peek().AddSymbol(sym);

            return sym;
        }

        public LocalSymbol AddLocal(string name, CodeValue op, object value = null)
        {
            LocalSymbol sym = new LocalSymbol(name, op, CurrentMethod.Locals.Count, value);
            CurrentMethod.Locals.Add(sym);

            _tables.Peek().AddSymbol(sym);

            return sym;
        }

        public InterField AddField(string name, TypeName type, string access, List<string> keywords)
        {
            InterField field = new InterField(name, type, access, keywords, new InterUserType(CurrentType));
            _tables.Peek().AddSymbol(field.Symbol);
            CurrentType.AddField(field);
            return field;
        }

        public InterProperty AddProperty(string name, TypeName type, string access, List<string> keywords)
        {
            InterProperty property = new InterProperty(name, type, access, keywords, new InterUserType(CurrentType));
            CurrentType.AddProperty(property);
            return property;
        }

        public void ClearImports() => ImportedNamespaces = ImmutableList<string>.Empty;

        public void AddImport(string nameSpace)
            => ImportedNamespaces = ImportedNamespaces.Add(nameSpace);

        public void AddReference(AssemblyReference reference)
            => AssemblyReferences = AssemblyReferences.Add(reference);

        
        public CodeType ResolveType(TypeName name)
        {
            if(name is ArrayTypeName)
            {
                var array = name as ArrayTypeName;
                return new ArrayType(ResolveType(array.ElementType));
            }else if(name is GenericTypeName)
            {
                var generic = name as GenericTypeName;

                CodeType[] parameters = new CodeType[generic.GenericParameters.Length];
                var genericName = UserType.ToUserType(ResolveType(new BasicTypeName(generic.baseName.Name + '`' + parameters.Length, generic.baseName.NamespaceContext)));

                for (int i = 0; i < parameters.Length; i++)
                    parameters[i] = ResolveType(generic.GenericParameters[i]);

                return GenericType.NewGenericType(genericName, parameters);
            }

            if (name.NamespaceContext.GenericParameters.ContainsKey(name.Name))
                return name.NamespaceContext.GenericParameters[name.Name];

            var ctype = CodeType.ByName(name.Name);
            if (ctype != null) return ctype;

            foreach (string ns in name.NamespaceContext.TravelUpHierarchy())
            {
                foreach (InterType type in Types)
                {
                    if (type.FullName == ns + '.' + name.Name) return new InterUserType(type);
                }
            }


            foreach (string ns in ImportedNamespaces)
            {
                foreach (InterType type in Types)
                    if (type.FullName == ns + "." + name.Name) return new InterUserType(type);

                foreach (var a in AssemblyReferences)
                {
                    var type = a.ResolveType(ns + "." + name.Name);
                    if (type == null) type = a.ResolveType(name.Name);
                    if (type != null)
                    {
                        ReferenceTracker.AddUsedReference(type);
                        return UserType.NewUserType(type);
                    }
                }
            }

            return null;
        }

        public CodeType ToCodeType(Type type, bool parseGenerics = true)
        {
            if (CodeType.ByName(type.Name.ToLower()) != null) return CodeType.ByName(type.Name.ToLower());

            UserType ut = null;

            if (type.IsArray)
                ut = new ArrayType(ToCodeType(type.GetElementType()));
            else if (type.IsGenericType && parseGenerics)
            {
                CodeType[] parameters = new CodeType[type.GetGenericArguments().Length];
                for (int i = 0; i < parameters.Length; i++) 
                    parameters[i] = new GenericParameterType(ToCodeType(type.GetGenericArguments()[i]), i);

                ut = GenericType.NewGenericType(UserType.NewUserType(type), parameters);
            }
            else
                ut = UserType.NewUserType(type);

            if (ut != null) ReferenceTracker.AddUsedReference(ut.GetAssembly());

            return ut;
        }


        public IMethodWrapper FindClosestFunction(string name, CodeType owner, CodeValue[] args, bool canBeNull = false)
        {
            var type = UserType.ToUserType(owner);
            Debug.Assert(type != null);

            List<IMethodWrapper> applicableFunctions = new List<IMethodWrapper>();

            foreach (var f in type.GetFunctions(this))
                if (f.Name == name && f.ArgumentCount == args.Length)
                    applicableFunctions.Add(f);

            if(applicableFunctions.Count == 0 && !canBeNull && type.GetBaseType() != null)
                return FindClosestFunction(name, type.GetBaseType(), args, canBeNull);
            

            //Debug.Assert(applicableFunctions.Count > 0 || canBeNull);

            return FindClosest(applicableFunctions, args, canBeNull);
        }

        public IMethodWrapper FindClosestIndexerGet(CodeType owner, params CodeValue[] args)
        {
            var type = UserType.ToUserType(owner);
            Debug.Assert(type != null);

            List<IMethodWrapper> applicableFunctions = new List<IMethodWrapper>();

            foreach (var f in type.GetIndexers(this))
                if (f.GetFunction.ArgumentCount == args.Length)
                    applicableFunctions.Add(f.GetFunction);

            Debug.Assert(applicableFunctions.Count > 0);

            return FindClosest(applicableFunctions, args);
        }


        public IMethodWrapper FindMostApplicableConstructor(UserType type, CodeValue[] args)
            => FindClosest(type.GetConstructors(this), args);

        private IMethodWrapper FindClosest(IEnumerable<IMethodWrapper> funcs, CodeValue[] args, bool canBeNull = false)
        {
            //TODO: Merge this part with FindClosestFunction
            IMethodWrapper closest = null;
            float lowestDifference = -1;

            foreach (var f in funcs)
            {
                if (f.ArgumentCount != args.Length) continue;

                bool canConvert = true;
                float diff = 0;
                for (int i = 0; i < args.Length; i++)
                {
                    var argType = args[i].Type;
                    var funcType = f.Arguments[i];

                    //if (argType != funcType && (argType.GetWiderType(funcType) == null || argType.GetWiderType(funcType) == argType)) { canConvert = false; break; }
                    canConvert = argType.StoredType.CanAssignTo(funcType.StoredType) != AssignType.CannotAssign;

                    if (funcType.StoredType != argType.StoredType) diff++;
                }

                if (!canConvert) continue;

                if (lowestDifference < 0 || diff < lowestDifference)
                {
                    lowestDifference = diff;
                    closest = f;
                }
            }

            Debug.Assert(closest != null || canBeNull);

            return closest;
        }

        public void Emit(IlBuilder builder)
        {
            builder.Start();

            foreach (InterType t in Types)
                t.Bind(this);

            foreach (InterType t in Types)
                t.BindSubMembers(this);

            foreach (InterType t in Types)
                t.BindSubSubMembers(this);

            EmitCLRHeader(builder);

            foreach (InterType t in Types)
                t.Emit(builder);

            builder.End();
        }

        private void EmitCLRHeader(IlBuilder builder)
        {
            builder.EmitLine($".assembly {assemblyName} {{}}");

            builder.EmitLine();

            foreach (var reference in ReferenceTracker.UsedReferences)
                reference.Emit(builder);

            builder.EmitLine(".module TestModule");

            builder.EmitLine();


        }

    }
}
