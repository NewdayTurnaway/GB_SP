using Unity.Netcode;
using UnityEngine;

namespace SolarSystemGame
{
    [RequireComponent(typeof(ClientNetworkTransform), typeof(Collider))]
    public class SpaceshipNetworkController : ObjectNetworkController<Spaceship>
    {
        public NetworkVariable<PlayerNetworkData> PlayerData = new(writePerm: NetworkVariableWritePermission.Owner);

        public override void OnNetworkDespawn()
        {
            if(MonoController != null)
            {
                MonoController.CollisionEnter -= CollisionEnter;
            }
            
            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            if (MonoController != null)
            {
                MonoController.CollisionEnter -= CollisionEnter;
            }
                                                                         
            base.OnDestroy();
        }

        public void SetPlayerData(PlayerNetworkData playerNetworkData)
        {
            PlayerData.Value = playerNetworkData;
            name = PlayerData.Value.Name;
            Debug.Log($"{PlayerData.Value.Name} Score: {PlayerData.Value.Score}");
        }

        public override void Initialization()
        {
            MonoController.CollisionEnter += CollisionEnter;
        }

        private void CollisionEnter(Collision collision)
        {
            if(collision.gameObject.TryGetComponent(out Crystal crystal))
            {
                crystal.Disable();
                var data = PlayerData.Value;
                data.Score++;
                PlayerData.Value = data;
                Debug.Log($"{PlayerData.Value.Name} Score: {PlayerData.Value.Score}");
                return;
            }

            MonoController.ResetPosition();
        }
    } 
}