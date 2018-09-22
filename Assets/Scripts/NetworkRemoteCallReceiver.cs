using Lidgren.Network;
using System;
using UnityEngine;

public class NetworkRemoteCallReceiver
{

    public static void ReceiveRemoteCall(NetIncomingMessage nim)
    {
        var instance = GetCallTargetInstance(nim.ReadInt32());
        if (instance == null) return;
        var instanceType = instance.GetType();
        var methodname = nim.ReadString();
        var method = instanceType.GetMethod(methodname);
        Debug.Log("RPC: " + instanceType.Name + "." + methodname);
        if (method == null) throw new Exception("Found no method named '" + methodname + "' on type '" + instance.GetType() + "'");
        var parms = method.GetParameters();
        var args = new object[parms.Length];
        args[0] = nim;
        for (var i = 1; i < args.Length; ++i) args[i] = ReadArgument(nim, parms[i].ParameterType);
        method.Invoke(instance, args);
    }

    static object GetCallTargetInstance(int id)
    {
        if (id == 0)
        {
            return LidPeer.instance;
        }
        else
        {
            return (byte)0; //NetworkActorRegistry.GetById(id);
        }
    }

    static object ReadArgument(NetIncomingMessage msg, Type type)
    {
        if (type == typeof(int))
        {
            return msg.ReadInt32();
        }
        else if (type == typeof(byte))
        {
            return msg.ReadByte();
        }
        else if (type == typeof(float))
        {
            return msg.ReadFloat();
        }
        else if (type == typeof(Vector3))
        {
            return NetworkUtils.ReadVector3(msg);
        }
        else if (type == typeof(Quaternion))
        {
            return NetworkUtils.ReadQuaternion(msg);
        }
        else if (type == typeof(string))
        {
            return msg.ReadString();
        }
        else
        {
            throw new Exception("Unsupported argument type " + type);
        }
    }
}