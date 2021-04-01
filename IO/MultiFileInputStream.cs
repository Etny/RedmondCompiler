using Redmond.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Redmond.IO
{
    class MultiFileInputStream : InputStream
    {
        private readonly List<string> _paths;
        private int _fileIndex = 0;


        private string _buffer1, _buffer2;

        public MultiFileInputStream(List<string> paths)
        {
            _paths = paths;
        }

        public override void OpenStream()
        {
            _buffer1 = File.ReadAllText(_paths[_fileIndex++]);
            if (_paths.Count > 1) _buffer2 = File.ReadAllText(_paths[_fileIndex++]);
        }

        public override char CurrentChar(int offset = 0)
        {
            int index = Position + offset;
            if (index >= _buffer1.Length)
            {
                if (_buffer2 == null) return GrammarConstants.EndChar;
                index -= _buffer1.Length;
                if (index > _buffer2.Length) return GrammarConstants.EndChar;
                return _buffer2[index];
            }
            else return _buffer1[index];
        }

        public override string GetSubString(int length)
        {
            int endIndex = Position + length - 1;

            if (endIndex >= _buffer1.Length)
            {
                string s1 = _buffer1.Substring(Position);
                endIndex = length - (_buffer1.Length - Position) ;
                string s2;
                if (_buffer2 == null || endIndex >= _buffer2.Length)
                {
                    s2 = _buffer2 ?? "";
                    s2 += GrammarConstants.EndChar;
                }
                else
                    s2 = _buffer2.Substring(0, endIndex);

                return s1 + s2;
            }
            else
                return _buffer1.Substring(Position, length);
        }

        public override void Advance(int advance)
        {
            base.Advance(advance);

            if(Position > _buffer1.Length)
            {
                Position -= _buffer1.Length;
                _buffer1 = _buffer2;
                AtEnd = _buffer1 == null;
                if (_paths.Count > _fileIndex) _buffer2 = File.ReadAllText(_paths[_fileIndex++]);
                else _buffer2 = null;
            }
        }
    }
}
