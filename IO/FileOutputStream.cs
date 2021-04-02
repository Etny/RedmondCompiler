using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Redmond.IO
{
    class FileOutputStream : OutputStream
    {

        private bool wroteIndent = false;

        private string _path;
        private FileStream stream;

        private StringBuilder builder;

        public FileOutputStream(string path) { _path = path; }

        public override void OpenStream()
        {
            builder = new StringBuilder();
        }

        public override void CloseStream()
        {
            File.WriteAllText(_path, string.Empty);
            using FileStream stream = new FileStream(_path, FileMode.OpenOrCreate);
            var bytes = new UTF8Encoding(true).GetBytes(builder.ToString());
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }


        public override void WriteString(string s = "")
        {
            if (!wroteIndent) builder.Append(GetIndentation());
            wroteIndent = true;
            builder.Append(s);
        }

        public override void WriteLine(string s = "")
        {
            if (!wroteIndent) builder.Append(GetIndentation());
            builder.Append(s + "\n");
            wroteIndent = false;
            currentIndent = nextIndent;
        }


        public override int ReserveLocation()
        {
            int temp = builder.Length;
            return temp;
        }

        public override void WriteStringAtLocation(int location, string s = "")
        {
            if (location > builder.Length)
            {
                builder.Append(new string(' ', location - builder.Length));

                WriteLine(s);
            }
            else
            {
                builder.Insert(location, GetIndentation() + s + "\n");
            }
        }
    }
}
