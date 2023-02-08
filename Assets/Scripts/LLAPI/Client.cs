using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private const int PORT = 0;
    private const string ADDRESS = "127.0.0.1";
    private const int SERVER_PORT = 5805;
    private const int BUFFER_SIZE = 1024;

    private int _hostID;
    private int _reliableChannel;
    private int _connectionID;
    private bool _isConnected = false;
    private byte _error;

    public string UserName { get; private set; }
    
    public event Action<string> OnMessageReceived;

    public void AcceptUserName(string name)
    {
        UserName = name;
    }

    [Obsolete]
    public void Connect()
    {
        NetworkTransport.Init();

        var connectionConfig = new ConnectionConfig();

        _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
        
        var topology = new HostTopology(connectionConfig, MAX_CONNECTION);
        
        _hostID = NetworkTransport.AddHost(topology, PORT);
        _connectionID = NetworkTransport.Connect(_hostID, ADDRESS, SERVER_PORT, 0, out _error);

        if ((NetworkError)_error == NetworkError.Ok)
        {
            _isConnected = true;
        }
        else
        {
            Debug.Log((NetworkError)_error);
        }
    }

    [Obsolete]
    public void Disconnect()
    {
        if (!_isConnected)
        {
            return;
        }

        NetworkTransport.Disconnect(_hostID, _connectionID, out _error);
        _isConnected = false;
    }

    [Obsolete]
    private void Update()
    {
        if (!_isConnected)
        {
            return;
        }

        var recBuffer = new byte[BUFFER_SIZE];
        var recData = NetworkTransport.Receive(
            out int recHostId,
            out int connectionId,
            out int channelId,
            recBuffer,
            BUFFER_SIZE,
            out int dataSize,
            out _error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    OnMessageReceived?.Invoke($"You have been connected to server.");
                    ClientSendMessage(UserName);
                    Debug.Log($"You have been connected to server.");
                    break;
                case NetworkEventType.DataEvent:
                    var message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    OnMessageReceived?.Invoke(message);
                    Debug.Log(message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    _isConnected = false;
                    OnMessageReceived?.Invoke($"You have been disconnected from server.");
                    Debug.Log($"You have been disconnected from server.");
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BUFFER_SIZE,
                out dataSize, out _error);
        }
    }

    [Obsolete]
    public void ClientSendMessage(string message)
    {
        var buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_hostID, _connectionID, _reliableChannel, buffer, message.Length * sizeof(char), 
            out _error);

        if ((NetworkError)_error != NetworkError.Ok)
        {
            Debug.Log((NetworkError)_error);
        }
    }
}
