using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class RegexTreeCompiler
    {

        private static string _ops = "|*+";

        public static RegexTreeNode CompileRegexTree(string source, int index = 0, RegexTreeNode leaf = null)
        {
            int i = 1;
            return CompileRegexTree(source, ref i, index, leaf);
        }

        public static RegexTreeNode CompileRegexTree(string source, ref int pos, int index = 0, RegexTreeNode leaf = null)
        {
            char c = source[index];
            RegexTreeNode node;

            if (_ops.Contains(c))
            {
                if (c == '*')
                    node = new OperatorNode(OperatorNode.RegexTreeOperator.Star);
                else
                {
                    node = new OperatorNode(c == '|' ? OperatorNode.RegexTreeOperator.Or : OperatorNode.RegexTreeOperator.Concat);

                    string sub;
                    if (source[++index] == '(')
                    {
                        sub = source.ReadWhile(++index, c => c != ')');
                        index++;
                    }
                    else
                        sub = source.ReadWhile(index, c => !_ops.Contains(c));

                    var subNode = CompileRegexTree(sub, ref pos);

                    index += sub.Length - 1;
                    if (index + 1 < source.Length && source[index+1] == '*')
                    {
                        var n = new OperatorNode(OperatorNode.RegexTreeOperator.Star);
                        n.AddChild(subNode, 0);
                        subNode = n;
                        index++;
                    }

                    node.AddChild(subNode, 1);
                }
                node.AddChild(leaf, 0);
            }
            else if (c == '(')
            {
                string sub = source.ReadUntil(++index, c => c == ')');
                node = CompileRegexTree(sub, ref pos);
                index+=sub.Length; // sub.Length - 1 plus 1 for the closing )
            }
            else
                node = new SymbolNode(pos++, source.ReadUntil(ref index, c => _ops.Contains(c) || c == '('));

            if (++index >= source.Length)
                return node;

            return CompileRegexTree(source, ref pos, index, node);
        }
    }
}
