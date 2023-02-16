using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class PlayerNetwork : NetworkBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string MOUSE_Y = "Mouse Y";
    private const string MOUSE_X = "Mouse X";
    private const float POSITION_RANGE = 20f;

    [Header("UnityComponents")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private FireAction _fireAction;

    [Header("PlayerSettings")]
    [SerializeField, Range(0, 100)] private int _health = 100;
    [SerializeField, Range(0, 10)] private float _speed = 5;
    [SerializeField, Range(0, 10)] private float _acceleration = 2;
    [SerializeField] private float _gravity = -9.8f;

    [SerializeField, Range(0.1f, 10.0f)] private float _sensitivity = 2.0f;
    [SerializeField, Range(-90.0f, .0f)] private float _minVert = -45.0f;
    [SerializeField, Range(0.0f, 90.0f)] private float _maxVert = 45.0f;
    
    private float _rotationX = .0f;
    private float _rotationY = .0f;

    private void OnValidate()
    {
        _characterController ??= GetComponent<CharacterController>();
        _playerCamera ??= GetComponentInChildren<Camera>();
        _fireAction ??= GetComponent<RayShooter>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _playerCamera.enabled = true;
            _playerCamera.TryGetComponent(out AudioListener audioListener);
            audioListener.enabled = true;
        }

        _fireAction.Reloading();
        _characterController.enabled = false;
        transform.position = new(Random.Range(-POSITION_RANGE, POSITION_RANGE),
                                 transform.position.y,
                                 Random.Range(-POSITION_RANGE, POSITION_RANGE));
        _characterController.enabled = true;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Movement();
        Rotation();
    }

    private void OnGUI()
    {
        if (!IsOwner || Camera.main == null)
        {
            return;
        }

        var info = $"Health: {_health}\nClip: {_fireAction.BulletCount}";
        var size = 24;
        var bulletCountSize = size * 3;
        var posX = Camera.main.pixelWidth / 2 - size / 4;
        var posY = Camera.main.pixelHeight / 2 - size / 2;
        var posXBul = Camera.main.pixelWidth - bulletCountSize * 2;
        var posYBul = Camera.main.pixelHeight - bulletCountSize;
        var style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.fontSize = size;
        GUI.Label(new Rect(posX, posY, size, size), "+", style);
        style.normal.textColor = Color.black;
        GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2, bulletCountSize * 2), info, style);
    }

    [ClientRpc]
    public void TakeDamageClientRpc(int value)
    {
        _health -= Math.Abs(value);

        if (_health <= 0)
        {
            NetworkManager.Singleton.DisconnectClient(OwnerClientId);
        }
    }

    private void Movement()
    {
        var moveX = Input.GetAxis(HORIZONTAL) * _speed;
        var moveZ = Input.GetAxis(VERTICAL) * _speed;
        var movement = new Vector3(moveX, 0, moveZ);
        movement = Vector3.ClampMagnitude(movement, _speed);
        movement *= Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= _acceleration;
        }

        movement.y = _gravity;
        movement = transform.TransformDirection(movement);
        _characterController.Move(movement);
    }

    private void Rotation()
    {
        _rotationX -= Input.GetAxis(MOUSE_Y) * _sensitivity;
        _rotationY += Input.GetAxis(MOUSE_X) * _sensitivity;
        _rotationX = Mathf.Clamp(_rotationX, _minVert, _maxVert);
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
    }
}
