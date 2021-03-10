﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public override int GetHashCode() => 0;

        public override bool Equals(object obj) => obj is EmptyTerminal;

        protected override IEnumerable<ProductionEntry> _calculateFirst(ImmutableList<NonTerminal> callers) { yield return this; }

    }
}
