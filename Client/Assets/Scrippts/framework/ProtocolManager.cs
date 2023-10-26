using System;

public static class ProtocolManager
{
    /// <summary>
    /// 编码协议体的名字，字节数组可分为两部分，第一部分包含协议体名字的长度（两个字节且是小端编码），第二部分包含协议体的名字
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] EncodeName(ProtoBuf.IExtensible msgBase)
    {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.ToString());
        Int16 len = (Int16)nameBytes.Length;
        byte[] bytes = new byte[len + 2];
        //小端
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }
    /// <summary>
    /// 编码协议体的内容，字节数组只包含协议体的内容
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] EncodeBody(ProtoBuf.IExtensible msgBase)
    {
        using (var memory = new System.IO.MemoryStream())
        {
            ProtoBuf.Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }

    /// <summary>
    /// 编码整个协议体，字节数组可分为三部分，第一部分包含整个协议体的长度（两个字节且小端编码），
    /// 第二部分包含协议体名字的长度（两个字节且小端编码）以及协议体的名字，第三部分包含协议体的内容
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] Encode(ProtoBuf.IExtensible msgBase)
    {
        byte[] nameBytes = ProtocolManager.EncodeName(msgBase);
        byte[] bodyBytes = ProtocolManager.EncodeBody(msgBase);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] bytes = new byte[2 + len];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, bytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, bytes, 2 + nameBytes.Length, bodyBytes.Length);

        return bytes;
    }

    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        if (offset + 2 > bytes.Length)
            return "";

        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        if (len <= 0)
            return "";

        if (len + 2 + offset > bytes.Length)
            return "";

        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }

    public static ProtoBuf.IExtensible DecodeBody(string protoName, byte[] bytes, int offset, int count)
    {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count))
        {
            System.Type t = System.Type.GetType(protoName);
            return ProtoBuf.Serializer.NonGeneric.Deserialize(t, memory) as ProtoBuf.IExtensible;
        }
    }



}

