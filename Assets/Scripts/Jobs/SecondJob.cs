using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SecondJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> Positions;
    [ReadOnly] public NativeArray<Vector3> Velocities;
    [WriteOnly] public NativeArray<Vector3> FinalPositions;

    public void Execute(int index)
    {
        FinalPositions[index] = Positions[index] + Velocities[index];
    }
}
