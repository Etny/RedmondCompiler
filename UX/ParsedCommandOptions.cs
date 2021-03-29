using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Redmond.UX
{
    class ParsedCommandOptions
    {

        public ImmutableList<CommandOption> Options { get; private set; }

        public ParsedCommandOptions(string[] opts)
        {
            List<CommandOption> options = new List<CommandOption>();

            for(int i = 1; i < opts.Length; i++)
            {
                string o = opts[i];
                string arg = null;

                while (o.StartsWith('-')) o = o[1..];

                if (i < opts.Length - 1 && !opts[i + 1].StartsWith('-'))
                    arg = opts[++i];

                options.Add(new CommandOption(o, arg));
            }

            Options = ImmutableList<CommandOption>.Empty.AddRange(options);
        }

        public CommandOption FindOption(params string[] names)
        {
            foreach(string name in names)
                if (Options.Exists(o => o.Name == name))
                    return Options.Find(o => o.Name == name);

            return null;
        }


        public class CommandOption
        {
            public readonly string Name;
            public readonly string Argument;

            public CommandOption(string name, string arg = null)
            {
                Name = name;
                Argument = arg;
            }

            public bool HasArgument => Argument != null;
        }
    }
}
