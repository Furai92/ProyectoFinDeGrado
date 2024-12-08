using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _chatUI;
    [SerializeField] private UIDocument _uiDocument;
    private Button _sendButton;
    private TextField _chatTextField;
    private Label _chatLabel;
    private bool _hasStarted = false; // Flag to track if host or client has started

    void SetUpUI()
    {
        // Set up UI
        _sendButton = _uiDocument.rootVisualElement.Q<Button>("send-button");
        _chatTextField = _uiDocument.rootVisualElement.Q<TextField>("chat-text-field");
        _chatLabel = _uiDocument.rootVisualElement.Q<Label>("chat-text");

        // Set up event listener for send button
        _sendButton.clicked += () =>
        {
            // Send chat message
            SendChatMessage(_chatTextField.value);
        };
    }

    private void SendChatMessage(string message) {
        _chatTextField.value = ""; // Clear text field
        SendChatMessageServerRpc(message); // Send message to server
    }

    [ServerRpc (RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message) {
       ReceiveChatMessageClientRpc(message, NetworkManager.Singleton.LocalClientId);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message, ulong senderClientId) {
  
        if (senderClientId == NetworkManager.Singleton.LocalClientId)
        {
            return; // Don't update the chat for the sender
        }
        _chatLabel.text = message; 
    }


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
            Destroy(_camera.gameObject); // Destroy camera
            _chatUI.SetActive(true); // Show chat UI
            SetUpUI(); // Set up UI
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
            Destroy(_camera.gameObject); // Destroy camera
            _chatUI.SetActive(true); // Show chat UI
            SetUpUI(); // Set up UI
        }
        else
        {
            Debug.LogError("NetworkManager is not assigned.");
        }
    }
}