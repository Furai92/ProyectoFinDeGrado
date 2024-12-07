using Unity.Netcode;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private Camera _camera;
    private bool _hasStarted = false; // Flag to track if host or client has started

    void OnGUI()
    {
        if (!_hasStarted)
        {
            // Set up GUI layout
            GUILayout.BeginArea(new Rect(10, 10, 200, 100));

            // Host button
            if (GUILayout.Button("Start Host"))
            {
                StartHost();
            }

            // Client button
            if (GUILayout.Button("Start Client"))
            {
                StartClient();
            }

            GUILayout.EndArea();
        }
    }

    private void StartHost()
    {
        if (_networkManager != null)
        {
            _networkManager.StartHost();
            Debug.Log("Started as Host");
            _hasStarted = true; // Hide GUI
            //Destroy(_camera.gameObject); // Destroy camera
        }
        else
        {
            Debug.LogError("NetworkManager is not assigned.");
        }
    }

    private void StartClient()
    {
        if (_networkManager != null)
        {
            _networkManager.StartClient();
            Debug.Log("Started as Client");
            _hasStarted = true; // Hide GUI
            //Destroy(_camera.gameObject); // Destroy camera
        }
        else
        {
            Debug.LogError("NetworkManager is not assigned.");
        }
    }
}