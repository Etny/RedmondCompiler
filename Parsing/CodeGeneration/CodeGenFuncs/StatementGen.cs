using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
    {

        [CodeGenFunction("AssignStatement")]
        public void CompileAssignStatement(SyntaxTreeNode node)
        {
            CompileNode(node.Children[1]);
            builder.EmitLine("Pop to ID " + node.Children[0].Val);
        }

        [CodeGenFunction("IfStatement")]
        public void CompileIfStatement(SyntaxTreeNode node)
        {
            CompileNode(node.Children[0]);

            builder.EmitLine("If then");
            builder.EmitLine("{");
            builder.Output.AddIndentation();

            CompileNode(node.Children[1]);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
        }
    }
}
