using System;
using System.Collections.Generic;
using System.Text;

namespace Redmond.Common
{
    static class CompileSettings
    {
        private static Dictionary<string, string> _settings = new Dictionary<string, string>();

        public static void InitSettings(string[] settingsLines)
        {
            foreach(string s in settingsLines)
            {
                var split = s.Split(":");
                _settings.Add(split[0].Trim(), split[1].Trim());
            }
        }

        public static int PrecedenceSearchDepth
            => int.Parse(tryGet("PrecedenceSearchDepth") ?? "0");

        public static bool AutoValueInheritance
            => bool.Parse(tryGet("AutoValueInheritance") ?? "false");

        private static string tryGet(string key)
        {
            if (_settings.ContainsKey(key)) return _settings[key];
            return null;
        }
    }
}
