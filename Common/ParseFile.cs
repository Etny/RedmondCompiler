using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Immutable;

namespace Redmond.Common
{
    class ParseFile
    {
        public readonly string Path;

        public ImmutableArray<string> LexLines { get; protected set; } = ImmutableArray<string>.Empty; 
        public ImmutableArray<string> TokenIdLines { get; protected set; } = ImmutableArray<string>.Empty;
        public ImmutableArray<string> ParseTableLines { get; protected set; } = ImmutableArray<string>.Empty;

        public ParseFile(string path)
        {
            Path = path;
        }

        public void Save()
        {
            if (!File.Exists(Path)) File.Create(Path);

            using StreamWriter write = new StreamWriter(Path);

            ImmutableArray<string>[] allLines = new ImmutableArray<string>[] { LexLines, TokenIdLines, ParseTableLines };

            for (int i = 0; i < allLines.Length; i++)
            {
                foreach (string line in allLines[i]) write.WriteLine(line);
                if (i < allLines.Length - 1) write.WriteLine("##");
            }

            write.Flush();
        }

        public ParseFile Read()
        {
            int mode = 0;
            List<string> read = new List<string>();

            foreach(string line in File.ReadAllLines(Path))
            {
                if (line == "##")
                {
                    switch (mode++)
                    {
                        case 0: LexLines = ImmutableArray<string>.Empty.AddRange(read); break;
                        case 1: TokenIdLines = ImmutableArray<string>.Empty.AddRange(read); break;
                        case 2: ParseTableLines = ImmutableArray<string>.Empty.AddRange(read); break;
                    }

                    read.Clear();
                }
                else
                    read.Add(line);
            }

            switch (mode)
            {
                case 0: LexLines = ImmutableArray<string>.Empty.AddRange(read); break;
                case 1: TokenIdLines = ImmutableArray<string>.Empty.AddRange(read); break;
                case 2: ParseTableLines = ImmutableArray<string>.Empty.AddRange(read); break;
            }

            return this;
        }

        public void SetTokenIdLines(IEnumerable<string> lines)
            => TokenIdLines = ImmutableArray<string>.Empty.AddRange(lines);

        public void SetParseTableLines(IEnumerable<string> lines)
            => ParseTableLines = ImmutableArray<string>.Empty.AddRange(lines);


        public void SetLexLines(IEnumerable<string> lines)
            => LexLines = ImmutableArray<string>.Empty.AddRange(lines);


    }
}
