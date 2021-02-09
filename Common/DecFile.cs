using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Redmond.Common
{
    class DecFile
    {
        public string[] TokenLines;
        public string[] LexLines;
        public string[] GrammarLines;
        public string[] SettingsLines;

        public DecFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            string[] lines = File.ReadAllLines(path);

            string currentPortion = "";
            List<string> lineList = new List<string>();

            foreach (string l in lines)
            {
                string line = StripLine(l);

                if (line.Length <= 0) continue;

                if (line[0] == '#')
                {
                    switch (line.ToLower())
                    {
                        case "#end":
                            switch (currentPortion)
                            {
                                case "#tokens":
                                    TokenLines = lineList.ToArray();
                                    break;
                                case "#lex":
                                    LexLines = lineList.ToArray();
                                    break;
                                case "#grammar":
                                    GrammarLines = lineList.ToArray();
                                    break;
                                case "#settings":
                                    SettingsLines = lineList.ToArray();
                                    break;
                            }
                            lineList.Clear();
                            break;

                        default:
                            currentPortion = line.ToLower();
                            break;
                    }
                }
                else
                    lineList.Add(line);
            }

        }

        private string StripLine(string line)
        {
            if (line.Contains(@"//")) line = line.Split("//", 2)[0];
            return line.Trim();
        }
    }
}
