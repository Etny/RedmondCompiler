using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        private static Dictionary<string, MethodInfo> _codeGenFunctions = new Dictionary<string, MethodInfo>();

        
        private void _InitCodeGenFunctions()
        {
            foreach(var func in typeof(IntermediateGenerator).GetMethods())
            {
                var atts = func.GetCustomAttributes(typeof(CodeGenFunctionAttribute));
                foreach(var a in atts)
                    _codeGenFunctions.Add((a as CodeGenFunctionAttribute).Name.ToLower(), func);
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private class CodeGenFunctionAttribute : Attribute
        {
            public readonly string Name;
            public CodeGenFunctionAttribute(string name) { Name = name; }
        }

    }
}
