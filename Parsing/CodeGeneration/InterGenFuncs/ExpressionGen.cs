
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

        //[CodeGenFunction("StringLiteral")]
        //[CodeGenFunction("NumericalLiteral")]
        //[CodeGenFunction("IdentiferExpression")]
        //    public void CompileValueExpression(SyntaxTreeNode node) { }


        private CodeValue ToValue(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "BoolLiteral":
                case "StringLiteral":
                case "NumericalLiteral":
                    return node.Val as CodeValue;

                case "IdentifierExpression":
                    return GetFirst((node.Val as SyntaxTreeNode).ValueString);

                case "Identifier":
                    return GetFirst(node.ValueString);

                case "MemberAccess":
                    return CompileMemberAccess(node);

                default:
                    return null;
            }
        }

        private CodeValue ToIntermediateExpression(SyntaxTreeNode node)
        {
            var value = ToValue(node);

            if (value != null)
                return value;

            if (!_codeGenFunctions.ContainsKey(node.Op.ToLower()))
            {
                return new InterOpValue(new LateStaticReferenceResolver(node));
            }

            var op = CompileNode(node);

            if (op is InterOp) return new InterOpValue(op as InterOp);

            return null;
        }

        public void PushExpression(SyntaxTreeNode node)
            => builder.AddInstruction(CompileNode(node) as InterInst);

        [CodeGenFunction("BinaryExpression")]
        public InterOp CompileBinaryExpression(SyntaxTreeNode node)
        {
            return new InterBinOp(
                new Operator((Operator.OperatorType)Enum.Parse(typeof(Operator.OperatorType), node[2].ValueString)), 
                ToIntermediateExpression(node.Children[0]), 
                ToIntermediateExpression(node.Children[1]));
        }

        [CodeGenFunction("CallExpression")]
        public InterOp CompileCallExpression(SyntaxTreeNode node)
        {
            var ret = CompileCall(node[0], true);
            ret.SetOwner(builder.CurrentMethod);

            return ret;
        }

        [CodeGenFunction("NewExpression")]
        public InterOp CompileNewExpression(SyntaxTreeNode node)
        {
            CodeValue[] parameters = new CodeValue[node[1].Children.Length];

            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = ToIntermediateExpression(node[1][i]);

            return new InterNew(node[0].ValueString, parameters);
        }
        private FieldOrPropertySymbol CompileMemberAccess(SyntaxTreeNode node)
        {
            var name = node[0].ValueString;

            if (node[1].Op == "MemberAccess")
            {
                var next = CompileMemberAccess(node[1]);
                if (next == null) return null;
                return new FieldOrPropertySymbol(next, name);
            }
            else
            {
                var final = GetFirst(node[1].ValueNode.ValueString);
                if (final == null) return null;
                return new FieldOrPropertySymbol(final, name);
            }
        }
    }
}
