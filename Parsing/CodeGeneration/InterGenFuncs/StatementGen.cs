using Redmond.IO.Error;
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
            TypeName type = TypeNameFromNode(node[0]);

            foreach (var n in node[1].Children)
                CompileVarDec(type, n);
        }

        public void CompileVarDec(TypeName type, SyntaxTreeNode node)
        {
            string name = node[0].ValueString;

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol = builder.AddLocal(name, type);//CurrentTable.AddSymbol(new CodeSymbol(name, node[2].ValueString));

            if (node.Children.Length > 1)
                builder.AddInstruction(new InterCopy(symbol, ToIntermediateExpression(node[1])));
        }

        [CodeGenFunction("VarDeclarationStatement")]
        public void CompileVarDeclarationStatement(SyntaxTreeNode node)
        {
            string name = node[0].ValueString;
            var value = ToIntermediateExpression(node[1]);

            if (CurrentTable.Contains(name))
                ErrorManager.ExitWithError(new Exception("Duplicate ID: " + name));

            CodeSymbol symbol = builder.AddLocal(name, value);//CurrentTable.AddSymbol(new CodeSymbol(name, node[2].ValueString));

            if (node.Children.Length > 1)
                builder.AddInstruction(new InterCopy(symbol, value));
        }

        [CodeGenFunction("TryBlock")]
        public void CompileTryBlock(SyntaxTreeNode node)
        {
            List<InterBranch> exits = new List<InterBranch>();

            void StartTryBlock()
            {
                builder.AddInstruction(new InterBlock(".try"));
            }
            
            void EndTryBlock()
            {

                InterBranch b1 = new InterBranch(InterBranch.BranchCondition.Leave);
                exits.Add(b1);
                builder.AddInstruction(b1);
                builder.AddInstruction(new InterBlock());
            }

            bool hasFinally = node.Children.Length > 2;

            if (hasFinally)
                StartTryBlock();

            StartTryBlock();
            CompileNode(node[0]);
            EndTryBlock();


            foreach(var c in node[1].Children)
            {
                bool hasException = c.Children.Length > 1;
                TypeName exceptionType = hasException ? TypeNameFromNode(c[1]) : TypeName.Unknown;

                PushNewTable();

                if (hasException)
                {
                    var loc = builder.AddLocal(c[2].ValueString, exceptionType);
                    builder.AddInstruction(new InterCopy(loc, new InterOpValue(new InterBlock("catch", exceptionType))));
                }
                else
                    builder.AddInstruction(new InterBlock(".catch"));

                CompileNode(c[0]);
                InterBranch b = new InterBranch(InterBranch.BranchCondition.Leave);
                exits.Add(b);
                builder.AddInstruction(b);

                Tables.Pop();

                builder.AddInstruction(new InterBlock());
            }

            if(hasFinally)
            {
                EndTryBlock();
                builder.AddInstruction(new InterBlock("finally"));
                CompileNode(node[2][0]);
                builder.AddInstruction(new InterEndFinally());
                builder.AddInstruction(new InterBlock());
            }

            string exitLabel = builder.CurrentMethod.NextLabel;
            foreach (var b in exits) b.SetLabel(exitLabel);

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

            string endLabel = builder.CurrentMethod.NextLabel;

            foreach (var b in builder.GetBreaks())
                b.SetLabel(endLabel);

            foreach (var c in builder.GetContinues())
                c.SetLabel(beginLabel);

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

            string endLabel = builder.CurrentMethod.NextLabel;

            foreach (var b in builder.GetBreaks())
                b.SetLabel(endLabel);

            foreach (var c in builder.GetContinues())
                c.SetLabel(beginLabel);

            Tables.Pop();
        }

        [CodeGenFunction("ForStatement")]
        public void CompileForStatement(SyntaxTreeNode node)
        {
            PushNewTable();

            if (node[0].Op != "Empty") CompileNode(node[0]);

            InterBranch initialBranch = new InterBranch();

            builder.AddInstruction(initialBranch);

            string beginLabel = builder.CurrentMethod.NextLabel;
            if (node[3].Op != "Empty") CompileNode(node[3]);

            string continueLabel = builder.CurrentMethod.NextLabel;
            if (node[2].Op != "Empty") CompileStatementExpressionList(node[2]); 

            initialBranch.SetLabel(builder.CurrentMethod.NextLabel);

            InterBranch checkBranch = new InterBranch(ToIntermediateExpression(node[1]), InterBranch.BranchCondition.OnTrue);
            checkBranch.SetLabel(beginLabel);
            builder.AddInstruction(checkBranch);

            string endLabel = builder.CurrentMethod.NextLabel;

            foreach (var b in builder.GetBreaks())
                b.SetLabel(endLabel);

            foreach (var c in builder.GetContinues())
                c.SetLabel(continueLabel);

            Tables.Pop();
        }

        [CodeGenFunction("ForEachStatement")]
        public void CompileForEachStatement(SyntaxTreeNode node)
        {
            PushNewTable();

            var enumerable = ToIntermediateExpression(node[2]);
            var typeVal = new GenericParamValue(enumerable);

            var enumCall = new InterPushEnumerator(enumerable, typeVal);
            var enumVal = new InterOpValue(enumCall);
            var enumerator = builder.AddLocal("loc_" + builder.CurrentMethod.Locals.Count, enumVal);
            builder.AddInstruction(new InterCopy(enumerator, enumVal));


            InterBranch initialBranch = new InterBranch();

            var loc = builder.AddLocal(node[1].ValueString, typeVal);

            builder.AddInstruction(new InterBlock(".try"));

            builder.AddInstruction(initialBranch);
            string beginLabel = builder.CurrentMethod.NextLabel;
            builder.AddInstruction(new InterCopy(loc, new InterOpValue(new InterCall("get_Current", new CodeValue[0], true, enumerator))));
            CompileNode(node[3]);

            var continueLabel = builder.CurrentMethod.NextLabel;
            initialBranch.SetLabel(continueLabel);
            var moveCall = new InterCall("MoveNext", new CodeValue[0], true, enumerator)
            {
                ThisPointerTypeNameOverride = new BasicTypeName("System.Collections.IEnumerator", new ResolutionContext())
            };
            InterBranch checkBranch = new InterBranch(new InterOpValue(moveCall), InterBranch.BranchCondition.OnTrue);
            checkBranch.SetLabel(beginLabel);
            builder.AddInstruction(checkBranch);

            InterBranch tryLeave = (InterBranch)builder.AddInstruction(new InterBranch(InterBranch.BranchCondition.Leave));
            builder.AddInstruction(new InterBlock());


            //TODO: Add Try-Finnaly support for the Dispose call

            builder.AddInstruction(new InterBlock("finally"));
            builder.AddInstruction(new InterCall("Dispose", new CodeValue[0], false, enumerator) { ThisPointerTypeNameOverride = new BasicTypeName("System.IDisposable") });
            builder.AddInstruction(new InterEndFinally());
            builder.AddInstruction(new InterBlock());

            string endLabel = builder.CurrentMethod.NextLabel;
            tryLeave.SetLabel(endLabel);

            foreach (var b in builder.GetBreaks())
                b.SetLabel(endLabel);

            foreach (var c in builder.GetContinues())
                c.SetLabel(continueLabel);

            Tables.Pop();
        }

        [CodeGenFunction("SwitchStatement")]
        public void CompileSwitchStatement(SyntaxTreeNode node)
        {
            var exp = ToIntermediateExpression(node[0]);
            var loc = builder.AddLocal("switchLocl" + builder.CurrentMethod.Locals.Count, exp);
            builder.AddInstruction(new InterCopy(loc, exp));

            InterBranch[] branches = new InterBranch[node[1].Children.Length];

            SyntaxTreeNode defaultCase = null;
            InterBranch defaultBranch = null;

            for(int i = 0; i < branches.Length; i++)
            {
                var sect = node[1][i];

                if(sect[0].Op == "DefaultLabel") { defaultCase = sect; continue; }

                var bin = new InterBinOp(new Operator(Operator.OperatorType.Ceq), ToIntermediateExpression(sect[0][0]), loc);
                var branch = new InterBranch(new InterOpValue(bin), InterBranch.BranchCondition.OnTrue);
                builder.AddInstruction(branch);
                branches[i] = branch;
            }

            if(defaultCase == null)
                builder.AddBreakStatement();
            else
            {
                defaultBranch = new InterBranch();
                builder.AddInstruction(defaultBranch);
            }

            for (int i = 0; i < branches.Length; i++)
            {
                var sect = node[1][i];

                if (sect[0].Op == "DefaultLabel") continue;

                string label = builder.CurrentMethod.NextLabel;

                CompileNode(sect[1]);

                branches[i].SetLabel(label);
            }

            if (defaultCase != null)
            {
                string defaultLabel = builder.CurrentMethod.NextLabel;
                CompileNode(defaultCase[1]);
                defaultBranch.SetLabel(defaultLabel);
            }

            string endLabel = builder.CurrentMethod.NextLabel;

            foreach (var b in builder.GetBreaks())
                b.SetLabel(endLabel);
        }

        [CodeGenFunction("BreakStatement")]
        public void CompileBreakStatement(SyntaxTreeNode node)
            => builder.AddBreakStatement();

        [CodeGenFunction("ContinueStatement")]
        public void CompileContinueStatement(SyntaxTreeNode node)
            => builder.AddContinueStatement();
    }
}
