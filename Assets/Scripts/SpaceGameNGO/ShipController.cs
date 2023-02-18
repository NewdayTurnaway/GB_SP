using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : NetworkMovableObject
{
    [SerializeField] private Transform _cameraAttach;
    
    private NetworkVariable<FixedString128Bytes> _playerName = new("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private PlayerObjectInitialSetup _initialSetup;
    private CameraOrbit _cameraOrbit;
    private PlayerLabel _playerLabel;
    private float _shipSpeed;
    private Rigidbody _rigidbody;

    protected override float Speed => _shipSpeed;
    public string PlayerName { get => _playerName.Value.ToString(); set => _playerName.Value = value; }

    private void OnGUI()
    {
        if (_cameraOrbit == null)
        {
            return;
        }
        _cameraOrbit.ShowPlayerLabels(_playerLabel);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!TryGetComponent(out _rigidbody))
        {
            return;
        }

        _initialSetup = FindObjectOfType<PlayerObjectInitialSetup>();
        _cameraOrbit = FindObjectOfType<CameraOrbit>();
        _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
        _playerLabel = GetComponentInChildren<PlayerLabel>();
        base.OnNetworkSpawn();
    }

    public void ResetPosition()
    {
        if (!IsOwner)
        {
            return;
        }

        gameObject.SetActive(false);
        transform.position = _initialSetup.GetRandomPosition();
        gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void AddScoreServerRpc()
    {
        Score.Value++;
    }

    protected override void HasAuthorityMovement()
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
        _cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);
        
        var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
        _rigidbody.velocity = velocity * Time.deltaTime;

        if (!Input.GetKey(KeyCode.C))
        {
            var targetRotation = Quaternion.LookRotation(
                Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }

    protected override void FromServerUpdate() { }
    protected override void SendToServer() { }
    
    [ClientRpc]
    protected override void DoSomethingClientRpc()
    {
        _cameraOrbit?.CameraMovementClientRpc();
    }
}
