using System;

public class ByteArray
{
    private const int DEFAULT_SIZE = 1024;
    private int initSize = 0;
    private int capacity = 0;

    public byte[] bytes;
    public int readIdx = 0;
    public int writeIdx = 0;

    public int remain { get { return capacity - writeIdx; } }
    public int length { get { return writeIdx - readIdx; } }

    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }

    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public void ReSize(int size)
    {
        if (size < length || size < initSize) return;
        int n = 1;
        while (n < size) n *= 2;
        capacity = n;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, length);
        bytes = newBytes;
        writeIdx = length;
        readIdx = 0;
    }

    public void CheckAndMoveBytes()
    {
        if (length < 8)
            MoveBytes();
    }

    public void MoveBytes()
    {
        if (length > 0)
            Array.Copy(bytes, readIdx, bytes, 0, length);
        writeIdx = length;
        readIdx = 0;
    }

    public int Write(byte[] bytes, int offset, int count)
    {
        if (remain < count)
        {
            ReSize(length + count);
        }
        Array.Copy(bytes, offset, this.bytes, writeIdx, count);
        writeIdx += count;
        return count;
    }

    public int Read(byte[] bytes, int offset, int count)
    {
        count = Math.Min(count, length);
        Array.Copy(this.bytes, readIdx, bytes, offset, count);
        readIdx += count;
        CheckAndMoveBytes();
        return count;
    }

    //长度信息法解决粘包半包问题，本程序规定用2个字节(16位)，同时规定使用小端模式存储长度信息
    //读取长度信息
    public Int16 ReadInt16()
    {
        if (length < 2) return 0;
        Int16 ret = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        readIdx += 2;
        CheckAndMoveBytes();
        return ret;
    }

    //调式
    public override string ToString()
    {
        return BitConverter.ToString(bytes, readIdx, length);
    }

    public string Debug()
    {
        return string.Format("readIdx:{0}, writeIdx:{1}, bytes:{2}", readIdx, writeIdx, this.ToString());
    }
}

