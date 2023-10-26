using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestByteArray : MonoBehaviour
{
    private void Start()
    {
        ByteArray byteArray = new ByteArray();
        Debug.Log(byteArray.Debug());

        byte[] writeByte = new byte[] { 1, 2, 3, 2, 5};
        byteArray.Write(writeByte, 0, writeByte.Length);
        Debug.Log(byteArray.Debug());

        byte[] readByte = new byte[1024];
        readByte[0] = 1;
        byteArray.Read(readByte, 1, byteArray.length);
        Debug.Log(byteArray.Debug());
        Debug.Log(BitConverter.ToString(readByte));
    }
}
