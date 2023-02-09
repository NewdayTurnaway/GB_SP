using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] private Button _startServerButton;
    [SerializeField] private Button _shutDownServerButton;
    [SerializeField] private Button _connectClientButton;
    [SerializeField] private Button _disconnectClientButton;
    [SerializeField] private Button _sendMessageButton;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextField _textField;
    [SerializeField] private Server _server;
    [SerializeField] private Client _client;

    [System.Obsolete]
    private void Start()
    {
        _startServerButton.interactable = true;
        _connectClientButton.interactable = true;

        _shutDownServerButton.interactable = false;
        _disconnectClientButton.interactable = false;
        _sendMessageButton.interactable = false;

        _startServerButton.onClick.AddListener(() => StartServer());
        _shutDownServerButton.onClick.AddListener(() => ShutDownServer());
        _connectClientButton.onClick.AddListener(() => PreSettings());
        _disconnectClientButton.onClick.AddListener(() => Disconnect());
        
        _client.OnMessageReceived += ReceiveMessage;
        _textField.CurrentSelectedUsername += InsertUsername;
    }

    private void OnDestroy()
    {
        _textField.CurrentSelectedUsername -= InsertUsername;
    }

    private void InsertUsername(string username)
    {
        if(username == $"@{_client.UserName})")
        {
            return;
        }

        _inputField.text += username;
    }

    public void ReceiveMessage(string message)
    {
        _textField.ReceiveMessage(message);
    }

    [System.Obsolete]
    private void StartServer()
    {
        _server.StartServer();
        _startServerButton.interactable = false;
        _shutDownServerButton.interactable = true;
    }

    [System.Obsolete]
    private void ShutDownServer()
    {
        _server.ShutDownServer();
        _shutDownServerButton.interactable = false;
        _startServerButton.interactable = true;
    }

    [System.Obsolete]
    private void PreSettings()
    {
        _textField.ReceiveMessage("Enter your name");
        _connectClientButton.interactable = false;
        _disconnectClientButton.interactable = true;
        _sendMessageButton.onClick.AddListener(Connect);
        _sendMessageButton.interactable = true;
    }

    [System.Obsolete]
    private void Connect()
    {
        _client.AcceptUserName(_inputField.text);
        _inputField.text = "";
        _client.Connect();
        _sendMessageButton.onClick.RemoveListener(Connect);
        _sendMessageButton.onClick.AddListener(() => SendMessage());
    }

    [System.Obsolete]
    private void Disconnect()
    {
        _client.Disconnect();
        _disconnectClientButton.interactable = false;
        _connectClientButton.interactable = true;
    }

    [System.Obsolete]
    private void SendMessage()
    {
        _client.ClientSendMessage(_inputField.text);
        _inputField.text = "";
    }
}
