using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.IO.Error
{
    class ErrorManager
    {

        public static ErrorReportingLevel ReportingLevel = ErrorReportingLevel.High;

        //Will probably change this to something more advanced in the future
        public static void PrintError(string s)
            => Console.WriteLine(s);

        public static void PrintError(Exception e)
            => Console.WriteLine(e);

        public static void ExitWithError(string s)
        {
            PrintError(s);
            Environment.Exit(-1);
        }
        public static void ExitWithError(Exception e)
        {
            PrintError(e);
            Environment.Exit(-1);
        }

        public enum ErrorReportingLevel
        {
            Low, High
        }
    }
}
