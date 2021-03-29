using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.UX
{
    class SubCommandInvoker
    {
        
        public static bool TryInvokeSubcommand(string[] args)
        {
            bool succes = true;
            var parsed = new ParsedCommandOptions(args);

            switch (args[0])
            {
                case "compile": CompileCommands.Compile(parsed); break;

                default:
                    succes = false;
                    break;
            }

            return succes;
        }

       


    }
}
