using Redmond.Output;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace Redmond.Lex.LexCompiler.RegexTree
{
    class OperatorNode : RegexTreeNode
    {
        public readonly RegexTreeOperator Operator;


        public OperatorNode(RegexTreeOperator op) { Operator = op; }

        public override void Print(IStringStream output)
        {
            output *= $"Operator {Operator} with child(ren):";
            output.AddIndentation();
            foreach (var node in Children)
                if(node != null) node.Print(output);
            output.ReduceIndentation();
        }

        public override IEnumerable<int> FirstPositions()
        {
            switch (Operator)
            {
                case RegexTreeOperator.Concat:

                    if (Children[0].Nullable())
                        return YieldCollection(Children[0].FirstPositions(), Children[1].FirstPositions());
                    else
                        return YieldCollection(Children[0].FirstPositions());

                case RegexTreeOperator.Or:
                    return YieldCollection(Children[0].FirstPositions(), Children[1].FirstPositions());


                case RegexTreeOperator.Star:
                    return YieldCollection(Children[0].FirstPositions());


                default:
                    return new List<int>();
            }
        }

        public override IEnumerable<int> LastPositions()
        {
            switch (Operator)
            {
                case RegexTreeOperator.Concat:

                    if (Children[1].Nullable())
                        return YieldCollection(Children[0].LastPositions(), Children[1].LastPositions());
                    else
                        return YieldCollection(Children[1].LastPositions());

                case RegexTreeOperator.Or:
                    return YieldCollection(Children[0].LastPositions(), Children[1].LastPositions());

                case RegexTreeOperator.Star:
                    return YieldCollection(Children[0].LastPositions());

                default:
                    return new List<int>();
            }
        }

        public override IEnumerable<int> FollowingPositions(int index)
        {
            IEnumerable<int> addition = new List<int>();
            switch (Operator)
            {
                case RegexTreeOperator.Concat:
                    if (Children[0].LastPositions().Contains(index))
                        addition = Children[1].FirstPositions();
                    return YieldCollection(Children[0].FollowingPositions(index), Children[1].FollowingPositions(index), addition);

                case RegexTreeOperator.Star:
                    if (Children[0].LastPositions().Contains(index))
                        addition = Children[0].FirstPositions();
                    return YieldCollection(Children[0].FollowingPositions(index), addition);

                default:
                    return YieldCollection(Children[0].FollowingPositions(index), Children[1].FollowingPositions(index));
            }

        }

        private IEnumerable<int> YieldCollection(params IEnumerable<int>[] cols)
        {
            foreach (var col in cols)
                foreach (int i in col)
                    yield return i;
        }

        public override RegexTreeNode Clone()
        {
            OperatorNode clone = new OperatorNode(Operator);

            for (int i = 0; i < 2; i++)
                if (Children[i] != null)
                    clone.AddChild(Children[i].Clone(), i);

            return clone;
        }

        public override void SetStartingPosition(ref int startPos)
        {
            Children[0].SetStartingPosition(ref startPos);

            if (Operator != RegexTreeOperator.Star)
                Children[1].SetStartingPosition(ref startPos);
        }

        public override bool Nullable()
        {
            return Operator switch
            {
                RegexTreeOperator.Concat => Children[0].Nullable() && Children[1].Nullable(),
                RegexTreeOperator.Or => Children[0].Nullable() || Children[1].Nullable(),
                RegexTreeOperator.Star => true,
                _ => true,
            };
        }

        public override SymbolNode GetNodeAtIndex(int index)
        {
            switch (Operator)
            {
                case RegexTreeOperator.Star:
                    return Children[0][index];

                default:
                    var i = Children[0][index];
                    if (i != null)
                        return i;
                    else
                        return Children[1][index];
            }
        }


        internal enum RegexTreeOperator
        {
            Concat, Or, Star
        }
    }
}
