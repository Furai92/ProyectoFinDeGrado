using System;

[Serializable]
public class ServerMessage
{
    public string MessageType;
    public string MessageContent;
    public string SdpMid;
    public int SdpMLineIndex;
}