using Redmond.Output.Error;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        [CodeGenFunction("AssignStatement")]
        public void CompileAssignStatement(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;
            CodeSymbol symbol = GetFirst(name);//?? CurrentTable.AddSymbol(new CodeSymbol(name, "int32"));

            builder.AddInstruction(new InterCopy(symbol, ToIntermediateExpression(node[1])));
        }

        [CodeGenFunction("DeclarationStatement")]
        public void CompileDeclarationStatement(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol = builder.AddLocalSymbol(name, node[1].ValueString);//CurrentTable.AddSymbol(new CodeSymbol(name, node[2].ValueString));

            if(node.Children.Length > 2)
                builder.AddInstruction(new InterCopy(symbol, ToIntermediateExpression(node[2])));
        }

        [CodeGenFunction("ReturnStatement")]
        public void CompileReturnStatement(SyntaxTreeNode node)
        {
            if (node.Children.Length > 0)
                builder.AddInstruction(new InterRet(ToIntermediateExpression(node[0])));
            else
                builder.AddInstruction(new InterRet());
        }

        [CodeGenFunction("IfStatement")]
        public void CompileIfStatement(SyntaxTreeNode node)
        {
            CompileNode(node.Children[0]);


            CompileNode(node.Children[1]);
        }
    }
}
