using System.Text;

namespace VividTK.VSFormatLib
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadTerminatedString(this BinaryReader reader, char terminator = '\0')
        {
            var sb = new StringBuilder();
            char c;

            do
            {
                c = reader.ReadChar();
                sb.Append(c);
            } while(c !=  terminator);

            return sb.ToString().TrimEnd(terminator);
        }
    }
}
