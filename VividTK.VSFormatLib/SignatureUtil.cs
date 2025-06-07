using System.Runtime.InteropServices;

namespace VividTK.VSFormatLib
{
    public static class SignatureUtil
    {
        private const string ExtensionFile = "RSAExtension_x64.dll";

        [DllImport(ExtensionFile, EntryPoint = "DLLRSASignBuffer", CharSet = CharSet.Unicode)]
        public static extern string ExtSignBuffer(IntPtr address, long position, string keys);
        [DllImport(ExtensionFile, EntryPoint = "DLLRSAVerifyBuffer", CharSet = CharSet.Unicode)]
        public static extern string ExtVerifyBuffer(IntPtr address, long size, string keys);
    }
}
