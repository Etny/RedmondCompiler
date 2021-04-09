using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ParserState
    {

        public Dictionary<int, int> Goto = new Dictionary<int, int>();
        public Dictionary<int, ParserAction> Action = new Dictionary<int, ParserAction>();
        public int Index;

#warning Remove this!
        private readonly List<GrammarItem> _i = null;

        public ParserState()
        {
           // _i = I;
        }

        public ParserState(List<GrammarItem> I)
        {
             _i = I;
        }


        public ParserState(int id, string serialized)
        {
            Index = id;

            int index = 0;

        

            string go = serialized.ReadUntil(ref index, c => c == GrammarConstants.ReservedChar);

            if (go.Length > 0)
            {
                foreach (string s in go.Split(','))
                {
                    var nums = s.Split(':');
                    Goto[int.Parse(nums[0])] = int.Parse(nums[1]);
                }
            }   

            string[] actions = serialized[(index + 1)..].Split(GrammarConstants.ReservedChar);

            if (actions.Length > 0)
            {
                foreach (string s in actions)
                {
                    int readIndex = 0;
                    int num = int.Parse(s.ReadUntil(ref readIndex, c => c == ':'));

                    Action[num] = ParserAction.FromString(s[(readIndex + 1)..]);
                }
            }
        }

        //TODO: this
        public override string ToString()
        {
            if (_i == null) return "state";

            string s = "";

            foreach (var g in _i)
                s += "["+g + "] \n";

            return s;
        }

        internal string Serialize()
        {
            StringBuilder builder = new StringBuilder();

            foreach (int key in Goto.Keys)
                builder.Append($"{key}:{Goto[key]},");

            if (builder.Length > 0) 
                builder.Remove(builder.Length - 1, 1);

            builder.Append(GrammarConstants.ReservedChar);

            foreach (int key in Action.Keys)
                builder.Append($"{key}:{Action[key]}{GrammarConstants.ReservedChar}");

            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }
    }

    
}
