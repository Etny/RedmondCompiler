
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
        private CodeValue ToValue(SyntaxTreeNode node)
        {
            switch (node.Op)
            {
                case "BoolLiteral":
                case "CharLiteral":
                case "StringLiteral":
                case "IntLiteral":
                case "RealLiteral":
                    return node.Val as CodeValue;

                case "NullLiteral":
                    return new NullValue();

                case "IdentifierExpression":
                    return GetFirst((node.Val as SyntaxTreeNode).ValueString);

                case "CastExpression":
                    return new ConvertedValue(ToValue(node[1]), TypeNameFromNode(node[0]), builder.CurrentMethod);

                case "Identifier":
                    return GetFirst(node.ValueString);

                case "QualifiedIdentifier":
                case "Qualifier":
                    return CompileFieldOrPropertyReference(node);

                case "MemberAccess":
                    return new FieldOrPropertySymbol(ToIntermediateExpression(node[0]), node[1].ValueString);

                case "BaseAccess":
                    return new FieldOrPropertySymbol(builder.CurrentMethod.ThisPointer, builder.CurrentType.BaseTypeName, node[0].ValueString);

                case "ArrayAccessExpression":
                    return new ArrayEntryValue(ToIntermediateExpression(node[0]), ToIntermediateExpression(node[1]));

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
                return new InterOpValue(new LateStaticReferenceResolver(node, builder.CurrentType.NamespaceContext), builder.CurrentMethod);
            }

            var op = CompileNode(node);

            if (op is InterOp) return new InterOpValue(op as InterOp, builder.CurrentMethod);

            return null;
        }

        public void PushExpression(SyntaxTreeNode node)
            => builder.AddInstruction(CompileNode(node) as InterInst);

        [CodeGenFunction("BinaryExpression")]
        [CodeGenFunction("BinaryBoolExpression")]
        public InterOp CompileBinaryExpression(SyntaxTreeNode node)
        {
            var op = new InterBinOp(
                Operator.FromName(node[2].ValueString), 
                ToIntermediateExpression(node.Children[0]), 
                ToIntermediateExpression(node.Children[1]));

            if (node.Op == "BinaryBoolExpression") op.IsBooleanExpression = true;

            return op;
        }

        [CodeGenFunction("TernaryExpression")]
        public InterOp CompileTernaryExpression(SyntaxTreeNode node)
        {
            var op1 = ToIntermediateExpression(node[1]);
            var op2 = ToIntermediateExpression(node[2]);

            var bin = CompileBinaryExpression(node[0]);

            return new InterTernary(op1, op2, bin); ;
        }

        [CodeGenFunction("BoolCheckExpression")]
        public InterOp CompileBinaryCheckExpression(SyntaxTreeNode node)
        {
            var op1 = ToIntermediateExpression(node[0]);
            var op2 = ToIntermediateExpression(node[1]);

            InterBinCheck check = new InterBinCheck(op1, op2, Operator.FromName(node[2].ValueString));
            check.SetOwner(builder.CurrentMethod);
            //check.SetLabel(builder.CurrentMethod.NextLabel);

            return check;
        }

        [CodeGenFunction("PostIncOrDec")]
        public InterOp CompilePostIncrementOrDecrementExpression(SyntaxTreeNode node)
        {
            var op1 = ToIntermediateExpression(node[0]);
            var op2 = new CodeValue(CodeType.Int32, 1);
            var op =  new InterBinOp(Operator.FromName(node[1].ValueString), op1, op2);
            var copy = new InterCopy((CodeSymbol)ToIntermediateExpression(node[0]), new InterOpValue(op, builder.CurrentMethod));
            copy.SetOwner(builder.CurrentMethod);
            return copy;
        }

        [CodeGenFunction("AssignExpression")]
        public InterOp CompileAssignExpression(SyntaxTreeNode node)
        {
            CodeSymbol symbol = ToIntermediateExpression(node[0]) as CodeSymbol;

            InterCopy copy;

            if (symbol == null)
                copy = new InterCopy(new LateStaticReferenceResolver(node[0], builder.CurrentType.NamespaceContext), ToIntermediateExpression(node[1]));
            else 
                 copy = new InterCopy(symbol, ToIntermediateExpression(node[1]));

            copy.SetOwner(builder.CurrentMethod);
            return copy;
        }

        [CodeGenFunction("StatementExpressionList")]
        public void CompileStatementExpressionList(SyntaxTreeNode node)
        {
            foreach (var c in node.Children)
                builder.AddInstruction(new InterPush(ToIntermediateExpression(c)));
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

            return new InterNew(TypeNameFromNode(node[0]), parameters);
        }

        [CodeGenFunction("NewArrayExpression")]
        public InterOp CompileNewArrayExpression(SyntaxTreeNode node)
        {
            if(node.Children.Length == 1)
            {
                CodeValue[] entries = new CodeValue[node[0].Children.Length];

                for (int i = 0; i < entries.Length; i++)
                    entries[i] = ToIntermediateExpression(node[0][i]);

                return new InterNewArray(entries);
                
            }

            TypeName type = TypeNameFromNode(node[0]);

            if(node[1].Op == "ParameterList")
            {
                CodeValue[] entries = new CodeValue[node[1].Children.Length];

                for (int i = 0; i < entries.Length; i++)
                    entries[i] = ToIntermediateExpression(node[1][i]);

                return new InterNewArray(type, entries);
            }

            return new InterNewArray(type, ToIntermediateExpression(node[1]));
        }

        private CodeValue CompileFieldOrPropertyReference(SyntaxTreeNode node)
        {
            if (node.Children.Length > 1)
            {
                var next = CompileFieldOrPropertyReference(node[0]);
                if (next == null) return null;
                return new FieldOrPropertySymbol(next, node[1].ValueString);
            }
            else
                return GetFirst(node[0].ValueString);
            
        }
    }
}
