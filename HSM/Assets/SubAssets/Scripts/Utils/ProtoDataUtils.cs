using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using ProtoBuf;
using GenPB;

public class ProtoDataUtils {

	public static byte[] ObjectToBytes<T>(T instance)
    {
        try
        {
            byte[] byteArray;
            if (null == instance)
            {
                byteArray = new byte[0];
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                Serializer.Serialize<T>(ms, instance);
                byteArray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(byteArray, 0, byteArray.Length);
                ms.Dispose();
            }

            return byteArray;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return new byte[0];
        }
    }

    public static T BytesToObject<T>(byte[] byteArray)
    {
        if (byteArray.Length <= 0)
        {
            return default(T);
        }

        try
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(byteArray, 0, byteArray.Length);
            ms.Position = 0;
            T result = Serializer.Deserialize<T>(ms);
            ms.Dispose();
            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return default(T);
        }
    }

    public static Vector3 Vector3PBToVecotr3(Vector3PB vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    public static Vector3PB Vector3ToVector3PB(Vector3 vector3)
    {
        Vector3PB vPb = new Vector3PB();
        vPb.X = vector3.x;
        vPb.Y = vector3.y;
        vPb.Z = vector3.z;

        return vPb;
    }

    public static void Vector3PBCopyVector3(ref Vector3PB vpb, Vector3 v)
    {
        vpb.X = v.x;
        vpb.Y = v.y;
        vpb.Z = v.z;
    }
}
