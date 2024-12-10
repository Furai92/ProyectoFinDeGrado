using Netcode.Transports.WebRTCTransport;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _mainMenuDocument;
    [SerializeField] private UIDocument _waitingRoomDocument;
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private WebRTCTransport _webRTCTransport;
    private Button _playButton;
    private Button _hostButton;
    private Button _joinButton;
    private UnsignedIntegerField _codeIntegerField;
    private Button _cancelButton;
    private Label _statusLabel;
    private Label _codeLabel;
    private Label _roleStatusLabel;
    private bool _isHost = false;

    void OnEnable()
    {
       _webRTCTransport.OnWebSocketConnected += SetUpWaitingRoomUI;
       _webRTCTransport.OnP2PConnectionEstablished += OnP2PConnectionEstablished;
    }

    void OnDisable()
    {
        _webRTCTransport.OnWebSocketConnected -= SetUpWaitingRoomUI;
        _webRTCTransport.OnP2PConnectionEstablished -= OnP2PConnectionEstablished;
    }

    void OnP2PConnectionEstablished()
    {
        if(_networkManager.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MultiplayerTest", LoadSceneMode.Single);
        }
    }

    void Start()
    {
        SetUpMainMenuUI();
    }

    void SetUpWaitingRoomUI()
    {
        _cancelButton = _waitingRoomDocument.rootVisualElement.Q<Button>("cancel-button");
        _statusLabel = _waitingRoomDocument.rootVisualElement.Q<Label>("status-label");
        _codeLabel = _waitingRoomDocument.rootVisualElement.Q<Label>("code-label");
        _roleStatusLabel = _waitingRoomDocument.rootVisualElement.Q<Label>("role-status-label");

        _cancelButton.clicked += () =>
        {
            _networkManager.Shutdown();
            SwitchMenu("main");
        };

        if (_isHost)
        {
            _statusLabel.text = "Waiting for player...";
            _roleStatusLabel.text = "SHARE THIS CODE";
            _codeLabel.text = _webRTCTransport.Password;
        }
        else
        {
            _statusLabel.text = "Joining game...\n The host may not be available on that code.";
            _roleStatusLabel.text = "JOINING WITH CODE";
            _codeLabel.text = _codeIntegerField.value.ToString();
        }
        SwitchMenu("waiting");
    }
    void SetUpMainMenuUI()
    {
        // Set up UI
        _playButton = _mainMenuDocument.rootVisualElement.Q<Button>("play-button");
        _hostButton = _mainMenuDocument.rootVisualElement.Q<Button>("host-button");
        _joinButton = _mainMenuDocument.rootVisualElement.Q<Button>("join-button");
        _codeIntegerField = _mainMenuDocument.rootVisualElement.Q<UnsignedIntegerField>("code-integer-field");

        _playButton.clicked += () =>
        {
            _networkManager.StartHost();
        };
        _hostButton.clicked += () =>
        {
            _networkManager.StartHost();
            _isHost = true;
        };
        _joinButton.clicked += () =>
        {
            if(_codeIntegerField.value != 0 && _codeIntegerField.value < 10000 && _codeIntegerField.value > 999 && _codeIntegerField.value.ToString().Length == 4)
            {
                Debug.Log("Joining game with code: " + _codeIntegerField.value);
                ChangeJoinCode(_codeIntegerField.value.ToString());
                _networkManager.StartClient();
                _isHost = false;
            }
        };
        SwitchMenu("main");
    }

    private void SwitchMenu(string menu)
    {
        if (menu == "main")
        {
            _mainMenuDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            _waitingRoomDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
        else if (menu == "waiting")
        {
            _mainMenuDocument.rootVisualElement.style.display = DisplayStyle.None;
            _waitingRoomDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }
    }

    private void ChangeJoinCode(string code)
    {
        _webRTCTransport.JoinCode = code;
    }
}
