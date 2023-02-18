using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpaceShipSettings), menuName = "SpaceGameNGO/"+ nameof(SpaceShipSettings))]
public class SpaceShipSettings : ScriptableObject
{
    [field: SerializeField, Range(.01f, 0.1f)] public float Acceleration { get; private set; } = 0.0124f;
    [field: SerializeField, Range(1f, 2000f)] public float ShipSpeed { get; private set; } = 1193;
    [field: SerializeField, Range(1f, 5f)] public float Faster { get; private set; } = 5;
    [field: SerializeField, Range(.01f, 179)] public float NormalFov { get; private set; } = 60;
    [field: SerializeField, Range(.01f, 179)] public float FasterFov { get; private set; } = 30;
    [field: SerializeField, Range(.1f, 5f)]public float ChangeFovSpeed { get; private set; } = 0.5f;
}
