using UnityEngine;

public class SettingsContainer : Singleton<SettingsContainer>
{
    [field: SerializeField] public SpaceShipSettings SpaceShipSettings { get; private set; }
}