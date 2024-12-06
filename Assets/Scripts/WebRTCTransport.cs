using System;
using Unity.Netcode;
using System.Net.WebSockets;
using Unity.WebRTC;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

namespace Netcode.Transports.WebRTCTransport
{
    public class WebRTCTransport : NetworkTransport
    {
        private ClientWebSocket _webSocket;
        private RTCPeerConnection _localConnection;
        private RTCDataChannel _sendChannel;
        private string _address = "79.72.91.98";
        private ushort _port = 80;
        private Queue<(byte[] data, float timestamp)> _messageQueue = new Queue<(byte[] data, float timestamp)>();
        private object _queueLock = new object();
        private bool _isInitialized = false;

        public override void Send(ulong clientId, ArraySegment<byte> data, NetworkDelivery delivery)
        {
            if (_sendChannel == null || _sendChannel.ReadyState != RTCDataChannelState.Open)
            {
                Debug.LogError("Data channel is not open.");
                return;
            }

            byte[] buffer = new byte[data.Count];
            if (data.Array != null) Array.Copy(data.Array, data.Offset, buffer, 0, data.Count);

            _sendChannel.Send(buffer);
            Debug.Log($"Sent {data.Count} bytes to client {clientId}");
        }

        public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
        {
            clientId = 0;
            payload = new ArraySegment<byte>();
            receiveTime = 0.0f;
            try
            {
                if (_sendChannel == null || _sendChannel.ReadyState != RTCDataChannelState.Open)
                {
                    return NetworkEvent.Disconnect;
                }

                lock (_queueLock)
                {
                    if (_messageQueue.Count > 0)
                    {
                        var (data, timestamp) = _messageQueue.Dequeue();
                        payload = new ArraySegment<byte>(data);
                        receiveTime = timestamp;
                        return NetworkEvent.Data;
                    }
                }
                return NetworkEvent.Nothing;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in PollEvent: {ex.Message}");
                return NetworkEvent.Disconnect;
            }
        }

        public override ulong GetCurrentRtt(ulong clientId)
        {
            return 0;
        }

        public override bool StartClient()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("Client already initialized");
                return false;
            }

            try 
            {
                Initialize();
                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start client: {ex}");
                return false;
            }
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

        public override void Shutdown()
        {
            Debug.Log("Shutting down");

            if (_sendChannel != null)
            {
                _sendChannel.Close();
                _sendChannel = null;
            }

            if (_localConnection != null)
            {
                _localConnection.Close();
                _localConnection = null;
            }

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown", CancellationToken.None).Wait();
                _webSocket.Dispose();
                _webSocket = null;
            }

            _isInitialized = false;

