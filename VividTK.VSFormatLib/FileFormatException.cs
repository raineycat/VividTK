namespace VividTK.VSFormatLib;


[Serializable]
public class FileFormatException : Exception
{
    public FileFormatException() { }
    public FileFormatException(string message) : base(message) { }
    public FileFormatException(string message, Exception inner) : base(message, inner) { }
}
