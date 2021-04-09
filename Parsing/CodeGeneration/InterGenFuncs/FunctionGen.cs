using Redmond.Parsing.CodeGeneration.IntermediateCode;
using Redmond.Parsing.CodeGeneration.IntermediateCode.IntermediateInstructions;
using Redmond.Parsing.CodeGeneration.SymbolManagement;
using Redmond.Parsing.SyntaxAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            PushNewTable();

            foreach (var child in node.Children)
                CompileNode(child);

            Tables.Pop();
        }

        [CodeGenFunction("File")]
        [CodeGenFunction("ImportList")]
        [CodeGenFunction("ClassList")]
        [CodeGenFunction("MemberList")]
        public void CompileChildren(SyntaxTreeNode node)
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
            CodeValue[] parameters = new CodeValue[node[1].Children.Length];

            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = ToIntermediateExpression(node[1][i]);

            CodeValue thisPtr = null;
            string functionName = "";
            bool baseAccess = false;

            switch (node[0].Op)
            {
                case "QualifiedIdentifier":
                case "MemberAccess":
                    if (node[0].Children.Length > 1)
                    {
                        functionName = node[0][1].ValueString;
                        thisPtr = ToIntermediateExpression(node[0][0]);
                    }
                    else functionName = node[0][0].ValueString;
                    break;

                case "BaseAccess":
                    functionName = node[0][0].ValueString;
                    baseAccess = true;
                    break;

                default:
                    thisPtr = ToIntermediateExpression(node[0]);
                    break;
            }

            var call = new InterCall(functionName, parameters, exp, thisPtr, baseAccess);

            return call;
        }

        [CodeGenFunction("Function")]
        public void CompileFunction(SyntaxTreeNode node)
        {
            MatchNode(node.Children[0], "FunctionDec");

            if(node.Children.Length > 1)
                MatchNode(node.Children[1], "FunctionBody");

            Tables.Pop();
        }

        [CodeGenFunction("Constructor")]
        public void CompileConstructor(SyntaxTreeNode node)
        {
            MatchNode(node.Children[0], "ConstructorDec");

            MatchNode(node.Children[1], "FunctionBody");

            Tables.Pop();
        }

        [CodeGenFunction("FunctionBody")]
        public void CompileFunctionBody(SyntaxTreeNode node)
        {
            if(node[0].Op == "FunctionBodyStatement" && builder.CurrentMethod.ReturnTypeName != new TypeName("void"))
                builder.AddInstruction(new InterRet(ToIntermediateExpression(node[0][0])));
            else
                CompileNodes(node.Children);
        }

        [CodeGenFunction("FunctionDec")]
        public void CompileFunctionDeclaration(SyntaxTreeNode node)
        {
            string name = node[1].ValueString;
            TypeName retType;
            ArgumentSymbol[] args = CompileParameterDecList(node[2]);
            List<string> functionKeywords = new List<string>();

            var decHeader = node[0];
            functionKeywords.Add(decHeader[0].ValueString);
            foreach (var mod in decHeader[1].Children) functionKeywords.Add(mod.ValueString);
            retType = TypeNameFromNode(decHeader[2]);

            PushNewTable();

            builder.AddMethod(name, retType, args, functionKeywords);
        }

        [CodeGenFunction("ConstructorDec")]
        public void CompileConstructorDeclaration(SyntaxTreeNode node)
        {
            List<string> functionKeywords = new List<string>();
            ArgumentSymbol[] args = CompileParameterDecList(node[1]);
            var decHeader = node[0];

            functionKeywords.Add(decHeader[0].ValueString);
            Debug.Assert(decHeader[1].Children.Length < 1);
            //Debug.Assert(TypeNameFromNode(decHeader[2]) == builder.CurrentType.Name);

            PushNewTable();

            bool callThis = node[2].Op == "ThisInitializer";

            var con = builder.AddConstructor(args, functionKeywords, callThis);

            CodeValue[] baseArgs = new CodeValue[node[2][0].Children.Length];
            for (int i = 0; i < baseArgs.Length; i++) baseArgs[i] = ToIntermediateExpression(node[2][0][i]);
            con.SetBaseArgs(baseArgs);
        }

        private ArgumentSymbol[] CompileParameterDecList(SyntaxTreeNode node)
        {
            ArgumentSymbol[] args = new ArgumentSymbol[node.Children.Length];

            for (int i = 0; i < args.Length; i++)
                args[i] = new ArgumentSymbol(node[i][1].ValueString, TypeNameFromNode(node[i][0]), i);

            return args;
        }

    }
}
