using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class NetworkMovableObject : NetworkBehaviour, INetworkUpdateSystem
{
    protected abstract float Speed { get; }

    protected event Action OnFixedUpdateAction;
    protected event Action OnUpdateAction;
    protected event Action OnLateUpdateAction;

    protected NetworkVariable<Vector3> _serverPosition = new();
    protected NetworkVariable<Vector3> _serverEuler = new();

    public NetworkVariable<int> Score = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        Initiate();
    }

    public override void OnNetworkDespawn()
    {
        OnFixedUpdateAction -= Movement;
        OnUpdateAction -= Movement;
        OnLateUpdateAction -= Movement;
        this.UnregisterAllNetworkUpdates();
    }

    private void LateUpdate()
    {
        DoSomethingClientRpc();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Crystal crystal))
        {
            crystal.gameObject.SetActive(false);
            AddScoreServerRpc();
        }

        if (!collision.gameObject.TryGetComponent(out ShipController shipController))
        {
            return;
        }

        shipController.ResetPosition();
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void AddScoreServerRpc() { }

    [ClientRpc]
    protected virtual void DoSomethingClientRpc() { }

    protected virtual void Initiate(NetworkUpdateStage updateStage = NetworkUpdateStage.Update)
    {
        this.RegisterNetworkUpdate(updateStage);

        switch (updateStage)
        {
            case NetworkUpdateStage.FixedUpdate:
                OnFixedUpdateAction += Movement;
                break;
            case NetworkUpdateStage.Update:
                OnUpdateAction += Movement;
                break;
            case NetworkUpdateStage.PreLateUpdate:
                OnLateUpdateAction += Movement;
                break;
        }
    }

    public void NetworkUpdate(NetworkUpdateStage updateStage)
    {
        switch (updateStage)
        {
            case NetworkUpdateStage.FixedUpdate:
                OnFixedUpdateAction?.Invoke();
                break;
            case NetworkUpdateStage.Update:
                OnUpdateAction?.Invoke();
                break;
            case NetworkUpdateStage.PreLateUpdate:
                OnLateUpdateAction?.Invoke();
                break;
        }
    }

    protected virtual void Movement()
    {
        if (IsOwner)
        {
            HasAuthorityMovement();
        }
        else
        {
            FromServerUpdate();
        }
    }

    protected abstract void HasAuthorityMovement();

    protected abstract void FromServerUpdate();

    protected abstract void SendToServer();
}