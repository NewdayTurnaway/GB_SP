using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SolarSystemGame
{
    public class PlayerInitialSetup : MonoBehaviour
    {
        [field: SerializeField] public List<Transform> SpawnPoints { get; private set; }
        [field: SerializeField] public string PlayerName { get; private set; }

        private void Start()
        {
            PlayerName = $"PlayerInstanceID: {GetInstanceID()}";
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

            var player = NetworkManager.Singleton.LocalClient.PlayerObject;
            var data = new PlayerNetworkData() { Name = PlayerName, Score = 0 };
            player.GetComponent<SpaceshipNetworkController>().SetPlayerData(data);
            player.transform.position = GetRandomPosition();
        }

        public Vector3 GetRandomPosition()
        {
            var index = Random.Range(0, SpawnPoints.Count);
            return SpawnPoints[index].position;
        }
    } 
}
