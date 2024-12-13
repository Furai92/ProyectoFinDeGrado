using Netcode.Transports.WebRTCTransport;
using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : NetworkManager
{
    public static ConnectionManager Instance { get; private set; }
    private WebRTCTransport _webRTCTransport;
    private OfflineTransport _offlineTransport;
    public NetworkManager NetworkManager => Singleton;
    public WebRTCTransport WebRTCTransport => _webRTCTransport;
    public OfflineTransport OfflineTransport => _offlineTransport;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _webRTCTransport = GetComponent<WebRTCTransport>();
        _offlineTransport = GetComponent<OfflineTransport>();
    }
}
