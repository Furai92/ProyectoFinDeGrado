using Netcode.Transports.WebRTCTransport;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MainMenu : UIMenu
{
    private Button _playButton;
    private Button _hostButton;
    private Button _joinButton;
    private UnsignedIntegerField _codeIntegerField;
    private Button _cancelButton;
    private Label _statusLabel;
    private Label _codeLabel;
    private Label _roleStatusLabel;
    private VisualElement _waitingRoomSubMenu;
    private bool _isHost = false;
    protected override string MainParentName => "MainMenu";

    void OnEnable()
    {
       ConnectionManager.Instance.WebRTCTransport.OnWebSocketConnected += OnWebSocketConnected;
       ConnectionManager.Instance.WebRTCTransport.OnP2PConnectionEstablished += OnP2PConnectionEstablished;
    }

    void OnDisable()
    {
        ConnectionManager.Instance.WebRTCTransport.OnWebSocketConnected -= OnWebSocketConnected;
        ConnectionManager.Instance.WebRTCTransport.OnP2PConnectionEstablished -= OnP2PConnectionEstablished;
    }

    public override void InitializeUI()
    {

        _playButton = RootVisualElement.Q<Button>("play-button");
        _hostButton = RootVisualElement.Q<Button>("host-button");
        _joinButton = RootVisualElement.Q<Button>("join-button");
        _codeIntegerField = RootVisualElement.Q<UnsignedIntegerField>("code-integer-field");

        _playButton.clicked += () =>
        {
            ConnectionManager.Instance.NetworkConfig.NetworkTransport = ConnectionManager.Instance.OfflineTransport;
            ConnectionManager.Instance.StartHost();
            OnP2PConnectionEstablished();
        };
        _hostButton.clicked += () =>
        {
            ConnectionManager.Instance.StartHost();
            _isHost = true;
            //UIManager.Instance.OpenMenu(MenuSelection.WaitingRoom);
            AddSubMenu(_waitingRoomSubMenu, true);
        };
        _joinButton.clicked += () =>
        {
            if(_codeIntegerField.value != 0 && _codeIntegerField.value < 10000 && _codeIntegerField.value > 999 && _codeIntegerField.value.ToString().Length == 4)
            {
                Debug.Log("Joining game with code: " + _codeIntegerField.value);
                ChangeJoinCode(_codeIntegerField.value.ToString());
                ConnectionManager.Instance.StartClient();
                _isHost = false;
            }
        };


        _cancelButton = RootVisualElement.Q<Button>("cancel-button");
        _statusLabel = RootVisualElement.Q<Label>("status-label");
        _codeLabel = RootVisualElement.Q<Label>("code-label");
        _roleStatusLabel = RootVisualElement.Q<Label>("role-status-label");
        _waitingRoomSubMenu = RootVisualElement.Q<VisualElement>("waiting-room-sub-menu");

        _cancelButton.clicked += () =>
        {
            ConnectionManager.Instance.Shutdown();
            //SwitchMenu("main");
        };

        if (_isHost)
        {
            _statusLabel.text = "Waiting for player...";
            _roleStatusLabel.text = "SHARE THIS CODE";
            _codeLabel.text = ConnectionManager.Instance.WebRTCTransport.Password;
        }
        else
        {
            _statusLabel.text = "Joining game...\n The host may not be available on that code.";
            _roleStatusLabel.text = "JOINING WITH CODE";
            _codeLabel.text = _codeIntegerField.value.ToString();
        }
    }

    void OnWebSocketConnected()
    {
        AddSubMenu(_waitingRoomSubMenu);
    }

    void OnP2PConnectionEstablished()
    {
        if(ConnectionManager.Instance.NetworkManager.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MultiplayerTest", LoadSceneMode.Single);
        }
    }
    private void ChangeJoinCode(string code)
    {
        ConnectionManager.Instance.WebRTCTransport.JoinCode = code;
    }
}
