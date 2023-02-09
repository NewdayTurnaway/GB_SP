using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private const int PORT = 5805;
    private const int BUFFER_SIZE = 1024;
    private const string USER_COLOR = "#FFC100";
    private const string USER_MENTION_COLOR = "#00FFA4";

    private readonly List<int> _connectionIDs = new();
    private readonly Dictionary<int, string> _users = new();

    private int _hostID;
    private int _reliableChannel;
    private bool _isStarted = false;
    private byte _error;

    [System.Obsolete]
    public void StartServer()
    {
        NetworkTransport.Init();

        var connectionConfig = new ConnectionConfig();

        _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);

        var topology = new HostTopology(connectionConfig, MAX_CONNECTION);

        _hostID = NetworkTransport.AddHost(topology, PORT);
        _isStarted = true;
    }

    [System.Obsolete]
    public void ShutDownServer()
    {
        if (!_isStarted)
        {
            return;
        }

        NetworkTransport.RemoveHost(_hostID);
        NetworkTransport.Shutdown();
        _isStarted = false;
    }

    [System.Obsolete]
    private void Update()
    {
        if (!_isStarted)
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
                    _connectionIDs.Add(connectionId);
                    _users.Add(connectionId, "");
                    break;

                case NetworkEventType.DataEvent:
                    var message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    message = CheckMessage(message);
                    if (_users[connectionId] != "")
                    {
                        ServerSendMessageToAll($"<link=\"@{_users[connectionId]}\"><{USER_COLOR}>{_users[connectionId]}:</color></link> {message}");
                        Debug.Log($"{_users[connectionId]}: {message}");
                    }
                    else
                    {
                        _users[connectionId] = message;
                        Debug.Log($"ID: {connectionId} = User: {_users[connectionId]}");
                    }
                    
                    break;

                case NetworkEventType.DisconnectEvent:
                    _connectionIDs.Remove(connectionId);
                    ServerSendMessageToAll($"<link=\"@{_users[connectionId]}\"><{USER_COLOR}>{_users[connectionId]}</color></link> has disconnected.");
                    Debug.Log($"{_users[connectionId]} has disconnected.");
                    _users.Remove(connectionId);
                    break;

                case NetworkEventType.BroadcastEvent:
                    break;
            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BUFFER_SIZE,
                out dataSize, out _error);
        }
    }

    [System.Obsolete]
    public void ServerSendMessageToAll(string message)
    {
        for (int i = 0; i < _connectionIDs.Count; i++)
        {
            ServerSendMessage(message, _connectionIDs[i]);
        }
    }

    [System.Obsolete]
    public void ServerSendMessage(string message, int connectionID)
    {
        var buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer, message.Length * sizeof(char), 
            out _error);

        if ((NetworkError)_error != NetworkError.Ok)
        {
            Debug.Log((NetworkError)_error);
        }
    }

    private string CheckMessage(string message)
    {
        var newMessage = message;
        var regex = new Regex(@"[@.*]\w+");
        var matchCollection = regex.Matches(message);

        for (int i = 0; i < matchCollection.Count; i++)
        {
            var newValue = matchCollection[i].Value.Remove(0, 1);

            if (!_users.ContainsValue(newValue))
            {
                continue;
            }

            newValue = $"<link=\"@{newValue}\"><{USER_MENTION_COLOR}>{newValue}</color></link>";
            newMessage = newMessage.Replace(matchCollection[i].Value, newValue);
        }

        return newMessage;
    }
}