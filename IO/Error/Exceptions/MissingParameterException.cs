using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.IO.Error.Exceptions
{
    class MissingParameterException : Exception
    {

        public MissingParameterException(string paramName) : base($"Missing required parameter \'{paramName}\'") { }

    }
}
