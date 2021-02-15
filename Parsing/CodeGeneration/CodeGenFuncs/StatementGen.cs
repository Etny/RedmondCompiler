using Redmond.Output.Error;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
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

            string name = node.Children[0].ValueString;

            CodeSymbol symbol = GetFirst(name) ?? CurrentTable.AddSymbol(new CodeSymbol(name, "int32"));


            builder.EmitLine("Pop to ID " + symbol);
        }

        [CodeGenFunction("DeclarationStatement")]
        public void CompileDeclarationStatement(SyntaxTreeNode node)
        {
            string name = node.Children[0].ValueString;

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol =  CurrentTable.AddSymbol(new CodeSymbol(name, node.Children[2].ValueString));

            CompileNode(node.Children[1]);

            builder.EmitLine("Pop to ID " + symbol);
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
