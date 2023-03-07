using UnityEngine;

namespace SolarSystemGame
{
    [RequireComponent(typeof(ClientNetworkTransform))]
    public class SpaceObjectOrbitNetworkController : ObjectNetworkController<SpaceObjectOrbit>
    {
    } 
}