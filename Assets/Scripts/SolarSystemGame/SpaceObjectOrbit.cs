using UnityEngine;

namespace SolarSystemGame
{
    public class SpaceObjectOrbit : MonoBehaviour
    {
        private const float CIRCLE_RADIANS = Mathf.PI * 2;

        [SerializeField] private Transform _aroundPoint;
        [field: SerializeField] public SpaceObjectOrbitSettings Settings { get; set; }

        private float _distance;
        private float _currentAngle;
        private float _currentRotationAngle;

        private void Awake()
        {
            _distance = (transform.position - _aroundPoint.position).magnitude;
        }

        private void Update()
        {
            var position = _aroundPoint.position;
            position.x += Mathf.Sin(_currentAngle) * _distance * Settings.offsetSin;
            position.z += Mathf.Cos(_currentAngle) * _distance * Settings.offsetCos;
            transform.position = position;
            _currentRotationAngle += Time.deltaTime * Settings.rotationSpeed;
            _currentRotationAngle = Mathf.Clamp(_currentRotationAngle, 0, 361);

            if (_currentRotationAngle >= 360)
            {
                _currentRotationAngle = 0;

            }
            transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, transform.up);
            _currentAngle += CIRCLE_RADIANS * Settings.circleInSecond * Time.deltaTime;
        }
    } 
}