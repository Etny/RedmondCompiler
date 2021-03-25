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

        [CodeGenFunction("ExpressionStatement")]
        public void CompileExpressionStatement(SyntaxTreeNode node)
        {
            builder.AddInstruction(new InterPush(ToIntermediateExpression(node[0])));
        }

        [CodeGenFunction("DeclarationStatement")]
        public void CompileDeclarationStatement(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol = builder.AddLocal(name, TypeNameFromNode(node[1]));//CurrentTable.AddSymbol(new CodeSymbol(name, node[2].ValueString));

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
            InterBranch branch = new InterBranch(ToIntermediateExpression(node[0]), InterBranch.BranchCondition.OnFalse);

            PushNewTable();

            builder.AddInstruction(branch);
            CompileNode(node[1]);
            branch.SetLabel(builder.CurrentMethod.NextLabel);

            Tables.Pop();
        }

        [CodeGenFunction("IfElseStatement")]
        public void CompileIfElseStatement(SyntaxTreeNode node)
        {
            InterBranch branchOnFalse = new InterBranch(ToIntermediateExpression(node[0]), InterBranch.BranchCondition.OnFalse);
            InterBranch branchAfterTrue = new InterBranch();

            PushNewTable();

            builder.AddInstruction(branchOnFalse);
            CompileNode(node[1]);
            builder.AddInstruction(branchAfterTrue);
            branchOnFalse.SetLabel(builder.CurrentMethod.NextLabel);
            CompileNode(node[2]);
            branchAfterTrue.SetLabel(builder.CurrentMethod.NextLabel);

            Tables.Pop();
        }

        [CodeGenFunction("WhileStatement")]
        public void CompileWhileStatement(SyntaxTreeNode node)
        {
            InterBranch initialBranch = new InterBranch();

            PushNewTable();

            builder.AddInstruction(initialBranch);

            string beginLabel = builder.CurrentMethod.NextLabel;
            CompileNode(node[1]);
            initialBranch.SetLabel(builder.CurrentMethod.NextLabel);

            InterBranch checkBranch = new InterBranch(ToIntermediateExpression(node[0]), InterBranch.BranchCondition.OnTrue);
            checkBranch.SetLabel(beginLabel);
            builder.AddInstruction(checkBranch);

            Tables.Pop();
        }

        [CodeGenFunction("DoWhileStatement")]
        public void CompileDoWhileStatement(SyntaxTreeNode node)
        {
            string beginLabel = builder.CurrentMethod.NextLabel;

            PushNewTable();

            CompileNode(node[1]);

            InterBranch checkBranch = new InterBranch(ToIntermediateExpression(node[0]), InterBranch.BranchCondition.OnTrue);
            checkBranch.SetLabel(beginLabel);
            builder.AddInstruction(checkBranch);

            Tables.Pop();
        }

        [CodeGenFunction("ForStatement")]
        public void CompileForStatement(SyntaxTreeNode node)
        {
            PushNewTable();

            if(node[0].Op != "Empty") CompileNode(node[0]);

            InterBranch initialBranch = new InterBranch();

            builder.AddInstruction(initialBranch);

            string beginLabel = builder.CurrentMethod.NextLabel;
            if (node[3].Op != "Empty") CompileNode(node[3]);
            if (node[2].Op != "Empty")  CompileStatementExpressionList(node[2]);
            initialBranch.SetLabel(builder.CurrentMethod.NextLabel);

            InterBranch checkBranch = new InterBranch(ToIntermediateExpression(node[1]), InterBranch.BranchCondition.OnTrue);
            checkBranch.SetLabel(beginLabel);
            builder.AddInstruction(checkBranch);

            Tables.Pop();
        }
    }
}
