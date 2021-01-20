using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class EmptyTerminal : ProductionEntry
    {
        public EmptyTerminal()
        { }

        protected override bool _isTerminal() => true;
        protected override bool _canBeEmpty() => true;

        public override string ToString() => "\'Empty\'";


        public override bool Equals(object obj) => obj is EmptyTerminal;

        protected override IEnumerable<ProductionEntry> _calculateFirst() { yield return this; }

    }
}
