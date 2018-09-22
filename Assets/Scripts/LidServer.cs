using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LidServer : LidPeer
{

    public readonly NetServer netserver;
    public event Action<string> OnNetworkDebugMessage = null;
    public readonly HashSet<NetworkClientInfo> connected_clients = new HashSet<NetworkClientInfo>();

    public LidServer()
        : base()
    {
        instance = this;
        isserver = true;
        isclient = false;
        var config = CreateConfig();
        config.Port = APPPORT;
        netpeer = netserver = new NetServer(config);
    }

    public void StartServer()
    {
        netserver.Start();
    }

    public void StopServer()
    {
        netserver.Shutdown(string.Empty);
    }

    protected override void OnDebugMessage(NetIncomingMessage nim)
    {
        if (OnNetworkDebugMessage != null) OnNetworkDebugMessage(nim.ReadString());
    }

    protected override void OnStatusChanged(NetIncomingMessage nim)
    {
        switch (nim.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                OnClientConnected(nim);
                break;
            case NetConnectionStatus.Disconnected:
                OnClientDisconnected(nim);
                break;
        }
    }

    private void OnClientConnected(NetIncomingMessage nim)
    {
        var client = GetClientInfo(nim);
        connected_clients.Add(client);
        NetworkRemoteCallSender.CallOnClient(client, "RPC_Hello", client.id);
    }

    private void OnClientDisconnected(NetIncomingMessage nim)
    {
        var client = GetClientInfo(nim);
        connected_clients.Remove(client);
    }

    private NetworkClientInfo GetClientInfo(NetIncomingMessage nim)
    {
        var netconnection = nim.SenderConnection;
        if (netconnection.Tag == null) netconnection.Tag = new NetworkClientInfo(++id, netconnection);
        return (NetworkClientInfo)netconnection.Tag;
    }

    protected override void OnDataMessage(NetIncomingMessage nim)
    {
        while (nim.Position < nim.LengthBits)
        {
            switch (nim.ReadByte())
            {
                case LidPeer.REMOTE_CALL_FLAG:
                    NetworkRemoteCallReceiver.ReceiveRemoteCall(nim);
                    break;
            }
        }
    }

    public void RPC_Olleh(NetIncomingMessage nim)
    {
        Debug.Log("RPC_Olleh");
    }
}