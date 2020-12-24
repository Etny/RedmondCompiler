using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    class DFAState
    {
        public ImmutableDictionary<char, DFAState> Transitions = ImmutableDictionary<char, DFAState>.Empty;
        public bool Marked = false;
        public readonly IEnumerable<int> Numbers;

        public DFAState(IEnumerable<int> nums)
        {
            Numbers = nums;
        }

        public void AddTransition(char key, DFAState state)
            => Transitions = Transitions.Add(key, state);
    }
}
