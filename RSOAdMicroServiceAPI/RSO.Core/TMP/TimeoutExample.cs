using System;
using System.Threading;
using System.Threading.Tasks;

public class TimeoutExample
{
    public async Task<string> PerformOperationAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var operationTask = PerformActualOperationAsync(cancellationTokenSource.Token);

        // Set a timeout for the operation
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

        var completedTask = await Task.WhenAny(operationTask, timeoutTask);

        if (completedTask == operationTask)
        {
            // The operation completed within the timeout
            return await operationTask;
        }
        else
        {
            // The operation timed out
            cancellationTokenSource.Cancel();
            throw new TimeoutException("Operation timed out");
        }
    }

    private async Task<string> PerformActualOperationAsync(CancellationToken cancellationToken)
    {
        // Your actual asynchronous operation goes here
        // e.g., calling an external service
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        return "Operation result";
    }
}