            Debug.Log("Shutdown complete");
        }

        public override async void Initialize(NetworkManager networkManager = null)
        {
            _webSocket = new ClientWebSocket();
            RTCConfiguration configuration = GetRTCConfiguration();
            _localConnection = new RTCPeerConnection(ref configuration);
            await ConnectWebSocket();

            byte[] buffer = new byte[4096];
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log("Received: " + message);
                await HandleMessage(_webSocket, message);
            }
        }

        private async Task ConnectWebSocket()
        {
            try
            {
                await _webSocket.ConnectAsync(new Uri($"ws://{_address}:{_port}"), CancellationToken.None);
                Debug.Log("WebSocket connected.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket connection failed: {ex.Message}");
            }
        }

        public override ulong ServerClientId { get; } = (ulong)Guid.NewGuid().GetHashCode();

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

                default:
                    Debug.LogWarning($"Unknown message type: {messageObject.MessageType}");
                    break;
            }
        }

        private async Task OnPassword()
        {
            await SendMessage("ConnectionRequest", "");
        }

        private void SubscribeToICE()
        {
            _localConnection.OnIceCandidate = e =>
            {
                Debug.Log("ICE candidate gathering");
                if (e.Candidate != null)
                {
                    Debug.Log("New ICE candidate");
                    _ = SendMessage("ICECandidate", e.Candidate, e.SdpMid, e.SdpMLineIndex ?? 0);
                }
                else
                {
                    Debug.Log("ICE candidate gathering complete");
                }
            };

            _localConnection.OnIceConnectionChange = state =>
            {
                Debug.Log($"Local ICE connection state changed: {state}");

                switch (state)
                {
                    case RTCIceConnectionState.Connected:
                        Debug.Log("ICE connection established");
                        break;
                    case RTCIceConnectionState.Disconnected:
                        Debug.Log("ICE connection lost");
                        break;
                    case RTCIceConnectionState.Failed:
                        Debug.LogError("ICE connection failed");
                        break;
                    case RTCIceConnectionState.Completed:
                        Debug.Log("ICE connection completed");
                        break;
                    default:
                        Debug.LogWarning($"Unknown ICE connection state: {state}");
                        break;
                }
            };
        }

        private void OnICECandidate(ServerMessage messageObject)
        {
            if (string.IsNullOrEmpty(messageObject.MessageContent))
            {
                Debug.LogWarning("Received empty ICE candidate, skipping.");
                return;
            }

            Debug.Log($"Received ICE candidate: {messageObject.MessageContent}");

            RTCIceCandidateInit candidateInit = new RTCIceCandidateInit
            {
                candidate = messageObject.MessageContent,
                sdpMid = messageObject.SdpMid,
                sdpMLineIndex = messageObject.SdpMLineIndex
            };

            RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);

            if (_localConnection != null)
            {
                bool success = _localConnection.AddIceCandidate(candidate);
                if (success)
                {
                    Debug.Log("ICE candidate added successfully.");
                }
                else
                {
                    Debug.LogError("Failed to add ICE candidate.");
                }
            }
            else
            {
                Debug.LogWarning("Local connection not initialized.");
            }
        }

        private async Task OnDisconnected()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
            _webSocket.Dispose();
        }

        private async Task OnSendSDPOffer()
        {
            Debug.Log("Pairing request received");
            _sendChannel = _localConnection.CreateDataChannel("send");

            RTCSessionDescriptionAsyncOperation offerOp = _localConnection.CreateOffer();
            await WaitForOperation(offerOp);

            RTCSessionDescription offer = offerOp.Desc;

            RTCSetSessionDescriptionAsyncOperation op = _localConnection.SetLocalDescription(ref offer);
            await WaitForOperation(op);

            SubscribeToICE();

            await SendMessage("SendSDPAnswer", offer.sdp);
        }

        private async Task OnRecieveSDPAnswer(ServerMessage messageObject)
        {
            RTCSessionDescription sdpAnswer = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = messageObject.MessageContent
            };

            RTCSetSessionDescriptionAsyncOperation op2 = _localConnection.SetRemoteDescription(ref sdpAnswer);
            await WaitForOperation(op2);

            SetupDataChannelCallbacks(_sendChannel);
        }

        private async Task OnSendSDPAnswer(ServerMessage messageObject)
        {
            RTCSessionDescription sdpOffer = new RTCSessionDescription
            {
                type = RTCSdpType.Offer,
                sdp = messageObject.MessageContent
            };

            RTCSetSessionDescriptionAsyncOperation op3 = _localConnection.SetRemoteDescription(ref sdpOffer);
            await WaitForOperation(op3);

            RTCSessionDescriptionAsyncOperation answer = _localConnection.CreateAnswer();
            await WaitForOperation(answer);

            RTCSessionDescription answerDesc = answer.Desc;
            RTCSetSessionDescriptionAsyncOperation op4 = _localConnection.SetLocalDescription(ref answerDesc);
            await WaitForOperation(op4);

            SubscribeToICE();

            await SendMessage("RecieveSDPAnswer", answerDesc.sdp);

            _localConnection.OnDataChannel = channel =>
            {
                _sendChannel = channel;
                SetupDataChannelCallbacks(channel);
                _sendChannel.Send("hello");
            };
        }

        private void SetupDataChannelCallbacks(RTCDataChannel channel)
        {
            channel.OnOpen = () => Debug.Log("Data channel is open");
            channel.OnClose = () => Debug.Log("Data channel closed");
            channel.OnMessage = bytes => 
            {
                lock (_queueLock)
                {
                    _messageQueue.Enqueue((bytes, Time.realtimeSinceStartup));
                    Debug.Log($"Received {bytes.Length} bytes");
                    Debug.Log("Trying to decode message as string: " + Encoding.UTF8.GetString(bytes));
                }
            };
        }

        private async Task WaitForOperation(AsyncOperationBase operation)
        {
            while (!operation.IsDone && !operation.IsError)
            {
                await Task.Yield();
            }

            if (operation.IsError)
            {
                Debug.LogError($"Operation failed: {operation.Error}");
            }
        }

        private RTCConfiguration GetRTCConfiguration()
{
    return new RTCConfiguration
    {
        iceServers = new RTCIceServer[]
        {
            new RTCIceServer
            {
                urls = new string[] { "stun:stun.relay.metered.ca:80" }
            },
            new RTCIceServer
            {
                urls = new string[] { "turn:global.relay.metered.ca:80" },
                username = "1ff01721b24d7e5ca58007f4",
                credential = "tgKQZK+k/T6CLuUs",
                credentialType = RTCIceCredentialType.Password
            },
            new RTCIceServer
            {
                urls = new string[] { "turn:global.relay.metered.ca:80?transport=tcp" },
                username = "1ff01721b24d7e5ca58007f4",
                credential = "tgKQZK+k/T6CLuUs",
                credentialType = RTCIceCredentialType.Password
            },
            new RTCIceServer
            {
                urls = new string[] { "turn:global.relay.metered.ca:443" },
                username = "1ff01721b24d7e5ca58007f4",
                credential = "tgKQZK+k/T6CLuUs",
                credentialType = RTCIceCredentialType.Password
            },
            new RTCIceServer
            {
                urls = new string[] { "turns:global.relay.metered.ca:443?transport=tcp" },
                username = "1ff01721b24d7e5ca58007f4",
                credential = "tgKQZK+k/T6CLuUs",
                credentialType = RTCIceCredentialType.Password
            }
        },
        iceTransportPolicy = RTCIceTransportPolicy.All
    };
}
        private async Task SendMessage(string type, string content, string sdpMid = null, int sdpMLineIndex = 0)
        {
            ServerMessage message = new ServerMessage
            {
                MessageType = type,
                MessageContent = content,
                SdpMid = sdpMid,
                SdpMLineIndex = sdpMLineIndex
            };

            string jsonMessage = JsonUtility.ToJson(message);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonMessage);

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Debug.Log($"Sent message: {jsonMessage}");
            }
            else
            {
                Debug.LogError("WebSocket is not connected.");
            }
        }
    }
}