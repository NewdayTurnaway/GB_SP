using Unity.Collections;
using Unity.Netcode;

namespace SolarSystemGame
{
    public struct PlayerNetworkData : INetworkSerializable
    {
        public string Name
        {
            get => _name.Value.ToString();
            set => _name = new FixedString32Bytes(value);
        }

        public int Score;

        private ForceNetworkSerializeByMemcpy<FixedString32Bytes> _name;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _name);
            serializer.SerializeValue(ref Score);
        }
    }
}
