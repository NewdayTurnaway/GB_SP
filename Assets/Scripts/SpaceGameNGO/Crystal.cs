using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject), typeof(Collider))]
public class Crystal : NetworkBehaviour
{
    public void Disable()
    {
        DisableServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableServerRpc()
    {
        DisableClientRpc();
    }

    [ClientRpc]
    private void DisableClientRpc()
    {
        gameObject.SetActive(false);
    }
}
