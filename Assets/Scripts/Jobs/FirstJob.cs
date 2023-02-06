using Unity.Collections;
using Unity.Jobs;

public struct FirstJob : IJob
{
    public NativeArray<int> IntArray;

    public void Execute()
    {
        for (int i = 0; i < IntArray.Length; i++)
        {
            if (IntArray[i] > 10)
            {
                IntArray[i] = 0;
            }
        }
    }
}
