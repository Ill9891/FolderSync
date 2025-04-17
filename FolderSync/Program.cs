using FolderSync;

if (args.Length != 4)
{
    Console.WriteLine("Please enter a valid amount of arguments.\n" +
        "Usage: <sourcePath> <replicaPath> <intervalInSeconds> <logFilePath>");
    return;
}

var sourcePath = args[0];
var replicaPath = args[1];
var logFilePath = args[3];

if (!int.TryParse(args[2], out int intervalSeconds) || intervalSeconds <= 0)
{
    Console.WriteLine("Invalid interval. It must be a positive integer.");
    return;
}

bool IsValidPath(string path)
{
    if (string.IsNullOrWhiteSpace(path) || path.Length < 3)
        return false;

    var invalidChars = Path.GetInvalidPathChars();

    return path.All(x => !invalidChars.Contains(x));
}

// Validate all paths
if (!IsValidPath(sourcePath) || !IsValidPath(replicaPath) || !IsValidPath(logFilePath))
{
    Console.WriteLine("One or more paths are invalid.");
    return;
}

// Ensure source and replica are not the same
if (Path.GetFullPath(sourcePath).TrimEnd(Path.DirectorySeparatorChar).Equals(
    Path.GetFullPath(replicaPath).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Source and replica folders cannot be the same.");
    return;
}

var logger = new Logger(logFilePath);
var synchronizer = new FolderSynchronizer(sourcePath, replicaPath, logger);

using var сancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    logger.Log("Cancellation requested. Exiting the programm...");
    eventArgs.Cancel = true;
    сancellationTokenSource.Cancel();
};

logger.Log("Starting folder synchronization.");
logger.Log($"Press Ctrl+C to exit.");

try
{
    while (!сancellationTokenSource.Token.IsCancellationRequested)
    {
        synchronizer.Synchronize();
        await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), сancellationTokenSource.Token);
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