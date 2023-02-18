using Unity.Netcode;
using UnityEngine;

public class PlanetOrbit : NetworkMovableObject
{
    private const float CIRCLE_RADIANS = Mathf.PI * 2;

    [SerializeField] private Transform _aroundPoint;
    [SerializeField] private float _smoothTime = .3f;
    [SerializeField] private float _circleInSecond = 1f;
    [SerializeField] private float _offsetSin = 1;
    [SerializeField] private float _offsetCos = 1;
    [SerializeField] private float _rotationSpeed;

    private float _distance;
    private float _currentAngle;
    private Vector3 _currentPositionSmoothVelocity;
    private float _currentRotationAngle;

    protected override float Speed => _smoothTime;

    private void Start()
    {
        _distance = (transform.position - _aroundPoint.position).magnitude;
        Initiate(NetworkUpdateStage.FixedUpdate);
    }

    protected override void HasAuthorityMovement()
    {
        if (!IsServer)
        {
            return;
        }

        var position = _aroundPoint.position;
        position.x += Mathf.Sin(_currentAngle) * _distance * _offsetSin;
        position.z += Mathf.Cos(_currentAngle) * _distance * _offsetCos;
        transform.position = position;
        _currentRotationAngle += Time.deltaTime * _rotationSpeed;
        _currentRotationAngle = Mathf.Clamp(_currentRotationAngle, 0, 361);
        
        if (_currentRotationAngle >= 360)
        {
            _currentRotationAngle = 0;
        
        }
        transform.rotation = Quaternion.AngleAxis(_currentRotationAngle, transform.up);
        _currentAngle += CIRCLE_RADIANS * _circleInSecond * Time.deltaTime;
        SendToServer();
    }

    protected override void SendToServer()
    {
        _serverPosition.Value = transform.position;
        _serverEuler.Value = transform.eulerAngles;
    }

    protected override void FromServerUpdate()
    {
        if (!IsClient)
        {
            return;
        }

        transform.SetPositionAndRotation(
            Vector3.SmoothDamp(transform.position, _serverPosition.Value, ref _currentPositionSmoothVelocity, Speed),
            Quaternion.Euler(_serverEuler.Value));
    }
}