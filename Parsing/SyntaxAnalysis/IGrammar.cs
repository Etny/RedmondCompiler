using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing.SyntaxAnalysis
{
    interface IGrammar
    {

        Parser GetParser();
    }
}
