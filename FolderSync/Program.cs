using FolderSync;

if (args.Length != 4)
{
    Console.WriteLine("Usage: <sourcePath> <replicaPath> <intervalInSeconds> <logFilePath>");
    return;
}

var sourcePath = args[0];
var replicaPath = args[1];

if (!int.TryParse(args[2], out int intervalSeconds))
{
    Console.WriteLine("Invalid interval.");
    return;
}
var logFilePath = args[3];

var logger = new Logger(logFilePath);
var synchronizer = new FolderSynchronizer(sourcePath, replicaPath, logger);

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    logger.Log("Cancellation requested. Exiting the programm...");
    eventArgs.Cancel = true;
    cts.Cancel();
};

logger.Log("Starting folder synchronization.");
logger.Log($"Press Ctrl+C to exit.");

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        synchronizer.Synchronize();
        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cts.Token);
    }
}
catch (TaskCanceledException)
{
    //Expected if cancelled
}
catch (Exception ex)
{
    Console.WriteLine($"The program crashed due to: '{ex.Message}'");
}

logger.Log("Synchronization stopped.");