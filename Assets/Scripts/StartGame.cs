using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private GameObject _playerPrefab;
    private Button _sendButton;
    private TextField _chatTextField;
    private Label _chatLabel;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetUpUI();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if(!IsHost) return;
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id))
            {
                GameObject playerInstance = Instantiate(_playerPrefab);
                NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
                networkObject.SpawnAsPlayerObject(id, true);

                Debug.Log($"Spawned player object for ClientId: {id}");
            }
        }
    }

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
        SendChatMessageRpc(message); // Send message to server
        Debug.Log($"Sent message: {message} ");
    }

    [Rpc(SendTo.NotMe)]
    private void SendChatMessageRpc(string message)
    {
        Debug.Log($"Received message: {message} ");
        _chatLabel.text = message;
    }
}