using Unity.Netcode;
using UnityEngine;
using System;

public class OfflineTransport : NetworkTransport
{
    public override ulong ServerClientId => throw new System.NotImplementedException();

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
        Shutdown();
    }

    public override void DisconnectRemoteClient(ulong clientId)
    {
        Shutdown();
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
