using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public struct ThirdJob : IJobParallelForTransform
{
    public NativeArray<int> Angles;
    public int Speed;

    public void Execute(int index, TransformAccess transform)
    {
        transform.localRotation = Quaternion.AngleAxis(Angles[index], Vector3.forward);
        Angles[index] = Angles[index] == 180 ? 0 : Angles[index] + Speed;
    }
}
