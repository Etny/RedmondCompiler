namespace Redmond.Output
{
    interface IStringStream
    {

        public void Write(string s = "");
        public void WriteLine(string s = "");

        public void AddIndentation(int indent = 1);
        public void ReduceIndentation(int indent = 1);

        public static IStringStream operator +(IStringStream l, string s)
        {
            l.Write(s);
            return l;
        }

        public static IStringStream operator *(IStringStream l, string s)
        {
            l.WriteLine(s);
            return l;
        }
    }
}