using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpType = Redmond.Lex.LexCompiler.RegexTree.OperatorNode.RegexTreeOperator;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class RegexTreeCompiler
    {

        private const string _ops = "|*:";

        private Dictionary<string, string> _macros = new Dictionary<string, string>();

        public RegexTreeCompiler(string path)
        {
            _macros.Add("letter", "[abcdefghijklmnopqrstuvwxyz]");
        }


        public RegexTreeNode CompileRegexTree(string source)
        {
            int i = 1;
            return CompileRegexTree(source, ref i);
        }

        private RegexTreeNode CompileCharacterRange(string source, ref int pos)
        {
            RegexTreeNode node;

            string expandedString = "";

            foreach (char c in source)
                expandedString += "(" + c + "|";
            expandedString = expandedString[0..^1] + new string(')', source.Length);

            node = CompileRegexTree(expandedString, ref pos);

            return node;
        }

        private RegexTreeNode CompileRegexNode(string source, ref int pos, ref int index) 
        {
            char c = source[index];
            RegexTreeNode node;

            if (c == '(')
            {
                int brackets = 1;
                string sub = "";
                char n = '\0';
                while (brackets > 0)
                {
                    if (n != '\0') sub += n;
                    n = source[++index];
                    if (n == ')') brackets--;
                    else if (n == '(') brackets++;
                }
                node = CompileRegexTree(sub, ref pos);
            }
            else if(c == '[')
            {
                index++;
                string s = source.ReadUntil(ref index, c => c == ']');
                node = CompileCharacterRange(s, ref pos);
                index++;
            }
            else if (_ops.Contains(c))
                node = new OperatorNode(c == '*' ? OpType.Star : (c == '|' ? OpType.Or : OpType.Concat));
            else if (c == 'ε')
                node = new EmptyNode(pos++);
            else
            {
                List<string> matchingMacros = new List<string>();

                int localIndex = index;
                int i = 0;
                while(true)
                {
                    var match = _macros.Keys.ToList().FindAll(s => i < s.Length && s[i] == source[localIndex + i]);
                    if (match.Count <= 0) break;
                    matchingMacros = match;
                    i++;
                }

                if(matchingMacros.Exists(s => s.Length <= i))
                {
                    matchingMacros = matchingMacros.FindAll(s => s.Length <= i);
                    var longestMatch = matchingMacros[matchingMacros.FindIndex(s => s.Length == matchingMacros.Max(s => s.Length))];
                    index += longestMatch.Length-1;
                    node = CompileRegexTree(_macros[longestMatch], ref pos);
                }
                else
                    node = new SymbolNode(pos++, c+"");
            }

            index++;
            return node;
        }

        public RegexTreeNode CompileRegexTree(string source, ref int pos, int index = 0)
        {
            RegexTreeNode root = null;
            OperatorNode opNode = null;

            while(index < source.Length)
            {
                int startIndex = index;
                root = CompileRegexNode(source, ref pos, ref index);

                while (index < source.Length)
                {
                    if (source[index] == '*')
                    {
                        var starNode = CompileRegexNode(source, ref pos, ref index);
                        starNode.AddChild(root, 0);
                        root = starNode;
                    }
                    else if (source[index] == '+')
                    {
                        var concatNode = new OperatorNode(OpType.Concat);
                        var starNode = new OperatorNode(OpType.Star);
                        concatNode.AddChild(root, 0);
                        concatNode.AddChild(starNode, 1);
                        starNode.AddChild(CompileRegexNode(source, ref pos, ref startIndex));
                        root = concatNode;
                        index++;
                    }
                    else break;
                }

                if (opNode != null) opNode.AddChild(root, 1);

                if (index >= source.Length) break;

                if (!_ops.Contains(source[index]))
                    source = source[0..index] + ":" + source[index..(source.Length)];

                var op = CompileRegexNode(source, ref pos, ref index) as OperatorNode;
                if (op == null) throw new Exception("Expected Operator Node");

                op.AddChild(opNode == null ? root : opNode, 0);
                opNode = op;

            }

            return opNode ?? root;
        }

 
    }
}
