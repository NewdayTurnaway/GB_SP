using System;
using UnityEngine;

namespace SolarSystemGame
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Spaceship : MonoBehaviour
    {
        [SerializeField] private PlayerCamera _playerCamera;
        [SerializeField] private ShowObjectLabels _labels;

        private float _shipSpeed;
        private Rigidbody _rigidbody;
        private PlayerInitialSetup _initialSetup;

        public Action<Collision> CollisionEnter { get; set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _initialSetup = FindObjectOfType<PlayerInitialSetup>();
        }

        private void OnDestroy()
        {
            if (_playerCamera == null)
            {
                return;
            }

            Destroy(_playerCamera.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollisionEnter?.Invoke(collision);
        }

        public void ResetPosition()
        {
            gameObject.SetActive(false);
            transform.position = _initialSetup.GetRandomPosition();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            var spaceShipSettings = SettingsContainer.Instance != null ? SettingsContainer.Instance.SpaceShipSettings : null;

            if (spaceShipSettings == null)
            {
                return;
            }

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;
            var currentFov = isFaster ?
                SettingsContainer.Instance.SpaceShipSettings.FasterFov : SettingsContainer.Instance.SpaceShipSettings.NormalFov;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, SettingsContainer.Instance.SpaceShipSettings.Acceleration);
            _playerCamera.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);

            var velocity = _playerCamera.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * Time.deltaTime;

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(
                    Quaternion.AngleAxis(_playerCamera.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        private void OnGUI()
        {
            _labels.DrawLabel(_playerCamera.Camera);
        }
    }
}