﻿using Redmond.Parsing.CodeGeneration.SymbolManagement;
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

        private InterMethod _method;

        public InterMethodWrapper(InterMethod method, IntermediateBuilder context) { _method = method;  }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => _method.IsInstance;

        public string FullSignature => _method.CallSignature;

        public string Name => _method.Name;

        public CodeType ReturnType => _method.ReturnType.StoredType;

        public int ArgumentCount => _method.Args;

        public CodeType[] Arguments => (from a in _method.Arguments select a.Type.StoredType).ToArray();
    }

    class MethodInfoWrapper : IMethodWrapper
    {

        private MethodInfo _method;
        private IntermediateBuilder _context;

        public MethodInfoWrapper(MethodInfo method, IntermediateBuilder context)
        {
            _method = method;
            _context = context;

            ReturnType = context.ResolveType(method.ReturnType).StoredType;
        }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => !_method.IsStatic;

        public CodeType ReturnType { get; }

        public string FullSignature 
            => $"{functionPrefixes}{_method.DeclaringType.FullName}::{_method.Name}({string.Join(',', from a in Arguments select a.Name)})";

        public string Name => _method.Name;

        public int ArgumentCount => _method.GetParameters().Length;

        public CodeType[] Arguments => (from p in _method.GetParameters() select _context.ResolveType(p.ParameterType).StoredType).ToArray();

        private string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += ReturnType.Name + " ";
                s += $"[{_method.Module.Name[.._method.Module.Name.LastIndexOf(".dll")]}]";

                return s;
            }
        }

    }

    class ConstructorInfoWrapper : IMethodWrapper
    {

        private ConstructorInfo _constructor;
        private IntermediateBuilder _context;

        public ConstructorInfoWrapper(ConstructorInfo constructor, IntermediateBuilder context)
        {
            _constructor = constructor;
            _context = context;

            ReturnType = CodeType.Void;
        }

        public bool IsVirtual => _constructor.IsVirtual;

        public bool IsInstance => !_constructor.IsStatic;

        public CodeType ReturnType { get; }

        public string FullSignature
            => $"{functionPrefixes}{_constructor.DeclaringType.FullName}::{_constructor.Name}({string.Join(',', from a in Arguments select a.Name)})";

        public string Name => _constructor.Name;

        public int ArgumentCount => _constructor.GetParameters().Length;

        public CodeType[] Arguments => (from p in _constructor.GetParameters() select _context.ResolveType(p.ParameterType).StoredType).ToArray();

        private string functionPrefixes
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
}
