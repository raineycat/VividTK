using System.Diagnostics;
using System.Text;

namespace VividTK.VSFormatLib;

public class ValidationStream : Stream
{
    public Stream Target { get; }
    public Stream ValidationSource { get; }

    public ValidationStream(Stream target, Stream validationSource)
    {
        Target = target;
        ValidationSource = validationSource;
    }

    public override bool CanRead => Target.CanRead;

    public override bool CanSeek => Target.CanSeek && ValidationSource.CanSeek;

    public override bool CanWrite => Target.CanWrite && ValidationSource.CanRead;

    public override long Length => Target.Length;

    public override long Position
    {
        get => Target.Position;
        set
        {
            Target.Position = value;
            ValidationSource.Position = value;
        }
    }

    public override void Flush() => Target.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var targetBuffer = new byte[count]; 
        var targetCount = Target.Read(targetBuffer, offset, count);

        var validationBuffer = new byte[targetCount];
        var validationCount = ValidationSource.Read(validationBuffer, offset, targetCount);

        if(CompareBuffers(targetBuffer, validationBuffer, targetCount))
        {
            Array.Copy(targetBuffer, buffer, targetCount);
            return targetCount;
        } else
        {
            Debug.Fail("Failed to validate buffer read!");
            return 0;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var targetPos = Target.Seek(offset, origin);
        var validationPos = ValidationSource.Seek(offset, origin);
        Debug.Assert(targetPos == validationPos);
        return targetPos;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var validationBuffer = new byte[count];
        var validationCount = ValidationSource.Read(validationBuffer, offset, count);

        if(CompareBuffers(buffer, validationBuffer, count))
        {
            Target.Write(buffer, offset, count);
        } else
        {
            Debug.Fail("Failed to validate buffer write!");
        }
    }

    protected override void Dispose(bool disposing)
    {
        Target.Dispose();
        ValidationSource.Dispose();
    }

    private static bool CompareBuffers(byte[] a, byte[] b, int count)
    {
        var actualCount = Math.Min(Math.Min(a.Length, b.Length), count);
        for(var i = 0; i < actualCount; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }
        return true;
    }
}
