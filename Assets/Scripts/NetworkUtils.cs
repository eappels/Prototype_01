using Lidgren.Network;
using UnityEngine;

public class NetworkUtils
{

    public static void Write(NetOutgoingMessage nom, Vector3 position)
    {
        nom.Write(position.x);
        nom.Write(position.y);
        nom.Write(position.z);
    }

    public static Vector3 ReadVector3(NetIncomingMessage nim)
    {
        Vector3 data;
        data.x = nim.ReadFloat();
        data.y = nim.ReadFloat();
        data.z = nim.ReadFloat();
        return data;
    }

    public static void Write(NetOutgoingMessage nom, Quaternion rotation)
    {
        nom.Write(rotation.x);
        nom.Write(rotation.y);
        nom.Write(rotation.z);
        nom.Write(rotation.w);
    }

    public static Quaternion ReadQuaternion(NetIncomingMessage nim)
    {
        Quaternion data;
        data.x = nim.ReadFloat();
        data.y = nim.ReadFloat();
        data.z = nim.ReadFloat();
        data.w = nim.ReadFloat();
        return data;
    }

    public static void WritePositionRotation(NetOutgoingMessage nom, Transform transform)
    {
        Write(nom, transform.position);
        Write(nom, transform.rotation);
    }
}