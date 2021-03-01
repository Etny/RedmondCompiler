using Redmond.Parsing.CodeGeneration.SymbolManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    interface IMethodWrapper
    {
        bool IsVirtual { get; }
        bool IsInstance { get; }

        string FullSignature { get; }

        CodeType ReturnType { get; }
    }

    class InterMethodWrapper : IMethodWrapper
    {

        private InterMethod _method;

        public InterMethodWrapper(InterMethod method, IntermediateBuilder context) { _method = method;  }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => _method.IsInstance;

        public string FullSignature => _method.CallSignature;

        public CodeType ReturnType => _method.ReturnType;
    }

    class MethodInfoWrapper : IMethodWrapper
    {

        private MethodInfo _method;

        public MethodInfoWrapper(MethodInfo method, IntermediateBuilder context) { _method = method; ReturnType = context.ResolveType(method.ReturnType); }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => !_method.IsStatic;

        public CodeType ReturnType { get; }

        public string FullSignature 
            => $"{functionPrefixes}{_method.DeclaringType.FullName}::{_method.Name}()";

        private string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += _method.ReturnType.Name.ToLower() + " ";
                s += $"[{_method.Module.Name[.._method.Module.Name.LastIndexOf(".dll")]}]";

                return s;
            }
        }

    }
}
