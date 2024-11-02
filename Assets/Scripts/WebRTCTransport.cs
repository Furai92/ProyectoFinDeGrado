using System;
using Unity.Netcode;
using System.Net.WebSockets;
using Unity.WebRTC;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Netcode.Transports.WebRTCTransport
{
    public class WebRTCTransport : NetworkTransport
    {
        private ClientWebSocket _webSocket;
        RTCPeerConnection _localConnection;
        RTCConfiguration? _configuration;
        RTCDataChannel _sendChannel;
        private string _address = "79.72.91.98";
        //private string _address = "127.0.0.1";
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
            if (data.Array != null) Array.Copy(data.Array, data.Offset, buffer, 0, data.Count);

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
            //TO DO
            return 0;
        }

        public override void Shutdown()
        {
            Debug.Log("Shutting down");

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

            Debug.Log("shutdown complete");
        }
        
        #region General

        public override async void Initialize(NetworkManager networkManager = null)
        {
            _webSocket = new ClientWebSocket();
            RTCConfiguration configuration = GetRTCConfiguration();
            _localConnection = new RTCPeerConnection(ref configuration);
            await _webSocket.ConnectAsync(new Uri($"ws://{_address}:{_port}"), CancellationToken.None);
            byte[] buffer = new byte[4096];
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Received: " + message);
                await HandleMessage(_webSocket, message);
            }
        }

        public override ulong ServerClientId { get; }

        async Task HandleMessage(WebSocket webSocket, string message)
        {
            ServerMessage messageObject = JsonUtility.FromJson<ServerMessage>(message);

            switch (messageObject.MessageType)
            {
                case "Password":
                    await OnPassword();
                    break;

                case "SendSDPOffer":
                    await OnSendSDPOffer();
                    break;

                case "RecieveSDPAnswer":
                    await OnRecieveSDPAnswer(messageObject);
                    break;

                case "SendSDPAnswer":
                    await OnSendSDPAnswer(messageObject);
                    break;

                case "ICECandidate":
                    OnICECandidate(messageObject);
                    break;

                case "Disconnected":
                    await OnDisconnected();
                    break;
            }
        }

        private async Task OnPassword()
        {
            await SendMessage("ConnectionRequest", "");
        }

        private void SuscribeToICE()
        {
            _localConnection.OnIceCandidate = e =>
            {
                if (e.Candidate != null)
                {
                    Debug.Log("New ICE candidate");
                    _ = SendMessage("ICECandidate", e.Candidate);
                }
                else
                {
                    Debug.Log("ICE candidate gathering complete");
                }
            };

            _localConnection.OnIceConnectionChange = state =>
            {
                Debug.Log($"Local ICE connection state changed: {state}");
        
                if (state == RTCIceConnectionState.Connected)
                {
                    Debug.Log("ICE connection established");
                }
                else if (state == RTCIceConnectionState.Disconnected || state == RTCIceConnectionState.Failed)
                {
                    Debug.Log("ICE connection lost");
                }
            };
        }

        private void OnICECandidate(ServerMessage messageObject)
        {
            // Receive ICE candidate from the other peer 
            if (string.IsNullOrEmpty(messageObject.MessageContent))
            {
                Debug.LogWarning("Received empty ICE candidate, skipping.");
                return;
            }

            Debug.Log("Received ICE candidate");

            // Populate candidate details
            RTCIceCandidateInit candidateInit = new RTCIceCandidateInit
            {
                candidate = messageObject.MessageContent,
                sdpMid = "",
                sdpMLineIndex = 0
            };

            Debug.Log("Creating ICE candidate");
            RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);

            // Attempt to add the candidate
            Debug.Log("Trying to add ICE candidate " + candidate);
            bool success = _localConnection.AddIceCandidate(candidate);
            if (success)
            {
                Debug.Log("ICE candidate added successfully: " + candidate);
            }
            else
            {
                Debug.LogError("Failed to add ICE candidate: " + candidate);
            }
        }
        
        private void OnDestroy()
        {
            _sendChannel.Close();
            _localConnection.Close();
        }

        private async Task OnDisconnected()
        {
            //Close the connection
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connected", CancellationToken.None);
            _webSocket.Dispose();
        }

        private async Task OnSendSDPOffer()
        {
            Debug.Log("Pairing request received");
            Debug.Log("Started offer");

            _sendChannel = _localConnection.CreateDataChannel("send");

            // Start the CreateOffer operation
            RTCSessionDescriptionAsyncOperation offerOp = _localConnection.CreateOffer();
            Debug.Log("Creating offer");

            await WaitForOperation(offerOp);
            if (offerOp.IsError)
            {
                Debug.LogError($"Offer creation failed: {offerOp.Error}");
                return;
            }

            RTCSessionDescription offer = offerOp.Desc;
            Debug.Log("Created offer: " + offer.sdp);

            Debug.Log("Setting local description");
            RTCSetSessionDescriptionAsyncOperation op = _localConnection.SetLocalDescription(ref offer);
            await WaitForOperation(op);
            if (op.IsError)
            {
                Debug.LogError($"Setting local description failed: {op.Error}");
                return;
            }

            Debug.Log("Finished setting local description");
            await SendMessage("SendSDPAnswer", offer.sdp);
        }
        
        private async Task OnRecieveSDPAnswer(ServerMessage messageObject)
        {
            Debug.Log("Received SDP answer");
            RTCSessionDescription sdpAnswer = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = messageObject.MessageContent
            };
            Debug.Log("Setting remote description");
            RTCSetSessionDescriptionAsyncOperation op2 = _localConnection.SetRemoteDescription(ref sdpAnswer);
            await WaitForOperation(op2);
            if (op2.IsError)
            {
                Debug.LogError($"Setting remote description failed: {op2.Error}");
                return;
            }
            Debug.Log("Finished setting remote description");
            SuscribeToICE();
            // Set up channel callbacks
            _sendChannel.OnClose = () => Debug.Log("Data channel closed");
            _sendChannel.OnMessage = e =>
            {
                Debug.Log($"Received message: {Encoding.UTF8.GetString(e)}");
                _sendChannel.Send("hello");
            };
        }

        private async Task OnSendSDPAnswer(ServerMessage messageObject)
        {
            //Receive the SDP offer from the other peer and send back an answer - Peer B
            Debug.Log("Received SDP offer");
            RTCSessionDescription sdpOffer = new RTCSessionDescription();
            sdpOffer.type = RTCSdpType.Offer;
            sdpOffer.sdp = messageObject.MessageContent;
            Debug.Log("Setting remote description");
            var op3 = _localConnection.SetRemoteDescription(ref sdpOffer);
            await WaitForOperation(op3);
            Debug.Log("Finished Setting remote description");
            
            Debug.Log("Creating answer");
            RTCSessionDescriptionAsyncOperation answer = _localConnection.CreateAnswer();
            await WaitForOperation(answer);
            Debug.Log("Finished creating answer");
            RTCSessionDescription answerDesc = answer.Desc;
            
            Debug.Log("setting local description");
            var op4 = _localConnection.SetLocalDescription(ref answerDesc);
            await WaitForOperation(op4);
            Debug.Log("finished setting local description");
            Debug.Log("Finished creating answer process");
            await SendMessage("RecieveSDPAnswer", answerDesc.sdp);
            SuscribeToICE();
            _localConnection.OnDataChannel = channel =>
            {
                Debug.Log("Data channel received");
                _sendChannel = channel;
                Debug.Log(_sendChannel.ReadyState);
                _sendChannel.OnClose = () => Debug.Log("Data channel closed");
                _sendChannel.OnMessage = e => Debug.Log($"Received message: {Encoding.UTF8.GetString(e)}");
                _sendChannel.Send("bye");
            };
        }
        
        private async Task WaitForOperation(AsyncOperationBase operation)
        {
            while (!operation.IsDone && !operation.IsError)
            {
                await Task.Yield();
                Debug.Log("Waiting for operation" + operation);
            }
            Debug.Log("Finished operation");
            if (operation.IsError)
            {
                Debug.LogError("Operation failed: " + operation.Error);
            }
        }

        private RTCConfiguration GetRTCConfiguration()
        {
            _configuration = new RTCConfiguration
            {
                iceServers = new RTCIceServer[]
                {
                    new RTCIceServer
                    {
                        urls = new string[] { "stun:stun.l.google.com:19302" }
                    },
                    new RTCIceServer
                    {
                        urls = new string[] { "turn:79.72.91.98:3478" },
                        username = "server14",
                        credential = "41server",
                        credentialType = RTCIceCredentialType.Password
                    }
                }
            };

            return _configuration.Value;
        }
        async Task SendMessage(string type, string content)
        {
            string message = JsonUtility.ToJson(new ServerMessage { MessageType = type, MessageContent = content });
            Debug.Log($"Sending: {message}");
            await _webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        #endregion
    }
}