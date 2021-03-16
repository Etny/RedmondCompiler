using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.IntermediateCode
{
    class LabelManager
    {
        public int Count = 0;
        public bool LabelNext = false;

        public string CurrentLabel => "Label" + Count;
        public string NextLabel { get { LabelNext = false; return "Label" + Count++; } }

    }
}
