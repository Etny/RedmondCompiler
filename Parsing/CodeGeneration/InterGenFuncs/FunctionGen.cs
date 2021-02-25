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
            string targetName = node[0].ValueString;

            InterInstOperand[] parameters = new InterInstOperand[node[1].Children.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = ToIntermediateExpression(node[1][i]);
            }

            builder.AddInstruction(new InterCall(node[0].ValueString, parameters));
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
            string[] argTypes = new string[1];
            string[] argNames = new string[1];
            List<string> functionKeywords = new List<string>();

            foreach(var child in node.Children)
            {
                switch (child.Op)
                {
                    case "AccessKeyword":

                    case "FunctionKeywords":
                        foreach (var kw in child.Children)
                            functionKeywords.Add(kw.ValueString);
                        break;

                    case "ParameterDecList":
                        var args = CompileParameterDecList(child);
                        argTypes = args.Item1;
                        argNames = args.Item2;
                        break;

                    case "IdentifierName":
                        name = child.ValueString;
                        //if (Tables.Peek().Contains(name)) throw new Exception("Already exists :(");
                        //Tables.Peek().AddSymbol(new SymbolManagement.CodeSymbol(name, "function"));
                        break;
                }
            }
            PushNewTable();

            InterMethod method = builder.AddMethod(name, "void", argTypes);
            foreach (string k in functionKeywords) method.AddFlag(k);
            for (int i = 0; i < argNames.Length; i++) builder.AddArguments(CurrentTable, argNames[i], argTypes[i], i);
        }

        //TODO: Improve this
        private (string[], string[]) CompileParameterDecList(SyntaxTreeNode node)
        {
            string[] types = new string[node.Children.Length];
            string[] names = new string[node.Children.Length];


            for (int i = 0; i < types.Length; i++)
            {
                types[i] = CodeType.ByName(node[i][0].ValueString).Name;
                names[i] = node[i][1].ValueString;
            }

            return (types, names);
        }

    }
}
