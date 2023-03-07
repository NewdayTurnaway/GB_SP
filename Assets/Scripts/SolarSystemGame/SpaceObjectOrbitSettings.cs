using System;

namespace SolarSystemGame
{
    [Serializable]
    public class SpaceObjectOrbitSettings
    {
        public string name;
        public float xPosition;
        public float scale;
        public float circleInSecond;
        public float offsetSin;
        public float offsetCos;
        public float rotationSpeed;
    }
}