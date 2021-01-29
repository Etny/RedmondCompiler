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
            Stack<ParserStackEntry> stateStack = new Stack<ParserStackEntry>();
            Stack<ParseTreeNode> treeStack = new Stack<ParseTreeNode>();


            stateStack.Push(new ParserStackEntry(_start));

            var action = stateStack.Peek().State.Action[new Terminal(input.NextToken.Text)];

            while (action.Item1 != ParserAction.Accept)
            {

                switch (action.Item1)
                { 
                    case ParserAction.Shift:
                        stateStack.Push(new ParserStackEntry(action.Item2 as ParserState));
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
                        stateStack.Push(new ParserStackEntry(stateStack.Peek().State.Goto[production.Lhs]));

                        if (production.HasAction)
                            production.Action.Invoke(stateStack);

                        break;
                }

                action = stateStack.Peek().State.Action[new Terminal(input.NextToken.Text)];
            }

            return treeStack.Peek();
        }

    }

    class ParserStackEntry
    {
        public readonly ParserState State;
        public readonly Dictionary<string, Object> Attributes = new Dictionary<string, object>();

        public ParserStackEntry(ParserState state)
        {
            State = state;
        }
    }
}
