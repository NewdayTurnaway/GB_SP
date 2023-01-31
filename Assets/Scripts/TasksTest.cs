using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TasksTest : MonoBehaviour
{
    private const int FRAMES = 60;
    private const int MILLISECONDS_DELAY = 1000;

    private CancellationTokenSource _cancellationTokenSource;

    private void Start()
    {
        TasksAsync();
    }

    private async void TasksAsync()
    {
        _cancellationTokenSource = new();
        var token = _cancellationTokenSource.Token;
        await Task.WhenAll(Task1Async(token), Task2Async(token));
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private async Task Task1Async(CancellationToken cancellationToken)
    {
        await Task.Delay(MILLISECONDS_DELAY, cancellationToken);
        Debug.Log("Task1 Complete");
    }

    private async Task Task2Async(CancellationToken cancellationToken)
    {
        for (int i = 0; i < FRAMES; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Task.Yield();
        }

        Debug.Log("Task2 Complete");
    }
}
