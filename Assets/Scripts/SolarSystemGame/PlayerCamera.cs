using UnityEngine;

namespace SolarSystemGame
{
    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        private const string MOUSE_Y = "Mouse Y";
        private const string MOUSE_X = "Mouse X";
        private const float HALF_EXTENDS_CONST = 0.5f * Mathf.Deg2Rad;
        private const float FOR_DELTA_TIME_CONST = 20.0f;
        [field: SerializeField] public Camera Camera { get; private set; }

        [SerializeField] private Transform _focus;
        [SerializeField] private LayerMask _obstacleMask;

        [Header("Settings")]
        [SerializeField, Range(0.01f, 10)] private float _distance = 5;
        [SerializeField, Range(0, 90)] private int _lookAngle = 0;
        [SerializeField, Min(0)] private float _focusRadius = 1;
        [SerializeField, Range(0, 1)] private float _focusCentering = 0.5f;
        [SerializeField, Range(0.1f, 5)] private float _sensitive = 0.5f;
        [SerializeField, Range(1, 360)] private float _rotationSpeed = 90;
        [SerializeField, Range(-89, 89)] private float _minVerticalAngle = -30;
        [SerializeField, Range(-89, 89)] private float _maxVerticalAngle = 60;

        private Vector3 _focusPoint;
        private Vector2 _orbitAngles = new(45, 0);
        private float _currentDistance;
        private float _desiredDistance;

        public int LookAngle => _lookAngle;

        private void OnValidate()
        {
            UpdateMinMaxVerticalAngles();
        }

        private void Awake()
        {
            Camera ??= GetComponent<Camera>();
            transform.parent = null;
            _desiredDistance = _distance;
            _currentDistance = _distance;
            _focusPoint = _focus.position;
            transform.localRotation = ConstrainAngles(ref _orbitAngles);
        }

        private void FixedUpdate()
        {
            UpdateFocusPoint();

            var lookRotation = ManualRotation(ref _orbitAngles) ? ConstrainAngles(ref _orbitAngles) : transform.localRotation;
            var lookDirection = lookRotation * Vector3.forward;

            var checkDistance = Physics.BoxCast(_focusPoint, GetCameraHalfExtends(), -lookDirection, out RaycastHit hit,
                lookRotation, _distance - Camera.nearClipPlane, _obstacleMask);
            _desiredDistance = checkDistance ? hit.distance * Camera.nearClipPlane : _distance;

            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * FOR_DELTA_TIME_CONST);
            var lookPosition = _focusPoint - lookDirection * _currentDistance;
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        public void SetFov(float fov, float changeSpeed)
        {
            Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, fov, changeSpeed * Time.deltaTime);
        }

        private void UpdateMinMaxVerticalAngles()
        {
            if (_maxVerticalAngle < _minVerticalAngle)
            {
                _minVerticalAngle = _maxVerticalAngle;
            }
        }

        private void UpdateFocusPoint()
        {
            var targetPoint = _focus.position;

            if (_focusRadius > 0)
            {
                var distance = Vector3.Distance(targetPoint, _focusPoint);
                var t = 1f;

                if (distance > .01f && _focusCentering > 0)
                {
                    t = Mathf.Pow(1 - _focusCentering, Time.deltaTime);
                }

                if (distance > _focusRadius)
                {
                    t = Mathf.Min(t, _focusRadius / distance);
                }

                _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
            }
            else
            {
                _focusPoint = targetPoint;
            }
        }

        private Vector3 GetCameraHalfExtends()
        {
            var halfExtends = Vector2.zero;
            halfExtends.y = Camera.nearClipPlane * Mathf.Tan(HALF_EXTENDS_CONST * Camera.fieldOfView);
            halfExtends.x = halfExtends.y * Camera.aspect;
            return halfExtends;
        }


        private bool ManualRotation(ref Vector2 orbitAngles)
        {
            var input = new Vector2(-Input.GetAxis(MOUSE_Y), Input.GetAxis(MOUSE_X));
            var e = Mathf.Epsilon;

            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += _rotationSpeed * _sensitive * Time.unscaledDeltaTime * input;
                return true;
            }

            return false;
        }

        private Quaternion ConstrainAngles(ref Vector2 orbitAngles)
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, _minVerticalAngle, _maxVerticalAngle);

            if (orbitAngles.y < 0)
            {
                orbitAngles.y += 360.0f;
            }
            else if (orbitAngles.y >= 360.0f)
            {
                orbitAngles.y -= 360.0f;
            }

            return Quaternion.Euler(orbitAngles);
        }
    } 
}
