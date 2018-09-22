using Lidgren.Network;
using UnityEngine;

public class NetworkClientInfo
{

    public readonly int clientid;
    public readonly NetConnection netconnection;
    public bool spawned;
    public GameObject gameobject;

    public NetworkClientInfo(int clientid, NetConnection netconnection)
    {
        this.clientid = clientid;
        this.netconnection = netconnection;
        this.netconnection.Tag = this;
    }
}