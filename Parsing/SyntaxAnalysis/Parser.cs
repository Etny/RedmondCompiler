using Redmond.Lex;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Parser
    {

        private readonly ParserState _start;

        public Parser(ParserState startState)
        {
            _start = startState;
        }

        public ParseTreeNode Parse(TokenStream input)
        {
            ParseTreeNode root = null;

            Stack<ParserState> stateStack = new Stack<ParserState>();
            Stack<ParseTreeNode> treeStack = new Stack<ParseTreeNode>();


            stateStack.Push(_start);

            var action = stateStack.Peek().Action[new Terminal(input.NextToken.Text)];

            while (action.Item1 != ParserAction.Accept)
            {

                switch (action.Item1)
                { 
                    case ParserAction.Shift:
                        stateStack.Push(action.Item2 as ParserState);
                        treeStack.Push(new ParseTreeNode(new Terminal(input.NextToken.Text)));
                        input.EatToken();
                        break;

                    case ParserAction.Reduce:
                        var production = action.Item2 as Production;
                        var newNode = new ParseTreeNode(production.Lhs);

                        for (int i = 0; i < production.Rhs.Length; i++)
                        {
                            stateStack.Pop();
                            newNode.AddChild(treeStack.Pop(), production.Rhs.Length - i - 1);
                        }

                        treeStack.Push(newNode);

                        stateStack.Push(stateStack.Peek().Goto[production.Lhs]);
                        break;
                }

                action = stateStack.Peek().Action[new Terminal(input.NextToken.Text)];
            }

            return treeStack.Peek();
        }

    }
}
