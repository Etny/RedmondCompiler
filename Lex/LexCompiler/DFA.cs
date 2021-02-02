using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    class DFA
    {
        public readonly DFAState Start;
        public readonly string Alphabet;
        public readonly string Name;
        public DFAState CurrentState;

        public readonly LexAction Action;

        public int LastJumpAhead = -1;
        private int _currentLength = 0;

        private List<DFAState> _fullDfa = null;

        public List<DFAState> FullDFA { get { if (_fullDfa == null) _fullDfa = Start.GetFullDFA(); return _fullDfa; } }

        public DFA(DFAState startState, string alphabet, string name = "", LexAction action = null)
        {
            Start = startState;
            CurrentState = Start;
            Alphabet = alphabet;
            Name = name;
            Action = action;
        }

        public bool Progress(char c)
        {
            if (!CurrentState.Transitions.ContainsKey(c)) return false;
            CurrentState = CurrentState.Transitions[c];
            _currentLength++;
            if (CurrentState.MarkedAsJumpahead) LastJumpAhead = _currentLength;
            return true;
        }

        public void Reset()
        {
            CurrentState = Start;
            LastJumpAhead = -1;
            _currentLength = 0;
        }
    }
}
