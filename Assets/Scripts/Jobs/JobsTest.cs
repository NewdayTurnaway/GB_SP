using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class JobsTest : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private int _speed;

    private NativeArray<int> _angles;

    private void Start()
    {
        FirstJobTest();
        SecondJobTest();
        ThirdJobTestInit();
    }

    private void Update()
    {
        ThirdJobTestExecute();
    }

    private void OnDestroy()
    {
        _angles.Dispose();
    }

    private void FirstJobTest()
    {
        using var intArray = new NativeArray<int>(new int[] { 4, 20, 66, 1, 8, 32 }, Allocator.TempJob);

        var firstJob = new FirstJob
        {
            IntArray = intArray
        };

        var firstJobHandle = firstJob.Schedule();
        firstJobHandle.Complete();

        for (int i = 0; i < intArray.Length; i++)
        {
            Debug.Log($"{nameof(FirstJob)} : {intArray[i]}");
        }
    }

    private void SecondJobTest()
    {
        var firstArray = new Vector3[] { Vector3.one, Vector3.left, Vector3.down, Vector3.back };
        var secondArray = new Vector3[] { Vector3.zero, Vector3.down, Vector3.right };
        var minLength = Mathf.Min(firstArray.Length, secondArray.Length);

        using var positions = new NativeArray<Vector3>(firstArray, Allocator.Persistent);
        using var velocities = new NativeArray<Vector3>(secondArray, Allocator.Persistent);
        using var finalPositions = new NativeArray<Vector3>(new Vector3[minLength], Allocator.Persistent);

        var secondJob = new SecondJob
        {
            Positions = positions,
            Velocities = velocities,
            FinalPositions = finalPositions
        };

        var secondJobHandle = secondJob.Schedule(minLength, 0);
        secondJobHandle.Complete();

        for (int i = 0; i < secondJob.FinalPositions.Length; i++)
        {
            Debug.Log($"{nameof(SecondJob)} : {secondJob.FinalPositions[i]}");
        }
    }

    private void ThirdJobTestInit()
    {
        _angles = new NativeArray<int>(_transforms.Length, Allocator.Persistent);
    }

    private void ThirdJobTestExecute()
    {
        using var transformAccessArray = new TransformAccessArray(_transforms);

        var thirdJob = new ThirdJob()
        {
            Angles = _angles,
            Speed = _speed
        };

        var thirdJobHandle = thirdJob.Schedule(transformAccessArray);
        thirdJobHandle.Complete();
    }
}
