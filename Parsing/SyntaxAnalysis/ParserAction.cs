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

        public readonly ParserState ShiftState;

        public ShiftAction(ParserState state)
            => ShiftState = state;

        public ShiftAction(string[] parsed)
        {
            int index = int.Parse(parsed[0]);
        }

        public override string ToString()
        {
            return $"s/{ShiftState.Index}";
        }
    }

    class ReduceAction : ParserAction
    {
        public override string Name => "reduce";


        public readonly int PopCount;
        public readonly NonTerminal Goto;
        public readonly GrammarAction Action;
        public readonly Production Production;

        public ReduceAction(Production prod)
        {
            PopCount = prod.Rhs.Length;
            Goto = prod.Lhs;
            Action = prod.HasAction ? prod.Action : null;
            Production = prod;
        }

        public ReduceAction(string[] parsed)
        {
            PopCount = int.Parse(parsed[0]);
            int gotoId = int.Parse(parsed[1]);

            if (parsed[2] == "n") Action = null;
            else if (parsed[2] == "d") Action = GrammarAction.Default;
            else Action = new GrammarAction(parsed[2], PopCount);
        }

        public override string ToString()
        {
            string actionString = Action == null ? "n" : (Action.DeclarationString == GrammarAction.Default.DeclarationString ? "d" : Action.DeclarationString);

            return $"r{PopCount}/{Goto.ID}/{actionString}";
        }
    }

        
    class AcceptAction : ParserAction 
    {
        public override string Name => "accept";

        public override string ToString()
            => "a";
    }
}
