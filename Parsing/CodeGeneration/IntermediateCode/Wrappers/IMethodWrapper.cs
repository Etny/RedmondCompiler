using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IMethodWrapper
    {
        bool IsVirtual { get; }
        bool IsInstance { get; }

        string FullSignature { get; }

        string Name { get; }

        CodeType ReturnType { get; }

        int ArgumentCount { get; }

        CodeType[] Arguments { get; }
    }

    class InterMethodWrapper : IMethodWrapper
    {

        protected InterMethod _method;
        protected IntermediateBuilder _context;

        public InterMethodWrapper(InterMethod method, IntermediateBuilder context) { _method = method; _context = context;/*foreach (var a in _method.Arguments) { a.Bind(context); }*/ }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => _method.IsInstance;

        public virtual string FullSignature => _method.CallSignature;

        public string Name => _method.Name;

        public virtual CodeType ReturnType => _method.ReturnType;

        public int ArgumentCount => _method.Args;

        public virtual CodeType[] Arguments {
            get
            {
                foreach (var a in _method.Arguments) a.Bind(_context);
                return (from a in _method.Arguments select a.Type.StoredType).ToArray();
            }
        }

    }

    class GenericInterMethodWrapper : InterMethodWrapper
    {
        private InterGenericType _type;

        private CodeType[] _args;
        private CodeType _returnType;

        public GenericInterMethodWrapper(InterMethod method, IntermediateBuilder context, InterGenericType type) : base(method, context)
        {
            _type = type;

            if (method.ReturnType is GenericParameterType)
            {
                var par = method.ReturnType as GenericParameterType;
                _returnType = new GenericParameterType(_type.GenericParameters[par.Index], par.Index);
            }
            else
                _returnType = method.ReturnType;

            _args = new CodeType[method.Arguments.Length];

            for (int i = 0; i < _args.Length; i++)
            {
                var p = method.Arguments[i];
                p.Bind(context);

                if (!(p.Type is GenericParameterType))
                    _args[i] = p.Type.StoredType;
                else
                {
                    var par = p.Type as GenericParameterType;
                    _args[i] = new GenericParameterType(_type.GenericParameters[par.Index], par.Index);
                }
            }
        }
        public override string FullSignature
          => $"{(_method.IsInstance ? "instance " : "")}{ReturnType.Name} {_type.Name}::{_method.Name}({_method.ArgList})";


        public override CodeType[] Arguments => _args;

        public override CodeType ReturnType => _returnType;

    }

    class MethodInfoWrapper : IMethodWrapper
    {

        protected MethodInfo _method;
        protected IntermediateBuilder _context;

        public MethodInfoWrapper(MethodInfo method, IntermediateBuilder context)
        {
            _method = method;
            _context = context;
        }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => !_method.IsStatic;

        public virtual CodeType ReturnType => _context.ToCodeType(_method.ReturnType).StoredType;

        public virtual string FullSignature
            => $"{functionPrefixes}{_method.DeclaringType.FullName}::{_method.Name}({string.Join(',', from a in Arguments select a.ArgumentName)})";

        public string Name => _method.Name;

        public int ArgumentCount => _method.GetParameters().Length;

        public virtual CodeType[] Arguments => (from p in _method.GetParameters() select _context.ToCodeType(p.ParameterType).StoredType).ToArray();

        protected virtual string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += ReturnType.ArgumentName + " ";
                s += $"[{_method.Module.Name[.._method.Module.Name.LastIndexOf(".dll")]}]";

                return s;
            }
        }

    }

    class GenericMethodInfoWrapper : MethodInfoWrapper
    {

        private CodeType[] _genericTypes;
        private CodeType[] _args;
        private GenericType _type;
        private CodeType _returnType;

        public GenericMethodInfoWrapper(MethodInfo method, IntermediateBuilder context, GenericType generic) : base(method, context)
        {
            _type = generic;
            _genericTypes = generic.GenericParameters;

            _returnType = GenericType.FinalizeGenericType(method.ReturnType, _context, _type.GenericParameters);

            _args = new CodeType[method.GetParameters().Length];

            for(int i = 0; i < _args.Length; i++)
                _args[i] = GenericType.FinalizeGenericType(method.GetParameters()[i].ParameterType, _context, _type.GenericParameters);


        }

        public override string FullSignature
          => $"{functionPrefixes}{_type.Name}::{_method.Name}({string.Join(',', from a in Arguments select a.ArgumentName)})";

        public override CodeType ReturnType => _returnType;


        protected override string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += ReturnType.ArgumentName + " ";

                return s;
            }
        }

        public override CodeType[] Arguments => _args;
    }

    class ConstructorInfoWrapper : IMethodWrapper
    {

        protected ConstructorInfo _constructor;
        protected IntermediateBuilder _context;

        public ConstructorInfoWrapper(ConstructorInfo constructor, IntermediateBuilder context)
        {
            _constructor = constructor;
            _context = context;

            ReturnType = CodeType.Void;
        }

        public bool IsVirtual => _constructor.IsVirtual;

        public bool IsInstance => !_constructor.IsStatic;

        public CodeType ReturnType { get; }

        public virtual string FullSignature
            => $"{functionPrefixes}{_constructor.DeclaringType.FullName}::{_constructor.Name}({string.Join(',', from a in Arguments select a.Name)})";

        public string Name => _constructor.Name;

        public int ArgumentCount => _constructor.GetParameters().Length;

        public CodeType[] Arguments => (from p in _constructor.GetParameters() select _context.ToCodeType(p.ParameterType).StoredType).ToArray();

        protected virtual string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += ReturnType.Name + " ";
                s += $"[{_constructor.Module.Name[.._constructor.Module.Name.LastIndexOf(".dll")]}]";

                return s;
            }
        }

    }

    class GenericConstructorInfoWrapper : ConstructorInfoWrapper
    {

        private GenericType _type;

        public GenericConstructorInfoWrapper(ConstructorInfo constructor, IntermediateBuilder context, GenericType type)
            : base(constructor, context)
        {
            _type = type;
        }

        public override string FullSignature
            => $"{functionPrefixes}{_type.Name}::{_constructor.Name}({string.Join(',', from a in Arguments select a.Name)})";


        protected override string functionPrefixes
            => "instance void ";

    }
}
