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
            byte[] buffer = new byte[1024];
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

        private async Task OnRecieveSDPAnswer(ServerMessage messageObject)
        {
            //Receive the SDP answer from the other peer - Peer A
            Debug.Log("Received SDP answer");
            RTCSessionDescription sdpAnswer;
            sdpAnswer.type = RTCSdpType.Answer;
            sdpAnswer.sdp = messageObject.MessageContent;
            Debug.Log("Setting remote description");
            RTCSetSessionDescriptionAsyncOperation op2 = _localConnection.SetRemoteDescription(ref sdpAnswer);
            await WaitForOperation(op2);
            Debug.Log("finished setting remote description");
            SuscribeToICE();
        }

        private void SuscribeToICE()
        {
            _localConnection.OnIceCandidate = async e =>
            {
                Debug.Log("ICE candidate");
                await SendMessage("ICECandidate", e.Candidate);
            };
            _localConnection.OnIceConnectionChange = state =>
            {
                if (state == RTCIceConnectionState.Completed || state == RTCIceConnectionState.Connected)
                {
                    Debug.Log("Connection established");
                }
                Debug.Log($"Local ICE connection state changed: {state}");
            };
            Debug.Log("suscribed to events");
        }

        private void OnICECandidate(ServerMessage messageObject)
        {
            // Receive ICE candidate from the other peer 
            Debug.Log("Received ICE candidate");
            RTCIceCandidateInit candidateInit = new RTCIceCandidateInit();
            Debug.Log("Setting ICE candidate");
            candidateInit.candidate = messageObject.MessageContent;
            candidateInit.sdpMid = "";
            candidateInit.sdpMLineIndex = 0;
            Debug.Log("Creating ICE candidate");
            RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);
            Debug.Log("Trying to add ICE candidate " + candidate);
            _localConnection.AddIceCandidate(candidate);
            Debug.Log("ICE candidate added " + candidate);
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
            //Send the SDP offer to the other peer - Peer A
            Debug.Log("Pairing request received");
            _sendChannel = _localConnection.CreateDataChannel("data");
            _sendChannel.OnOpen = () =>
            {
                Debug.Log("Data channel open");
                _sendChannel.Send("hello");
            };
            _sendChannel.OnClose = () => Debug.Log("Data channel closed");
            _sendChannel.OnMessage = e => Debug.Log($"Received message: {Encoding.UTF8.GetString(e)}");
            Debug.Log("Started offer");
            
            // Start the CreateOffer operation
            RTCSessionDescriptionAsyncOperation offerOp = _localConnection.CreateOffer();
            Debug.Log("creating offer");
            await WaitForOperation(offerOp);
            RTCSessionDescription offer = offerOp.Desc;
            Debug.Log("Created offer: " + offer.sdp);
            Debug.Log("setting local description");
            RTCSetSessionDescriptionAsyncOperation op = _localConnection.SetLocalDescription(ref offer);
            await WaitForOperation(op);
            Debug.Log("finished setting local description");
            await SendMessage("SendSDPAnswer", offer.sdp);
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
            Debug.Log("Finished creating answer process");
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
            if (_configuration.HasValue) return _configuration.Value;

            _configuration = new RTCConfiguration();
            var config = _configuration.GetValueOrDefault();
            config.iceServers = new RTCIceServer[]
            {
                        // new RTCIceServer
                        // {
                        //     urls = new string[] { "stun:79.72.91.98:3478" }
                        // },
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
        async Task SendMessage(string type, string content)
        {
            string message = JsonUtility.ToJson(new ServerMessage { MessageType = type, MessageContent = content });
            Debug.Log($"Sending: {message}");
            await _webSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        
        #endregion
    }
}