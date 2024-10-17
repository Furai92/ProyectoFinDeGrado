using Unity.Netcode;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _networkManager.StartHost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
