using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolarSystemGame
{
    [RequireComponent(typeof(NetworkObject))]
    public class CrystalsController : NetworkBehaviour
    {
        [field: SerializeField] public UIGameRecord UIGameRecord { get; private set; }
        [field: SerializeField] public Crystal CrystalPrefab { get; private set; }
        [field: SerializeField] public List<Transform> CrystalSpawnPoints { get; private set; }

        private readonly NetworkVariable<bool> _isCreated = new(false);

        private readonly Dictionary<string, int> _gameRecords = new();
        private event Action _allCrystalsCollected;

        private bool _stopUpdate = false;

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnServerStarted += SetCrystals;
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowRecord;
            _allCrystalsCollected += () => ShowRecord(NetworkManager.Singleton.LocalClientId);
        }

        private void FixedUpdate()
        {
            if (!_isCreated.Value || _stopUpdate)
            {
                return;
            }

            var objects = SceneManager.GetActiveScene().GetRootGameObjects();
            var count = 0;
            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].TryGetComponent(out Crystal crystal))
                {
                    continue;
                }
                count = crystal.gameObject.activeSelf == false ? count + 1 : count;
            }

            if (count == CrystalSpawnPoints.Count)
            {
                _stopUpdate = false;
                _allCrystalsCollected?.Invoke();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                return;
            }

            _allCrystalsCollected -= () => ShowRecord(NetworkManager.Singleton.LocalClientId);

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= SetCrystals;
                NetworkManager.Singleton.OnClientDisconnectCallback -= ShowRecord;
            }
        }

        private void SetCrystals()
        {
            for (int i = 0; i < CrystalSpawnPoints.Count; i++)
            {
                var crystal = Instantiate(CrystalPrefab, CrystalSpawnPoints[i].position, CrystalPrefab.transform.rotation);
                crystal.GetComponent<NetworkObject>().Spawn();
            }

            _isCreated.Value = true;
            NetworkManager.Singleton.OnServerStarted -= SetCrystals;
        }

        private void ShowRecord(ulong clientId)
        {
            var objects = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].TryGetComponent(out SpaceshipNetworkController shipController))
                {
                    continue;
                }

                shipController.gameObject.SetActive(false);

                var data = shipController.PlayerData.Value;
                _gameRecords.Add(data.Name, data.Score);
            }
            UIGameRecord.gameObject.SetActive(true);
            UIGameRecord.Text.text = "";
            foreach (var gameRecord in _gameRecords)
            {
                UIGameRecord.Text.text += $"{gameRecord.Key} : {gameRecord.Value}\n";
            }
            _gameRecords.Clear();

            ResetFlagServerRpc();

            _allCrystalsCollected -= () => ShowRecord(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowRecord;
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetFlagServerRpc()
        {
            _isCreated.Value = false;
        }
    } 
}
