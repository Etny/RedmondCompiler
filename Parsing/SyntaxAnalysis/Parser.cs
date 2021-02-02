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

        public void Parse(TokenStream input)
        {
            Stack<ParserStackEntry> stateStack = new Stack<ParserStackEntry>();


            stateStack.Push(new ParserStackEntry(_start, Token.Unknown));

            (ParserAction, object) nextAction()
            {
                if (stateStack.Peek().State.Action.ContainsKey(new Terminal(input.NextToken.Text, false)))
                    return stateStack.Peek().State.Action[new Terminal(input.NextToken.Text, false)];
                else
                    return stateStack.Peek().State.Action[new Terminal(input.NextToken.Type.Name, true)];
            }

            var action = nextAction();

            while (action.Item1 != ParserAction.Accept)
            {

                switch (action.Item1)
                { 
                    case ParserAction.Shift:
                        stateStack.Push(new ParserStackEntry(action.Item2 as ParserState, input.EatToken()));
                        break;

                    case ParserAction.Reduce:
                        var production = action.Item2 as Production;

                        Stack<ParserStackEntry> tempStack = new Stack<ParserStackEntry>(new Stack<ParserStackEntry>(stateStack));

                        for (int i = 0; i < production.Rhs.Length; i++)
                        {
                            stateStack.Pop();
                        }

                        stateStack.Push(new ParserStackEntry(stateStack.Peek().State.Goto[production.Lhs], Token.Unknown));
                        tempStack.Push(stateStack.Peek());

                        if (production.HasAction)
                            production.Action.Invoke(tempStack);

                        break;
                }

                action = nextAction();
            }
        }

    }

    class ParserStackEntry
    {
        public readonly ParserState State;
        public readonly Token Token;
        private readonly Dictionary<string, object> Attributes = new Dictionary<string, object>();

        public ParserStackEntry(ParserState state, Token token)
        {
            State = state;
            Token = token;
        }

        public object GetAttribute(string key)
        {
            if (Attributes.ContainsKey(key)) return Attributes[key];
            else if (key == "val" && Token.Type != TokenType.GetTokenType("Unkown")) return Token.Value;
            return null;
        }

        public void UpdateAttribute(string key, object value)
        {
            if (!Attributes.ContainsKey(key))
                Attributes.Add(key, value);
            else
                Attributes[key] = value;
        }
    }
}
