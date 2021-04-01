using Redmond.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    class ProductionEntryRegister
    {

        private int _nextId = 0;
        private Dictionary<string, int> _tags = new Dictionary<string, int>();

        public int this[string s] => _tags[s];


        public void ParseSerializedTags(IEnumerable<string> tags)
        {
            foreach (string l in tags)
            {
                var split = l.Split(GrammarConstants.ReservedChar);
                int num = int.Parse(split[0]);
                string tag = split[1];
                RegisterTag(num, tag);
            }
        }

        public List<string> Serialize()
        {
            List<string> savedTags = new List<string>();

            foreach (string s in _tags.Keys)
            {
                if (s[0] == '/') continue;
                savedTags.Add((_tags[s] + "") + GrammarConstants.ReservedChar + s);
            }

            return savedTags;
        }

        public int GetId(string tag)
        {
            if (_tags.ContainsKey(tag)) return _tags[tag];
            while (_tags.ContainsValue(++_nextId)) ;
            _tags[tag] = _nextId;
            return _nextId;
        }

        public void RegisterTag(int num, string tag)
        {
            _tags.Add(tag, num);
        }
    }
}
