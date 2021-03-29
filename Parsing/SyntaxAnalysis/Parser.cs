using Redmond.Lex;
using Redmond.Output.Error;
using Redmond.Output.Error.Exceptions;
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

            ParserAction nextAction()
            {
                try
                {
                    if (stateStack.Peek().State.Action.ContainsKey(new Terminal(input.NextToken.Text, false)))
                        return stateStack.Peek().State.Action[new Terminal(input.NextToken.Text, false)];
                    else
                        return stateStack.Peek().State.Action[new Terminal(input.NextToken.Type.Name, true)];
                } 
                catch
                {
                    ErrorManager.ExitWithError(new ParserActionNotFoundException(stateStack.Peek().State, input.NextToken));
                }
                return new AcceptAction();
            }

            var action = nextAction();

            while (!(action is AcceptAction))
            {
                //if (input.NextToken.Text == "Console")
                //    Console.WriteLine("aaaa");

                switch (action)
                { 
                    case ShiftAction shift:
                        stateStack.Push(new ParserStackEntry(shift.ShiftState, input.EatToken()));
                        break;

                    case ReduceAction reduce:
                        Stack<ParserStackEntry> tempStack = new Stack<ParserStackEntry>(new Stack<ParserStackEntry>(stateStack));

                        for (int i = 0; i < reduce.PopCount; i++)
                            stateStack.Pop();

                        stateStack.Push(new ParserStackEntry(stateStack.Peek().State.Goto[reduce.Goto], Token.Unknown));
                        tempStack.Push(stateStack.Peek());

                        reduce.Action?.Invoke(tempStack);

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
            else if (Token.Type != TokenType.GetTokenType("Unkown") && Token.Values.ContainsKey(key)) return Token.Values[key];
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
