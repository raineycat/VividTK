using System.Text;
using VividTK.VSFormatLib.Chart;
using VividTK.VSFormatLib.VSD;

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
        
        public static void Write(this BinaryWriter w, ObjectType val) => w.Write((byte)val);
        public static void Write(this BinaryWriter w, SongField val) => w.Write((byte)val);
        public static void Write(this BinaryWriter w, ChartDataType val) => w.Write((byte)val);
        public static void Write(this BinaryWriter w, SongFieldType val) => w.Write(SongFieldTypeHelper.FieldTypeToByte(val));

        public static void WriteTerminatedString(this BinaryWriter w, string str, char terminator = '\0')
        {
            foreach (var c in str)
            {
                w.Write(c);
            }
            w.Write(terminator);
        }
    }
}
