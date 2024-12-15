using Unity.Netcode;
using System;

public class OfflineTransport : NetworkTransport
{
    public override ulong ServerClientId => 0;

    public override void Initialize(NetworkManager networkManager)
    {
        
    }

    public override void Shutdown()
    {
       
    }

    public override bool StartClient()
    {
        return true;
    }

    public override bool StartServer()
    {
        return true;
    }

    public override void DisconnectLocalClient()
    {
        
    }

    public override void DisconnectRemoteClient(ulong clientId)
    {
        
    }

    public override ulong GetCurrentRtt(ulong clientId)
    {
        return 0;
    }

    public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
    {
        
    }

    public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
    {
        clientId = 0;
        payload = new ArraySegment<byte>();
        receiveTime = 0f;
        return NetworkEvent.Nothing;
    }
}
