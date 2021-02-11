using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("block")]
        [CodeGenFunction("stmntList")]
        public void CompileCompound(SyntaxTreeNode node)
        {
            foreach (var child in node.Children)
                CompileNode(child);
        }

        [CodeGenFunction("call")]
        public void CompileFunctionCall(SyntaxTreeNode node)
        {
            builder.EmitString("Call to: " + node.Children[0].Val);
        }

    }
}
