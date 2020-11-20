using System;
using System.Text;

namespace SpaInspector
{
    public static class Extensions
    {
        public static string ReadNullTerminatedString(this System.IO.BinaryReader stream)
        {
            var stringBuilder = new StringBuilder();
            char ch;
            while ((ch = stream.ReadChar()) != 0)
                stringBuilder.Append(ch);
            return stringBuilder.ToString();
        }
    }
}
