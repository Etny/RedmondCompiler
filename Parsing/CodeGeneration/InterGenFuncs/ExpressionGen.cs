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


        private bool IsValue(SyntaxTreeNode node)
            => ToValue(node) != null;

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

                case "MemberAccessExpression":
                    return ToValue(node[0]);

                case "MemberAccess":
                    var value = ToValue(node[1]);
                    if (value == null) return null;
                    return null;

                default:
                    return null;
            }
        }

        private InterInstOperand ToIntermediateExpression(SyntaxTreeNode node)
            => IsValue(node) ? new InterInstOperand(ToValue(node)) : new InterInstOperand(CompileNode(node) as InterOp);

        public void PushExpression(SyntaxTreeNode node)
            => builder.AddInstruction(CompileNode(node) as InterInst);

        [CodeGenFunction("BinaryExpression")]
        public InterOp CompileBinaryExpression(SyntaxTreeNode node)
            => new InterBinOp(node[2].ValueString, ToIntermediateExpression(node.Children[0]), ToIntermediateExpression(node.Children[1]));

        [CodeGenFunction("CallExpression")]
        public InterOp CompileCallExpression(SyntaxTreeNode node)
        {
            var ret = CompileCall(node[0], true);
            ret.SetOwner(builder.CurrentMethod);

            return ret;
        }

    }
}
