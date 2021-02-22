using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class IntermediateGenerator
    {

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
            builder.AddInstruction(new InterCall(node.ValueString));
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

                    case "IdentifierName":
                        name = child.ValueString;
                        if (Tables.Peek().Contains(name)) throw new Exception("Already exists :(");
                        Tables.Peek().AddSymbol(new SymbolManagement.CodeSymbol(name, "function"));
                        break;
                }
            }
            PushNewTable();

            InterMethod method = new InterMethod(name, "void");
            foreach (string k in functionKeywords) method.AddFlag(k);
            builder.AddMethod(method);
        }

    }
}
