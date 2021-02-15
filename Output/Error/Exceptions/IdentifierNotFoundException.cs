using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Output.Error.Exceptions
{
    class IdentifierNotFoundException : Exception
    {

        private string ID;
        public IdentifierNotFoundException(string id)
        {
            ID = id;
        }

        //TODO: Elaborte on this
        public override string ToString()
        {
            return ErrorManager.ReportingLevel switch
            {
                ErrorManager.ErrorReportingLevel.Low => $"Identifier not found: {ID}",
                _ => $"Identifier not found:{ID}"
            };
        }
    }
}
