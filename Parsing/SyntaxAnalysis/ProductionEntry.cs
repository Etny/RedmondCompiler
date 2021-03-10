using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Redmond.Parsing.SyntaxAnalysis
{
    abstract class ProductionEntry
    {
        public bool IsTerminal { get => _isTerminal(); }

        public bool IsEmptyTerminal { get => this is EmptyTerminal; }
        public bool CanBeEmpty { get => _canBeEmpty(); }
        public bool CanFirstBeEmpty { get; protected set; } = false;



        public ProductionEntry[] First { get { if (_first == null) CalculateFirst(); return _first; } }
        protected ProductionEntry[] _first = null;

        public ProductionEntry[] Follow { get { Debug.Assert(!IsTerminal); if (_follow == null) _finalizeFollow(); return _follow; } }
        protected ProductionEntry[] _follow = null;
        protected List<IEnumerable<ProductionEntry>> _followGroups = new List<IEnumerable<ProductionEntry>>();
        protected List<NonTerminal> _followFollow = new List<NonTerminal>();


        public void AddToFollow(IEnumerable<ProductionEntry> e) => _followGroups.Add(e);
        public void AddToFollow(ProductionEntry e) => AddToFollow(new ProductionEntry[] { e });
        public void AddFollowToFollow(NonTerminal e) { if (!e.Equals(this)) _followFollow.Add(e); }


        protected abstract bool _isTerminal();
        protected abstract bool _canBeEmpty();

        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();

        public static ProductionEntry[] GetFirstOfSet(ProductionEntry[] prods, int startIndex = 0)
        {
            List<ProductionEntry> first = new List<ProductionEntry>();

            int i = startIndex;
            for(; i < prods.Length; i++)
            {
                bool containsEmpty = false;

                foreach (var p in prods[i].First)
                    if (!p.IsEmptyTerminal) 
                        first.Add(p);
                    else
                        containsEmpty = true;

                if (!containsEmpty) break;
            }

            if (i == prods.Length && prods[i - 1].CanFirstBeEmpty)
                first.Add(new EmptyTerminal());

            return first.ToArray();
        }

        private void _finalizeFollow()
        {
            List<ProductionEntry> follow = new List<ProductionEntry>();

            void addSet(IEnumerable<ProductionEntry> e)
            {
                foreach (var p in e)
                    if (!follow.Exists(pp => pp.Equals(p)) && !p.IsEmptyTerminal)
                        follow.Add(p);
            }

            foreach (var e in _followGroups)
                addSet(e);

            foreach (var f in _followFollow)
                addSet(f.Follow);

            _follow = follow.ToArray();
        }

        protected abstract IEnumerable<ProductionEntry> _calculateFirst(ImmutableList<NonTerminal> callers);


        public void CalculateFirst()
            => CalculateFirst(ImmutableList<NonTerminal>.Empty);

        public void CalculateFirst(ImmutableList<NonTerminal> callers)
        {
            if (_first == null)
                _first = _calculateFirst(callers).ToArray();
        }
        public virtual void CalculateFollows() { }
    }
}
