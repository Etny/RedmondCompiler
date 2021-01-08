using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    class DFAState
    {
        public ImmutableDictionary<char, DFAState> Transitions = ImmutableDictionary<char, DFAState>.Empty;
        public readonly string ID;

        public bool IsStartingState = false, IsAcceptingState = false;
        public bool MarkedAsJumpahead = false;

        public DFAState(string id = "")
        {
            ID = id;
        }

        public List<DFAState> GetFullDFA(List<DFAState> states = null)
        {
            if (states == null) states = new List<DFAState>();

            states.Add(this);

            foreach(var s in Transitions.Values)
                if(!states.Contains(s)) states = s.GetFullDFA(states);

            return states;
        }

        public void AddTransition(char key, DFAState state)
            => Transitions = Transitions.Add(key, state);
    }
}
