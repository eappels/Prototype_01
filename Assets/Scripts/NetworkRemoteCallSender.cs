using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkRemoteCallSender
{

    private const NetDeliveryMethod deliveryMethod = NetDeliveryMethod.ReliableOrdered;

    public static void CallOnClient(NetworkClientInfo client, string methodname, params object[] args)
    {
        CallOnClients(new[] { client }, methodname, args);
    }

    public static void CallOnClients(IList<NetworkClientInfo> clients, string methodname, params object[] args)
    {
        if (!LidPeer.isserver) throw new Exception();
        var netconnections = new List<NetConnection>(clients.Count);
        for (var i = 0; i < clients.Count; ++i) netconnections.Add(clients[i].netconnection);
        CallOnConnections(netconnections, methodname, args);
    }

    private static void CallOnConnections(IList<NetConnection> connections, string methodname, params object[] args)
    {
        var msg = BuildMessage(0, methodname, args);
        LidPeer.instance.netpeer.SendMessage(msg, connections, deliveryMethod, 1);
    }

    public static void CallOnServer(string methodname, params object[] args)
    {
        var nom = BuildMessage(0, methodname, args);
        var client = (LidClient)LidPeer.instance;
        client.netclient.SendMessage(nom, deliveryMethod, 0);
    }

    private static NetOutgoingMessage BuildMessage(int id, string methodname, params object[] args)
    {
        var nom = LidPeer.instance.CreateMessage();
        nom.Write(LidPeer.REMOTE_CALL_FLAG);
        nom.Write(id);
        nom.Write(methodname);
        for (var i = 0; i < args.Length; ++i)  WriteArgument(nom, args[i]);
        return nom;
    }

    static void WriteArgument(NetOutgoingMessage nom, object a)
    {
        if (a is byte)
        {
            nom.Write((byte)a);

        }
        else if (a is int)
        {
            nom.Write((int)a);

        }
        else if (a is float)
        {
            nom.Write((float)a);

        }
        else if (a is Vector3)
        {
            NetworkUtils.Write(nom, (Vector3)a);

        }
        else if (a is Quaternion)
        {
            NetworkUtils.Write(nom, (Quaternion)a);

        }
        else if (a is string)
        {
            nom.Write((string)a);

        }
        else
        {
            throw new Exception("Unsupported remote call argument type '" + a.GetType() + "'");
        }
    }
}