using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class AsteroidBelt : MonoBehaviour
{
    private const int ROTATION_SPEED_CORRECTION = 1000;
    private const int ORBIT_SPEED_CORRECTION = 10000;

    [Header("Spawner Settings")]
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField, Min(1)] private int _density = 50;
    [SerializeField] private int _seed;
    [SerializeField, Min(0)] private float _innerRadius = 25;
    [SerializeField, Min(0)] private float _outerRadius = 25;
    [SerializeField, Min(0)] private float _height = 5;
    [SerializeField] private bool _rotatingClockwise = true;

    [Header("Asteroid Settings")]
    [SerializeField, Min(0)] private float _minOrbitSpeed = 1;
    [SerializeField, Min(0)] private float _maxOrbitSpeed = 1.5f;
    [SerializeField, Min(0)] private float _minRotationSpeed = 1;
    [SerializeField, Min(0)] private float _maxRotationSpeed = 1;

    private Transform[] _transforms;
    private TransformAccessArray _transformAccessArray;

    [BurstCompile]
    private struct AsteroidRotateJob : IJobParallelForTransform
    {
        public float orbitSpeed;
        public Vector3 parentPosition;
        public Vector3 parentDirectionUp;
        public float rotationSpeed;
        public Vector3 rotationDirection;
        public bool rotationClockwise;
        public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            if (rotationClockwise)
            {
                transform.position = 
                    math.mul(
                            quaternion.AxisAngle(parentDirectionUp, rotationSpeed / ROTATION_SPEED_CORRECTION * deltaTime),
                            transform.position - parentPosition) + (float3)parentPosition;
            }
            else
            {
                transform.position =
                    math.mul(
                            quaternion.AxisAngle(-parentDirectionUp, rotationSpeed / ROTATION_SPEED_CORRECTION * deltaTime),
                            transform.position - parentPosition) + (float3)parentPosition;
            }

            transform.rotation = 
                math.mul(
                    transform.rotation, 
                    quaternion.AxisAngle(rotationDirection, orbitSpeed / ORBIT_SPEED_CORRECTION * deltaTime));
        }
    }


    private void Start()
    {
        _transforms = new Transform[_density];

        Random.InitState(_seed);

        for (int i = 0; i < _density; i++)
        {
            var randomRadius = Random.Range(_innerRadius, _outerRadius);
            var randomRadian = Random.Range(0, 2 * Mathf.PI);

            var y = Random.Range(-_height / 2, _height / 2);
            var x = randomRadius * Mathf.Cos(randomRadian);
            var z = randomRadius * Mathf.Sin(randomRadian);
            
            var localPosition = new Vector3(x, y, z);
            var worldOffset = transform.rotation * localPosition;
            var worldPosition = transform.position + worldOffset;

            var _asteroid = Instantiate(_asteroidPrefab, worldPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            _asteroid.transform.SetParent(transform);
            _transforms[i] = _asteroid.transform;
        }

        _transformAccessArray = new TransformAccessArray(_transforms);
    }

    private void Update()
    {
        var jobHandle = default(JobHandle);

        for (int i = 0; i < _density; i++)
        {
            var job = new AsteroidRotateJob()
            {
                parentPosition = transform.position,
                parentDirectionUp = transform.up,
                orbitSpeed = Random.Range(_minOrbitSpeed, _maxOrbitSpeed),
                rotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed),
                rotationDirection = new(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)),
                rotationClockwise = _rotatingClockwise,
                deltaTime = Time.deltaTime
            };

            jobHandle = job.Schedule(_transformAccessArray);
        }

        jobHandle.Complete();
    }

    private void OnDisable()
    {
        _transformAccessArray.Dispose();
    }
}