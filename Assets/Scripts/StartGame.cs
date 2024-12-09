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
        Debug.Log($"Sent message: {message} ");
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        ReceiveChatMessageClientRpc(message, senderClientId);
    }

    [ClientRpc]
    private void ReceiveChatMessageClientRpc(string message, ulong senderClientId)
    {
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
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 40,
            padding = new RectOffset(30, 30, 15, 15),
            alignment = TextAnchor.MiddleCenter
        };

        float areaWidth = 800;
        float areaHeight = 500;
        float x = (Screen.width - areaWidth) / 2;
        float y = (Screen.height - areaHeight) / 2;

        GUILayout.BeginArea(new Rect(x, y, areaWidth, areaHeight));

        GUILayout.Space(40);

        if (GUILayout.Button("Start Host", buttonStyle, GUILayout.Width(600), GUILayout.Height(150)))
        {
            StartHost();
        }

        GUILayout.Space(40);

        if (GUILayout.Button("Start Client", buttonStyle, GUILayout.Width(600), GUILayout.Height(150)))
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