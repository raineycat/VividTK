namespace VividTK.VSFormatLib.VSD
{
    // chart_enums.gml
    public enum SongFieldType
    {
        U8,
        S8,
        U32,
        S32,
        F16,
        F32,
        Bool,
        String,

        Invalid
    }

    public static class SongFieldTypeHelper
    {
        public static SongFieldType ByteToFieldType(byte b) =>
            b switch
            {
                1 => SongFieldType.F32,
                2 => SongFieldType.F32,
                3 => SongFieldType.U8,
                4 => SongFieldType.F32,
                5 => SongFieldType.F32,
                6 => SongFieldType.U8,
                7 => SongFieldType.S8,

                176 => SongFieldType.U8,
                177 => SongFieldType.S8,
                178 => SongFieldType.U32,
                179 => SongFieldType.S32,

                181 => SongFieldType.F16,
                182 => SongFieldType.F32,
                183 => SongFieldType.Bool,
                184 => SongFieldType.String,

                _ => SongFieldType.Invalid
            };
        
        public static byte FieldTypeToByte(SongFieldType type) =>
            type switch
            {
                // 1 => SongFieldType.F32,
                // 2 => SongFieldType.F32,
                // 3 => SongFieldType.U8,
                // 4 => SongFieldType.F32,
                // 5 => SongFieldType.F32,
                // 6 => SongFieldType.U8,
                // 7 => SongFieldType.S8,

                SongFieldType.U8 => 176,
                SongFieldType.S8 => 177,
                SongFieldType.U32 => 178,
                SongFieldType.S32 => 179,

                SongFieldType.F16 => 181,
                SongFieldType.F32 => 182,
                SongFieldType.Bool => 183,
                SongFieldType.String => 184,

                _ => 255
            };

        public static object ReadFrom(this SongFieldType type, BinaryReader r) =>
            type switch
            {
                SongFieldType.U8 => r.ReadByte(),
                SongFieldType.S8 => r.ReadChar(),
                SongFieldType.U32 => r.ReadUInt32(),
                SongFieldType.S32 => r.ReadInt32(),
                SongFieldType.F16 => r.ReadHalf(),
                SongFieldType.F32 => r.ReadSingle(),
                SongFieldType.Bool => r.ReadBoolean(),
                SongFieldType.String => r.ReadTerminatedString(),
                SongFieldType.Invalid => throw new InvalidOperationException(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

        public static SongFieldType GuessFromManagedType(object o)
        {
            if (o is byte) return SongFieldType.U8;
            if (o is char) return SongFieldType.S8;
            if (o is uint) return SongFieldType.U32;
            if (o is int) return SongFieldType.S32;
            if (o is Half) return SongFieldType.F16;
            if (o is float) return SongFieldType.F32;
            if (o is bool) return SongFieldType.Bool;
            if (o is string) return SongFieldType.String;
            return SongFieldType.Invalid;
        }

        public static void WriteSongField(this BinaryWriter writer, object o, SongFieldType type)
        {
            switch (type)
            {
                case SongFieldType.U8:
                    writer.Write((byte)o);
                    break;
                case SongFieldType.S8:
                    writer.Write((char)o);
                    break;
                case SongFieldType.U32:
                    writer.Write((uint)o);
                    break;
                case SongFieldType.S32:
                    writer.Write((int)o);
                    break;
                case SongFieldType.F16:
                    writer.Write((Half)o);
                    break;
                case SongFieldType.F32:
                    writer.Write((float)o);
                    break;
                case SongFieldType.Bool:
                    writer.Write((bool)o);
                    break;
                case SongFieldType.String:
                    writer.Write((string)o);
                    break;
                case SongFieldType.Invalid:
                    break;
            }
        }
    }
}
