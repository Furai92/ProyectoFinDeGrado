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
        RTCConfiguration? _configuration;
        RTCDataChannel _sendChannel;
        private string _password;
        private string _address = "79.72.91.98";
        private ushort _port = 80;
        public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
        {
            if (_sendChannel == null || _sendChannel.ReadyState != RTCDataChannelState.Open)
            {
                Debug.LogError("Data channel is not open.");
                return;
            }

            // Convert ArraySegment<byte> to byte[]
            byte[] buffer = new byte[data.Count];
            Array.Copy(data.Array, data.Offset, buffer, 0, data.Count);

            // Send the data over the data channel
            _sendChannel.Send(buffer);
            Debug.Log($"Sent {data.Count} bytes to client {clientId}");
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
            Shutdown();
        }

        public override void DisconnectLocalClient()
        {
            Shutdown();
        }

        public override ulong GetCurrentRtt(ulong clientId)
        {
            return 0;
        }

        public override void Shutdown()
        {
            Debug.Log("Shutting down WebRTC transport");

            // Close the data channels if they are open
            if (_sendChannel != null)
            {
                _sendChannel.Close();
                _sendChannel = null;
            }

            // Close the peer connection
            if (_localConnection != null)
            {
                _localConnection.Close();
                _localConnection = null;
            }

            // Close the WebSocket connection if it is open
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.None).Wait();
                _webSocket.Dispose();
                _webSocket = null;
            }

            Debug.Log("WebRTC transport shutdown complete");
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
                    RTCConfiguration configuration = GetRTCConfiguration();
                    _localConnection = new RTCPeerConnection(ref configuration);
                    _localConnection = new RTCPeerConnection();
                    _sendChannel = _localConnection.CreateDataChannel("sendChannel");
                    _sendChannel.OnOpen = () =>
                    {
                        Debug.Log("Data channel open");
                        _sendChannel.Send("Hello");
                    };
                    _sendChannel.OnClose = () => Debug.Log("Data channel closed");
                    _sendChannel.OnMessage = e => Debug.Log($"Received message: {Encoding.UTF8.GetString(e)}");
                    _localConnection.OnIceCandidate = async e => { Debug.Log("ICE candidate"); await SendMessage(webSocket, "ICECandidate", e.Candidate); };
                    _localConnection.OnIceConnectionChange = state =>
                    {
                        if (state == RTCIceConnectionState.Connected)
                        {
                            Debug.Log("Connection established");
                        }
                        Debug.Log($"Local ICE connection state changed: {state}");
                    };
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
                    RTCConfiguration configuration2 = GetRTCConfiguration();
                    _localConnection = new RTCPeerConnection(ref configuration2);
                    _localConnection = new RTCPeerConnection();
                    // Handle incoming data channels
                    _localConnection.OnDataChannel = channel =>
                    {
                        Debug.Log("Data channel received");
                        _sendChannel = channel;
                        _sendChannel.OnOpen = () =>
                        {
                            Debug.Log("Data channel open");
                            _sendChannel.Send("Bye");
                        };
                        _sendChannel.OnClose = () => Debug.Log("Data channel closed");
                        _sendChannel.OnMessage = e => Debug.Log($"Received message: {Encoding.UTF8.GetString(e)}");
                    };
                    _localConnection.OnIceCandidate = async e => { Debug.Log("ICE candidate"); await SendMessage(webSocket, "ICECandidate", e.Candidate); };
                    _localConnection.OnIceConnectionChange = state =>
                    {
                        if (state == RTCIceConnectionState.Completed)
                        {
                            Debug.Log("Connection established");
                            
                        }
                        Debug.Log($"Local ICE connection state changed: {state}");
                    };

                    RTCSessionDescription sdpOffer = new RTCSessionDescription();
                    sdpOffer.type = RTCSdpType.Offer;
                    sdpOffer.sdp = messageObject.MessageContent;
                    Debug.Log("Creating remote connection");
                    var op3 = _localConnection.SetRemoteDescription(ref sdpOffer);
                    Debug.Log("Setting remote description");
                     //while (!op3.IsDone) { Debug.Log("Waiting for remote description"); await Task.Yield(); }

                    Debug.Log("Creating answer");
                    RTCSessionDescriptionAsyncOperation answer = _localConnection.CreateAnswer();
                    while (!answer.IsDone) { Debug.Log("Waiting for answer creation"); await Task.Yield(); }
                    RTCSessionDescription answerDesc = answer.Desc;
                    _localConnection.SetLocalDescription(ref answerDesc);

                    await SendMessage(_webSocket, "RecieveSDPAnswer", answerDesc.sdp);
                    break;

                case "ICECandidate":
                    // Receive ICE candidate from the other peer 
                    Debug.Log("Received ICE candidate");
                    RTCIceCandidateInit candidateInit = new RTCIceCandidateInit();
                    Debug.Log("Setting ICE candidate");
                    candidateInit.candidate = messageObject.MessageContent;
                    candidateInit.sdpMid = "";
                    candidateInit.sdpMLineIndex = 0;
                    Debug.Log("Creating ICE candidate");
                    Debug.Log($"Parsed ICE candidate: {candidateInit.candidate}");
                    Debug.Log($"sdpMid: {candidateInit.sdpMid}");
                    Debug.Log($"sdpMLineIndex: {candidateInit.sdpMLineIndex}");
                    RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);
                    Debug.Log("Trying to add ICE candidate " + candidate);
                    _localConnection.AddIceCandidate(candidate);
                    Debug.Log("ICE candidate added " + candidate);
                    break;

                case "Disconnected":
                    //Close the connection
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connected", CancellationToken.None);
                    _webSocket.Dispose();
                    break;
            }
        }

        private RTCConfiguration GetRTCConfiguration()
        {
            if (_configuration.HasValue) return _configuration.Value;

            _configuration = new RTCConfiguration();
            var config = _configuration.GetValueOrDefault();
            config.iceServers = new RTCIceServer[]
            {
                        new RTCIceServer
                        {
                            urls = new string[] { "stun:79.72.91.98:3478" }
                        },
                        new RTCIceServer
                        {
                            urls = new string[] { "turn:79.72.91.98:3478" },
                            username = "server14",
                            credential = "41server",
                            credentialType = RTCIceCredentialType.Password
                        }
            };
            _configuration = config;
            return _configuration.Value;
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