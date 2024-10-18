using System;
using Unity.Netcode;
using System.Net.WebSockets;
using Unity.WebRTC;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections;

namespace Netcode.Transports.WebRTCTransport
{
    public class WebRTCTransport : NetworkTransport
    {
        private ClientWebSocket _webSocket;
        RTCPeerConnection _localConnection;
        RTCDataChannel _sendChannel;
        private string _password;
        private string _address = "79.72.91.98";
        private ushort _port = 80;
        public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
        {

        }

        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
        {
            clientId = 0;
            payload = new ArraySegment<byte>();
            receiveTime = 0.0f;
            return NetworkEvent.Nothing;
        }

        public override bool StartClient()
        {
            return true;
        }

        public override bool StartServer()
        {
            return StartClient();
        }

        public override void DisconnectRemoteClient(ulong clientId)
        {

        }

        public override void DisconnectLocalClient()
        {

        }

        public override ulong GetCurrentRtt(ulong clientId)
        {
            return 0;
        }

        public override void Shutdown()
        {

        }

        public override async void Initialize(NetworkManager networkManager = null)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri($"ws://{_address}:{_port}"), CancellationToken.None);
            Task receiveTask = Task.Run(async () =>
            {
                byte[] buffer = new byte[1024];
                while (_webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log("Received: " + message);
                    await HandleMessage(_webSocket, message);
                }
            });
        }

        public override ulong ServerClientId { get; }

        async Task HandleMessage(WebSocket webSocket, string message)
        {
            ServerMessage messageObject = JsonUtility.FromJson<ServerMessage>(message);

            if (messageObject == null)
            {
                Debug.Log("Failed to deserialize message.");
                return;
            }

            switch (messageObject.MessageType)
            {
                case "Password":
                    _password = messageObject.MessageContent;
                    await SendMessage(webSocket, "ConnectionRequest", "");
                    break;

                case "SendSDPOffer":
                    //Send the SDP offer to the other peer - Peer A
                    Debug.Log("Pairing request received");
                    RTCConfiguration configuration = new RTCConfiguration();
                    configuration.iceServers = new RTCIceServer[] 
                    {
                        new RTCIceServer 
                        { 
                            urls = new string[] { "turn:79.72.91.98:3478" }, 
                            username = "server14", 
                            credential = "41server", 
                            credentialType = RTCIceCredentialType.Password 
                        },
                        new RTCIceServer 
                        { 
                            urls = new string[] { "stun:79.72.91.98:3478" } 
                        }
                    };
                    _localConnection = new RTCPeerConnection(ref configuration);
                    _localConnection.OnIceCandidate = async e => { Debug.Log("ICE candidate"); await SendMessage(webSocket, "ICECandidate", e.Candidate); };
                    _localConnection.OnIceConnectionChange = state =>
                    {
                        Debug.Log($"Local ICE connection state changed: {state}");
                        HandleIceConnectionStateChange(state);
                    };

                    _sendChannel = _localConnection.CreateDataChannel("sendChannel");
                    Debug.Log("Creating offer");
                    RTCSessionDescriptionAsyncOperation offer = _localConnection.CreateOffer();
                    while (!offer.IsDone) await Task.Yield();
                    Debug.Log("Offer created");
                    RTCSessionDescription offerDesc = offer.Desc;
                    _localConnection.SetLocalDescription(ref offerDesc);
                    Debug.Log("Sending offer");
                    await SendMessage(_webSocket, "SendSDPAnswer", offerDesc.sdp);
                    break;

                case "RecieveSDPAnswer":
                    //Receive the SDP answer from the other peer - Peer A
                    Debug.Log("Received SDP answer");
                    RTCSessionDescription sdpAnswer = new RTCSessionDescription();
                    sdpAnswer.type = RTCSdpType.Answer;
                    sdpAnswer.sdp = messageObject.MessageContent;
                    Debug.Log("Setting remote description");
                    var op2 = _localConnection.SetRemoteDescription(ref sdpAnswer);

                    while (!op2.IsDone) { Debug.Log("Waiting for remote description"); await Task.Yield(); }

                    break;

                case "SendSDPAnswer":
                    //Receive the SDP offer from the other peer and send back an answer - Peer B
                    Debug.Log("Received SDP offer");
                    RTCConfiguration configuration2 = new RTCConfiguration();
                    configuration2.iceServers = new RTCIceServer[]
                    {
                        new RTCIceServer
                        {
                            urls = new string[] { "turn:79.72.91.98:3478" },
                            username = "server14",
                            credential = "41server",
                            credentialType = RTCIceCredentialType.Password
                        },
                        new RTCIceServer
                        {
                            urls = new string[] { "stun:79.72.91.98:3478" }
                        }
                    };
                    _localConnection = new RTCPeerConnection(ref configuration2);
                    _localConnection.OnIceCandidate = async e => { Debug.Log("ICE candidate"); await SendMessage(webSocket, "ICECandidate", e.Candidate); };
                    _localConnection.OnIceConnectionChange = state =>
                    {
                        Debug.Log($"Local ICE connection state changed: {state}");
                        HandleIceConnectionStateChange(state);
                    };
                    RTCSessionDescription sdpOffer = new RTCSessionDescription();
                    sdpOffer.type = RTCSdpType.Offer;
                    sdpOffer.sdp = messageObject.MessageContent;
                    Debug.Log("SDP: " + sdpOffer.sdp);
                    Debug.Log("Creating remote connection");
                    var op3 = _localConnection.SetRemoteDescription(ref sdpOffer);
                    while (!op3.IsDone) { Debug.Log("Waiting for remote description"); await Task.Yield(); }

                    Debug.Log("Creating answer");
                    RTCSessionDescriptionAsyncOperation answer = _localConnection.CreateAnswer();
                    while (!answer.IsDone) { Debug.Log("Waiting for answer creation"); await Task.Yield(); }
                    RTCSessionDescription answerDesc = answer.Desc;
                    _localConnection.SetLocalDescription(ref answerDesc);

                    await SendMessage(_webSocket, "RecieveSDPAnswer", answerDesc.sdp);
                    break;

                case "ICECandidate":
                    //Receive ICE candidate from the other peer 
                    Debug.Log("Received ICE candidate");
                    RTCIceCandidateInit candidateInit = new RTCIceCandidateInit();
                    RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);
                    candidateInit.candidate = messageObject.MessageContent;
                    _localConnection.AddIceCandidate(candidate);
                    break;

                case "Disconnected":
                    //Close the connection
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connected", CancellationToken.None);
                    _webSocket.Dispose();
                    break;
            }
        }

        void HandleIceConnectionStateChange(RTCIceConnectionState state)
        {
            switch (state)
            {
                case RTCIceConnectionState.New:
                    Debug.Log("ICE connection state: New");
                    break;
                case RTCIceConnectionState.Checking:
                    Debug.Log("ICE connection state: Checking");
                    break;
                case RTCIceConnectionState.Connected:
                    Debug.Log("ICE connection state: Connected");
                    break;
                case RTCIceConnectionState.Completed:
                    Debug.Log("ICE connection state: Completed");
                    break;
                case RTCIceConnectionState.Disconnected:
                    Debug.Log("ICE connection state: Disconnected");
                    break;
                case RTCIceConnectionState.Failed:
                    Debug.Log("ICE connection state: Failed");
                    break;
                case RTCIceConnectionState.Closed:
                    Debug.Log("ICE connection state: Closed");
                    break;
                default:
                    Debug.Log("ICE connection state: Unknown");
                    break;
            }
        }

        async Task SendMessage(WebSocket webSocket, string type, string content)
        {
            string message = JsonUtility.ToJson(new ServerMessage { MessageType = type, MessageContent = content });
            Debug.Log($"Sending: {message}");
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

[Serializable]
public class ServerMessage
{
    public string MessageType;
    public string MessageContent;
}