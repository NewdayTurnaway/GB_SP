using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerObjectInitialSetup : MonoBehaviour
{
    [field: SerializeField] public List<Transform> SpawnPoints { get; private set; }
    [field: SerializeField] public string PlayerName { get; private set; } = "Player";

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    public void SetPlayerName(string newName)
    {
        PlayerName = newName;
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        NetworkManager.Singleton.LocalClient.PlayerObject.name = PlayerName;
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<ShipController>().PlayerName = PlayerName;
        NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = GetRandomPosition();
    }

    public Vector3 GetRandomPosition()
    {
        var index = Random.Range(0, SpawnPoints.Count);
        return SpawnPoints[index].position;
    }
}
