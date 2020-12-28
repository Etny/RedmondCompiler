using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Lex.LexCompiler
{
    class DecoratedDFAState : DFAState
    {

        public bool Marked = false;
        public readonly IEnumerable<int> Numbers;

        public DecoratedDFAState(IEnumerable<int> nums, string id = "") : base(id)
        {
            Numbers = nums;
        }

    }
}
