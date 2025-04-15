namespace FolderSync
{
    public class Logger
    {
        private readonly string _logPath;
        private readonly object _lockObj = new();

        public Logger(string path)
        {
            _logPath = path;

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
                Console.WriteLine($"Created logs directory: {_logPath}");
            }
        }

        public void Log(string message)
        {
            var timestamp = DateTime.Now;
            var date = timestamp.ToString("yyyy-MM-dd");

            var logFilePath = Path.Combine(_logPath, $"FileSync-{date}.log");
            var fullMessage = $"[{timestamp:yyyy-MM-dd HH:mm:ss}] {message}";

            lock (_lockObj)
            {
                Console.WriteLine(fullMessage);
                File.AppendAllText(logFilePath, fullMessage + Environment.NewLine);
            }
        }
    }
}