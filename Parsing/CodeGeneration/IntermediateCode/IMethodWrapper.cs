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


    }

    public class InterMethodWrapper : IMethodWrapper
    {

        private InterMethod _method;

        internal InterMethodWrapper(InterMethod method) { _method = method; }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => _method.IsInstance;

        public string FullSignature => $"{functionPrefixes}{_method.Name}()";

        private string functionPrefixes
        {
            get
            {
                string s = "";

                if (IsInstance) s += "instance ";
                s += _method.ReturnType + " ";
                s += _method.Owner.FullName + "::";

                return s;
            }
        }
    }

    public class MethodInfoWrapper : IMethodWrapper
    {

        private MethodInfo _method;

        public MethodInfoWrapper(MethodInfo method) { _method = method; }

        public bool IsVirtual => _method.IsVirtual;

        public bool IsInstance => !_method.IsStatic;

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
