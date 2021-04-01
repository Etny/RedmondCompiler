using Redmond.Lex;
using Redmond.IO.Error;
using Redmond.IO.Error.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class Parser
    {

        private readonly ParserState _start;
        private readonly List<ParserState> _states;

        public Parser(List<ParserState> states)
        {
            _start = states[0];
            _states = states;
        }

        public void Parse(TokenStream input)
        {
            Stack<ParserStackEntry> stateStack = new Stack<ParserStackEntry>();


            stateStack.Push(new ParserStackEntry(_start, Token.Unknown));

            ParserAction nextAction()
            {
                try
                {
                    if (stateStack.Peek().State.Action.ContainsKey(new Terminal(input.NextToken.Text, false).ID))
                        return stateStack.Peek().State.Action[new Terminal(input.NextToken.Text, false).ID];
                    else
                        return stateStack.Peek().State.Action[new Terminal(input.NextToken.Type, true).ID];
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
                        stateStack.Push(new ParserStackEntry(_states[shift.StateIndex], input.EatToken()));
                        break;

                    case ReduceAction reduce:
                        Stack<ParserStackEntry> tempStack = new Stack<ParserStackEntry>(new Stack<ParserStackEntry>(stateStack));

                        for (int i = 0; i < reduce.PopCount; i++)
                            stateStack.Pop();

                        stateStack.Push(new ParserStackEntry(_states[stateStack.Peek().State.Goto[reduce.GotoID]], Token.Unknown));
                        tempStack.Push(stateStack.Peek());

                        if(reduce.Action != null)
                            reduce.Action.Invoke(tempStack);

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
            else if (Token.Type != "Unkown" && Token.Values.ContainsKey(key)) return Token.Values[key];
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
