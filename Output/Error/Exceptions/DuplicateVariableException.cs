using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Output.Error.Exceptions
{
    class DuplicateVariableException : Exception
    {

        public DuplicateVariableException(string varName) : base($"{varName} is already defined") { }

    }
}
