using Redmond.UX;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Parsing
{
    class CompilationOptions
    {
        private readonly ParsedCommandOptions _opts;
        public CompilationOptions(ParsedCommandOptions opts)
        {
            _opts = opts;
        }

        public bool ParseTree => _opts.FindOption("tree") != null;

        public string AssemblyName
            => _opts.FindOption("assemblyname", "assembly", "name", "n")?.Argument ?? "AssemblyName";

        public string ModuleName
            => _opts.FindOption("moduleName", "module", "modname", "m")?.Argument ?? AssemblyName;
    }
}
