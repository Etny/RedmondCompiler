using System;
using System.Collections.Generic;
using System.Text;
using OpType = Redmond.Lex.LexCompiler.RegexTree.OperatorNode.RegexTreeOperator;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class RegexTreeCompiler
    {

        private static string _ops = "|*+";

        public static RegexTreeNode CompileRegexTree(string source)
        {
            int i = 1;
            return CompileRegexTree(source, ref i);
        }

        private static RegexTreeNode CompileRegexNode(string source, ref int pos, ref int index) 
        {
            char c = source[index];
            RegexTreeNode node;

            if(c == '(')
            {
                int brackets = 1;
                string sub = "";
                char n = '\0';
                while (brackets > 0)
                {
                    if(n != '\0') sub += n;
                    n = source[++index];
                    if (n == ')') brackets--;
                    else if (n == '(') brackets++;
                }
                node = CompileRegexTree(sub, ref pos);
            }else if (_ops.Contains(c))
                node = new OperatorNode(c == '*' ? OpType.Star : (c == '|' ? OpType.Or : OpType.Concat));
            else
                node = new SymbolNode(pos++, source.ReadUntil(ref index, c => _ops.Contains(c) || c == '('));

            index++;
            return node;
        }

        public static RegexTreeNode CompileRegexTree(string source, ref int pos, int index = 0)
        {
            RegexTreeNode root = null;
            OperatorNode opNode = null;

            while(index < source.Length)
            {
                root = CompileRegexNode(source, ref pos, ref index);

                while (index < source.Length && source[index] == '*')
                {
                    var starNode = CompileRegexNode(source, ref pos, ref index);
                    starNode.AddChild(root, 0);
                    root = starNode;
                }

                if (opNode != null) opNode.AddChild(root, 1);

                if (index >= source.Length) break;

                var op = CompileRegexNode(source, ref pos, ref index) as OperatorNode;
                if (op == null) throw new Exception("Expected Operator Node");

                op.AddChild(opNode == null ? root : opNode, 0);
                opNode = op;

            }

            return opNode ?? root;
        }

 
    }
}
