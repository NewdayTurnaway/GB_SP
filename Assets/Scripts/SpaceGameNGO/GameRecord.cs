using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRecord : NetworkBehaviour
{
    [field: SerializeField] public UIGameRecord UIGameRecord { get; private set; }
    [field: SerializeField] public GameObject CrystalPrefab { get; private set; }
    [field: SerializeField] public List<Transform> CrystalSpawnPoints { get; private set; }
    
    private readonly NetworkVariable<bool> _isCreated = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private readonly Dictionary<string, int> _gameRecords = new();
    private event Action _allÑrystalsÑollected;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnServerStarted += SetCrystals;
        NetworkManager.Singleton.OnClientDisconnectCallback += ShowRecord;
        _allÑrystalsÑollected += () => ShowRecord(NetworkManager.Singleton.LocalClientId);
    }

    private void FixedUpdate()
    {
        if (!_isCreated.Value)
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
            _allÑrystalsÑollected?.Invoke();
            _isCreated.Value = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _allÑrystalsÑollected -= () => ShowRecord(NetworkManager.Singleton.LocalClientId);

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
            if(!objects[i].TryGetComponent(out ShipController shipController))
            {
                continue;
            }

            _gameRecords.Add(shipController.PlayerName, shipController.Score.Value);
        }
        UIGameRecord.gameObject.SetActive(true);

        foreach (var gameRecord in _gameRecords)
        {
            UIGameRecord.Text.text += $"{gameRecord.Key} : {gameRecord.Value}\n";
        }
        _gameRecords.Clear();

        _allÑrystalsÑollected -= () => ShowRecord(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.OnClientDisconnectCallback -= ShowRecord;
    }
}
