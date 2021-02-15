using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Redmond.Parsing.CodeGeneration
{
    internal partial class CodeGenerator
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
            builder.EmitLine("Call to: " + node.Val);
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
            builder.EmitLine("{");
            builder.Output.AddIndentation();

            CompileNodes(node.Children);

            builder.Output.ReduceIndentation();
            builder.EmitLine("}");
        }

        [CodeGenFunction("FunctionDec")]
        public void CompileFunctionDeclaration(SyntaxTreeNode node)
        {
            string name = "error";
            string access = "error";
            List<string> functionKeywords = new List<string>();

            foreach(var child in node.Children)
            {
                switch (child.Op)
                {
                    case "AccessKeyword":
                        access = child.ValueString;
                        break;

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
            builder.EmitString($".method {access} ");
            foreach (string k in functionKeywords) builder.EmitString(k + " ");
            builder.EmitLine($"void {name}() cil managed");
        }

    }
}
