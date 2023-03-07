using System;
using Unity.Netcode;
using UnityEngine;

namespace SolarSystemGame
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class ObjectNetworkController<T> : NetworkBehaviour where T: MonoBehaviour
    {
        [field: SerializeField] public T MonoController { get; private set; }

        private event Action _onEnabled = () => { };

        private void Awake()
        {
            MonoController.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                Destroy(MonoController);
            }

            MonoController.enabled = true;

            _onEnabled += Initialization;
            _onEnabled.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            _onEnabled -= Initialization;
        }

        public override void OnDestroy()
        {
            _onEnabled -= Initialization;
        }

        public virtual void Initialization() { }
    }
}