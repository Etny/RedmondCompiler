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

            if (node.Children.Length > 2)
            {
                var member = ToIntermediateExpression(node[2]);

                if (member == null)
                    return new InterCall(node[0].ValueString, parameters, exp, new LateStaticReferenceResolver(node[2]));

                thisPtr = member;

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
            CompileNodes(node.Children);
        }

        [CodeGenFunction("FunctionDec")]
        public void CompileFunctionDeclaration(SyntaxTreeNode node)
        {
            string name = node[1].ValueString;
            string retType;
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
            Debug.Assert(decHeader[2].ValueString == builder.CurrentType.Name);

            PushNewTable();

            builder.AddConstructor(args, functionKeywords);
        }

        private ArgumentSymbol[] CompileParameterDecList(SyntaxTreeNode node)
        {
            ArgumentSymbol[] args = new ArgumentSymbol[node.Children.Length];

            for (int i = 0; i < args.Length; i++)
                args[i] = new ArgumentSymbol(node[i][1].ValueString, node[i][0].ValueString, i);

            return args;
        }

    }
}
