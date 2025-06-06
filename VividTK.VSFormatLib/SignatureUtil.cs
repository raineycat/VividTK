namespace VividTK.VSFormatLib
{
    public static class SignatureUtil
    {
        [Obsolete("This method hasn't been finished, and as such gives invalid results! Please don't use!")]
        public static string DecodeChartSignature(byte[] bytes)
        {
            if (bytes.Length < 384)
            {
                throw new FileFormatException("Invalid buffer length for chart signature!");
            }

            var sig = new string('_', 0xFFF).ToCharArray();

            for (int i = 0; i < 384; i++)
            {
                var num = bytes[i];
                var hex = string.Format("{0:X2}", num);
                sig[i * 3 + 1] = hex[0];
                sig[i * 3 + 2] = hex[1];
            }

            return string.Join("", sig).TrimEnd('_');
        }
    }
}
