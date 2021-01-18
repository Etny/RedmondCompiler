using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Redmond.Parsing.SyntaxAnalysis
{
    abstract class ProductionEntry
    {
        public bool IsTerminal { get => _isTerminal(); }

        public ProductionEntry[] First { get { if (_first == null) _first = _calculateFirst().ToArray(); return _first; } }
        protected ProductionEntry[] _first = null;

        protected abstract bool _isTerminal();

        public abstract override string ToString();
        public abstract override bool Equals(object obj);

        public bool CanFirstBeEmpty()
            => First.ToList().Exists(e => e is EmptyTerminal);

        protected abstract IEnumerable<ProductionEntry> _calculateFirst();

        
    }
}
