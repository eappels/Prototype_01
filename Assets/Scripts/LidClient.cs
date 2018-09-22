using Lidgren.Network;
using System;
using System.IO;
using UnityEngine;

public class LidClient : LidPeer
{

    public readonly NetClient netclient;
    public event Action<string> OnNetworkDebugMessage = null;
    public event Action Connected = null, Disconnected = null;

    public LidClient()
        : base()
    {
        instance = this;
        isserver = false;
        isclient = true;
        var config = CreateConfig();
        netpeer = netclient = new NetClient(config);
    }

    public void StartClient()
    {
        netclient.Start();
    }

    public void StopClient()
    {
        netclient.Shutdown(string.Empty);
    }

    protected override void OnDebugMessage(NetIncomingMessage nim)
    {
        if (OnNetworkDebugMessage != null) OnNetworkDebugMessage(nim.ReadString());
    }

    public void Connect(string connectionstring)
    {
        if (connectionstring.Contains(":"))
        {
            string[] tmpstringarray = connectionstring.Split(':');
            netclient.Connect((string)tmpstringarray[0], int.Parse(tmpstringarray[1]));
        }
        else
        {
            netclient.Connect(connectionstring, APPPORT);
        }
    }

    public void Disconnect()
    {
        netclient.Disconnect(string.Empty);
    }

    protected override void OnStatusChanged(NetIncomingMessage nim)
    {
        switch (nim.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                if (Connected != null) Connected();
                break;
            case NetConnectionStatus.Disconnected:
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) GameObject.Destroy(go);
                if (Disconnected != null) Disconnected();
                break;
        }
    }

    protected override void OnDataMessage(NetIncomingMessage nim)
    {
        switch (nim.ReadByte())
        {
            case LidPeer.REMOTE_CALL_FLAG:
                NetworkRemoteCallReceiver.ReceiveRemoteCall(nim);
                break;
        }
    }

    public void RPC_Hello(NetIncomingMessage nim, byte id)
    {
        this.id = id;
        NetworkRemoteCallSender.CallOnServer("RPC_RequestObjects");
    }

    public void RPC_Spawn(NetIncomingMessage nim, byte id, string prefabname, Vector3 position, Quaternion rotation)
    {
        var gameobject = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabname), position, rotation);
    }
}