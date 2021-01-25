using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OpType = Redmond.Lex.LexCompiler.RegexTree.OperatorNode.RegexTreeOperator;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class RegexTreeCompiler
    {

        public readonly string[] Lines;

        private const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _ops = "|*:";
        private static readonly Dictionary<char, char> _specialEscapes = new Dictionary<char, char>()
        {
            {'t', '\t'},
            {'r', '\r'},
            {'n', '\n'},
            {'w', ' ' }
        };

        private readonly Dictionary<string, RegexTreeNode> _macros = new Dictionary<string, RegexTreeNode>();


        public RegexTreeCompiler(string[] lines)
            => Lines = lines;
        
        #region Full File Compilation
        public List<(string, RegexTreeNode)> CompileFile(string suffix = "")
        {

            List<(string, RegexTreeNode)> trees = new List<(string, RegexTreeNode)>();

            int mode = 0; // 0 = Macros, 1 = Productions, 2 == Done

            for(int lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
            {
                string line = Lines[lineIndex];

                if(line == "%%") { mode++; continue; }

                string[] split;

                switch (mode)
                {
                    case 0:
                        split = line.Split('=', 2);
                        AddMacro(split[0].Trim(), split[1].Trim().Replace(" ", ""));
                        break;

                    case 1:
                        if (!line.EndsWith('}')) continue;

                        split = new string[] { line.Substring(0, line.LastIndexOf('{')), line.Substring(line.LastIndexOf('{'))[1..^1] };

                        trees.Add((split[1], CompileRegexTree(split[0].Trim().Replace(" ", "") + suffix)));
                        break;

                    default:
                        break;
                }
            }

            return trees;
        }

        private void AddMacro(string key, string regex)
            => _macros.Add(key, CompileRegexTree(regex));
        #endregion

        #region Range Expression Compilation
        private RegexTreeNode CompileCharacterRange(string source, ref int pos)
        {
            RegexTreeNode node;

            string expandedString = "";
            int charCount = source.Length;

            string appendCharacter(string s, string c)
            {
                if (letters.Contains(c))
                    return s + "(" + c + "|";
                else
                    return s + "(\\" + c + "|";
            }

            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];

                if (c == '-' && i > 0 && i < source.Length - 1)
                {
                    char prev = (char)(source[i - 1] + 1);
                    char next = source[i + 1];

                    if (next == '\\' && i < source.Length - 2)
                    {
                        next = ReadEscapedSymbol(source[++i + 1])[0];
                        charCount--;
                    }

                    if (prev > next)
                    {
                        char temp = next;
                        next = prev;
                        prev = temp;
                    }
                    charCount -= 2;
                    charCount += (next - prev) + 1;

                    char current = prev;

                    while (current != next + 1)
                        expandedString = appendCharacter(expandedString, current++ + "");

                    i++;
                }
                else if (c == '\\' && i < source.Length - 1)
                {
                    charCount--;
                    expandedString = appendCharacter(expandedString, ReadEscapedSymbol(source[++i]));
                }
                else
                    expandedString = appendCharacter(expandedString, c + "") ;
                
            }
               
            expandedString = expandedString[0..^1] + new string(')', charCount);

            node = CompileRegexTree(expandedString, ref pos);

            return node;
        }
        #endregion

        #region Single Node Compilation
        private RegexTreeNode CompileRegexNode(string source, ref int pos, ref int index) 
        {
            char c = source[index];
            RegexTreeNode node;

            if (c == '(')
            #region Subexpressions 
            {
                string sub = source.ReadUntilClosingBracket(ref index, '(', ')');
                node = CompileRegexTree(sub, ref pos);
            }
            #endregion
            else if (c == '[')
            #region Range Expressions
            {
                string sub = source.ReadUntilClosingBracket(ref index, '[', ']');
                node = CompileCharacterRange(sub, ref pos);
            }
            #endregion
            else if (_ops.Contains(c))
            #region Operators
                node = new OperatorNode(c == '*' ? OpType.Star : (c == '|' ? OpType.Or : OpType.Concat));
            #endregion
            else if (c == 'ε')
            #region Empty Nodes
                node = new EmptyNode(pos++);
            #endregion
            else if(c == '/')
            #region Read Ahead
            {
                index++;
                node = CompileRegexNode(source, ref pos, ref index);
                node.MarkedAsJumpahead = true;
                index--;
            }
            #endregion
            else if (c == '\\')
            #region Escaped Symbols
            {
                if (index >= source.Length - 1) node = new SymbolNode(pos++, c + "");
                else
                    node = new SymbolNode(pos++, ReadEscapedSymbol(source[++index]));
            }
            #endregion
            else
            #region Symbols and Macros
            {
                List<string> matchingMacros = _macros.Keys.ToList();

                int localIndex = index;
                int i = 0;
                while (localIndex + i < source.Length)
                {
                    var match = matchingMacros.FindAll(s => i < s.Length && s[i] == source[localIndex + i]);
                    if (match.Count <= 0) break;
                    matchingMacros = match;
                    i++;
                }

                if (i != 0 && matchingMacros.Exists(s => s.Length <= i))
                {
                    matchingMacros = matchingMacros.FindAll(s => s.Length <= i);
                    var longestMatch = matchingMacros[matchingMacros.FindIndex(s => s.Length == matchingMacros.Max(s => s.Length))];
                    index += longestMatch.Length - 1;
                    //node = CompileRegexTree(_macros[longestMatch], ref pos);
                    node = _macros[longestMatch].Clone();
                    node.SetStartingPosition(ref pos);
                }
                else
                    node = new SymbolNode(pos++, c + "");
            }
            #endregion

            index++;
            return node;
        }
        #endregion

        #region Full Tree Compilation
        public RegexTreeNode CompileRegexTree(string source)
        {
            int i = 1;
            return CompileRegexTree(source, ref i);
        }

        public RegexTreeNode CompileRegexTree(string source, ref int pos, int index = 0)
        {
            RegexTreeNode root = null;
            OperatorNode opNode = null;


            while(index < source.Length)
            {
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
                        var rootClone = root.Clone();
                        rootClone.SetStartingPosition(ref pos);
                        concatNode.AddChild(root, 0);
                        concatNode.AddChild(starNode, 1);
                        starNode.AddChild(rootClone);
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
        #endregion

        private string ReadEscapedSymbol(char escaped)
        {
            string symbol = escaped + "";

            if (_specialEscapes.Keys.Contains(escaped))
                symbol = _specialEscapes[escaped] + "";

            return symbol;
        }

    }
}
