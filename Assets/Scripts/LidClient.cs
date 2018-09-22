using Lidgren.Network;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LidClient : LidPeer
{

    public readonly NetClient netclient;
    public event Action<string> OnNetworkDebugMessage = null;
    public event Action Connected = null, Disconnected = null;
    private int clientid;

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

    public void RPC_Hello(NetIncomingMessage nim, int clientid)
    {
        this.clientid = clientid;
        NetworkRemoteCallSender.CallOnServer("RPC_RequestObjects");
    }

    public void RPC_Spawn(NetIncomingMessage nim, int id, string prefabname, Vector3 position, Quaternion rotation)
    {
        var gameobject = (GameObject)GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabname), position, rotation);
        var lidobject = gameobject.AddComponent<LidObject>();
        lidobject.id = id;
        if (lidobject.id == this.clientid)
        {
            UIManager.Instance.btn_Spawn.GetComponentInChildren<Text>().text = "Despawn";
            Camera.main.GetComponent<WoWCamera>().target = lidobject.transform;
        }
    }

    public void RPC_Despawn(NetIncomingMessage nim, int clientid)
    {
        foreach (LidObject lo in MonoBehaviour.FindObjectsOfType(typeof(LidObject)).Cast<LidObject>())
        {
            if (lo.id == clientid)
            {
                GameObject.Destroy(lo.gameObject);
            }
            if (this.clientid == clientid)
            {
                UIManager.Instance.btn_Spawn.GetComponentInChildren<Text>().text = "Spawn";
                Camera.main.GetComponent<WoWCamera>().target = null;
            }
        }
    }
}