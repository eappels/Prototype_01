using Lidgren.Network;

public class NetworkClientInfo
{

    public readonly byte id;
    public readonly NetConnection netconnection;
    public bool spawned;

    public NetworkClientInfo(byte id, NetConnection netconnection)
    {
        this.id = id;
        this.netconnection = netconnection;
        this.netconnection.Tag = this;
    }
}