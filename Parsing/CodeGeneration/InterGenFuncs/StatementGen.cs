using Redmond.Output.Error;
using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        [CodeGenFunction("AssignStatement")]
        public void CompileAssignStatement(SyntaxTreeNode node)
        {
            CodeSymbol symbol = null;

            if (node[0].Op == "Identifier")
                symbol = GetFirst(node[0].ValueString);
            else if (node[0].Op == "MemberAccess")
                symbol = CompileMemberAccess(node[0]);
            else
                Debug.Assert(false);

            if(symbol == null)
                builder.AddInstruction(new InterCopy(new LateStaticReferenceResolver(node[0]), ToIntermediateExpression(node[1])));
            else
                builder.AddInstruction(new InterCopy(symbol, ToIntermediateExpression(node[1])));

        }

        [CodeGenFunction("DeclarationStatement")]
        public void CompileDeclarationStatement(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol = builder.AddLocal(name, node[1].ValueString);//CurrentTable.AddSymbol(new CodeSymbol(name, node[2].ValueString));

            if(node.Children.Length > 2)
                builder.AddInstruction(new InterCopy(symbol, ToIntermediateExpressionOrLateStaticBind(node[2])));
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
