using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIInit : MonoBehaviour
{
    private const string PLACEHOLDET_TEXT = "Enter name to login";

    [field: SerializeField] public TMP_InputField InputField { get; private set; }
    [field: SerializeField] public Button HostButton { get; private set; }
    [field: SerializeField] public Button ClientButton { get; private set; }
    [field: SerializeField] public PlayerObjectInitialSetup InitialSetup { get; private set; }

    private void Start()
    {
        HostButton.onClick.AddListener(StartHost);
        ClientButton.onClick.AddListener(StartClient);
    }

    private void OnDestroy()
    {   
        HostButton.onClick.RemoveListener(StartHost);
        ClientButton.onClick.RemoveListener(StartClient);
    }

    private void StartHost()
    {
        if (!CheckInputText())
        {
            return;
        }

        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    private void StartClient()
    {
        if (!CheckInputText())
        {
            return;
        }

        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }

    private bool CheckInputText()
    {
        if (InputField.text == "")
        {
            var placeholder = (TextMeshProUGUI)InputField.placeholder;
            placeholder.color = Color.red;
            placeholder.text = PLACEHOLDET_TEXT;
            return false;
        }

        InitialSetup.SetPlayerName(InputField.text);
        return true;
    }
}
