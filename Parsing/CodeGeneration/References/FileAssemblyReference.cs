using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Redmond.Parsing.CodeGeneration.References
{
    class FileAssemblyReference : AssemblyReference
    {

        private Assembly Assembly;

        public Dictionary<string, string> Properties = new Dictionary<string, string>();
        private string _name;

        public FileAssemblyReference(Assembly assembly)
        {
            Assembly = assembly;

            var props = assembly.FullName.Split(',');
            _name = props[0];

            for(int i = 1; i < props.Length; i++)
            {
                string[] p1 = props[i].Split('=');
                if (p1.Length != 2) continue;
                Properties.Add(p1[0].Trim(), p1[1].Trim());
            }
        }

        public override void Emit(IlBuilder builder)
        {
            List<string> metadata = new List<string>();

            if (Properties.ContainsKey("Version")) metadata.Add($".ver {Properties["Version"].Replace('.', ':')}");
            if (Properties.ContainsKey("PublicKeyToken")) metadata.Add($".publickeytoken = ({Properties["PublicKeyToken"].ToUpper()})");


            if (metadata.Count == 0)
                builder.EmitLine($".assembly extern {Name}{{}}");
            else
            {
                builder.EmitLine($".assembly extern {Name}");
                builder.EmitLine("{");
                builder.Output.AddIndentation();
                foreach (string m in metadata) builder.EmitLine(m);
                builder.Output.ReduceIndentation();
                builder.EmitLine("}");
            }
        }


        public override string Name => _name;

        public override Type ResolveType(string name) => Assembly.GetType(name);
    }
}
