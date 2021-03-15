using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

        [CodeGenFunction("File")]
        [CodeGenFunction("ImportList")]
        [CodeGenFunction("ClassList")]
        [CodeGenFunction("MemberList")]
        [CodeGenFunction("CompoundStatement")]
        [CodeGenFunction("StatementList")]
        public void CompileCompound(SyntaxTreeNode node)
        {
            foreach (var child in node.Children)
                CompileNode(child);
        }

        [CodeGenFunction("Call")]
        public void CompileFunctionCall(SyntaxTreeNode node)
        {
            builder.AddInstruction(CompileCall(node, false));
        }

        public InterCall CompileCall(SyntaxTreeNode node, bool exp)
        {
            InterInstOperand[] parameters = new InterInstOperand[node[1].Children.Length];

            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = ToIntermediateExpressionOrLateStaticBind(node[1][i]);

            CodeValue thisPtr = null;

            if (node.Children.Length > 2)
            {
                var member = ToIntermediateExpression(node[2]);

                if (member == null)
                    return new InterCall(node[0].ValueString, parameters, exp, new LateStaticReferenceResolver(node[2]));

                thisPtr = member.ToValue();

            }

            return new InterCall(node[0].ValueString, parameters, exp, thisPtr);
        }

        [CodeGenFunction("Function")]
        public void CompileFunction(SyntaxTreeNode node)
        {
            MatchNode(node.Children[0], "FunctionDec");

            MatchNode(node.Children[1], "FunctionBody");

            Tables.Pop();
        }

        [CodeGenFunction("FunctionBody")]
        public void CompileFunctionBody(SyntaxTreeNode node)
        {
            CompileNodes(node.Children);
        }

        [CodeGenFunction("FunctionDec")]
        public void CompileFunctionDeclaration(SyntaxTreeNode node)
        {
            string name = "error";
            string retType = null;
            ArgumentSymbol[] args = null;
            List<string> functionKeywords = new List<string>();

            foreach(var child in node.Children)
            {
                switch (child.Op)
                {
                    case "AccessKeyword":
                        functionKeywords.Add(child.ValueString);
                        break;

                    case "ModifierList":
                        foreach (var kw in child.Children)
                            functionKeywords.Add(kw.ValueString);
                        break;

                    case "ParameterDecList":
                        args = CompileParameterDecList(child);
                        break;

                    case "Identifier":
                        name = child.ValueString;
                        //if (Tables.Peek().Contains(name)) throw new Exception("Already exists :(");
                        //Tables.Peek().AddSymbol(new SymbolManagement.CodeSymbol(name, "function"));
                        break;

                    case "Name":
                        retType = child.ValueString;
                        break;
                }
            }
            PushNewTable();

            InterMethod method = builder.AddMethod(name, retType, args, functionKeywords);
        }

        //TODO: Improve this
        private ArgumentSymbol[] CompileParameterDecList(SyntaxTreeNode node)
        {
            ArgumentSymbol[] args = new ArgumentSymbol[node.Children.Length];

            for (int i = 0; i < args.Length; i++)
                args[i] = new ArgumentSymbol(node[i][1].ValueString, node[i][0].ValueString, i);

            return args;
        }

    }
}
