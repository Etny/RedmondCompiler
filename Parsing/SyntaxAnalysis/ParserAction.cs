using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    abstract class ParserAction 
    { 
        public abstract string Name { get; }
    

        public static ParserAction FromString(string s)
        {
            string[] parsed = s[1..].Split('/');

            return s[0] switch
            {
                'r' => new ReduceAction(parsed),
                's' => new ShiftAction(parsed),
                'a' => new AcceptAction(),
                _ => null
            };
        }
    }

    class ShiftAction : ParserAction
    {

        public override string Name => "shift";

        public readonly int StateIndex;
        public ParserState debugState;

        public ShiftAction(ParserState state)
        {
            debugState = state;
            StateIndex = state.Index;
        }

        public ShiftAction(string[] parsed)
        {
            StateIndex = int.Parse(parsed[0]);
        }

        public override string ToString()
        {
            return $"s{StateIndex}";
        }
    }

    class ReduceAction : ParserAction
    {
        public override string Name => "reduce";


        public readonly int PopCount;
        public readonly int GotoID;
        public readonly GrammarAction Action;
        public readonly Production Production;
        public Production debugProd;

        public ReduceAction(Production prod)
        {
            debugProd = prod;
            PopCount = prod.Rhs.Length;
            GotoID = prod.Lhs.ID;
            Action = prod.HasAction ? prod.Action : null;
            Production = prod;
        }

        public ReduceAction(string[] parsed)
        {
            PopCount = int.Parse(parsed[0]);
            GotoID = int.Parse(parsed[1]);

            if (parsed[2] == "n") Action = null;
            else if (parsed[2] == "d") Action = GrammarAction.Default;
            else Action = GrammarAction.FromDec(parsed[2], PopCount);
        }

        public override string ToString()
        {
            string actionString = Action == null ? "n" : (PopCount == 1 && Action.DeclarationString == GrammarAction.Default.DeclarationString ? "d" : Action.DeclarationString);

            return $"r{PopCount}/{GotoID}/{actionString}";
        }
    }

        
    class AcceptAction : ParserAction 
    {
        public override string Name => "accept";

        public override string ToString()
            => "a";
    }
}
