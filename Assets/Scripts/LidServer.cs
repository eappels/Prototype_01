using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LidServer : LidPeer
{

    public readonly NetServer netserver;
    public event Action<string> OnNetworkDebugMessage = null;
    public readonly HashSet<NetworkClientInfo> connected_clients = new HashSet<NetworkClientInfo>();
    private int clientidcounter;

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
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) GameObject.Destroy(go);
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
        NetworkRemoteCallSender.CallOnClient(client, "RPC_Hello", client.clientid);
    }

    private void OnClientDisconnected(NetIncomingMessage nim)
    {
        var client = GetClientInfo(nim);
        if (client.spawned) RPC_RequestDespawn(nim);
        connected_clients.Remove(client);
    }

    private NetworkClientInfo GetClientInfo(NetIncomingMessage nim)
    {
        var netconnection = nim.SenderConnection;
        if (netconnection.Tag == null) netconnection.Tag = new NetworkClientInfo(++clientidcounter, netconnection);
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

    public void RPC_RequestObjects(NetIncomingMessage nim)
    {
        var client = GetClientInfo(nim);
        foreach (LidObject lo in MonoBehaviour.FindObjectsOfType(typeof(LidObject)).Cast<LidObject>())
        {
            NetworkRemoteCallSender.CallOnClient(client, "RPC_Spawn", lo.id, lo.prefabname, lo.transform.position, lo.transform.rotation);
        }
    }

    public void RPC_RequestSpawn(NetIncomingMessage nim, string prefabname, Vector3 position, Quaternion rotation)
    {
        var client = GetClientInfo(nim);
        var gameobject = client.gameobject = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabname), position, rotation);
        LidObject lo = gameobject.AddComponent<LidObject>();
        lo.id = client.clientid;
        lo.prefabname = prefabname;
        client.spawned = true;
        NetworkRemoteCallSender.CallOnAllClients("RPC_Spawn", client.clientid, prefabname, position, rotation);
    }

    public void RPC_RequestDespawn(NetIncomingMessage nim)
    {
        var client = GetClientInfo(nim);
        GameObject.Destroy(client.gameobject);
        client.spawned = false;
        NetworkRemoteCallSender.CallOnAllClients("RPC_Despawn", client.clientid);
    }
}