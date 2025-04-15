namespace FolderSync
{
    public class FolderSynchronizer
    {
        private readonly string _sourcePath;
        private readonly string _replicaPath;
        private readonly Logger _logger;
        private readonly FileHasher _hasher = new();

        public FolderSynchronizer(string source, string replica, Logger logger)
        {
            _sourcePath = source;
            _replicaPath = replica;
            this._logger = logger;
        }

        public void Synchronize()
        {
            if (!Directory.Exists(_sourcePath))
            {
                Directory.CreateDirectory(_sourcePath);
                _logger.Log($"Created source directory: {_sourcePath}");
            }

            if (!Directory.Exists(_replicaPath))
            {
                Directory.CreateDirectory(_replicaPath);
                _logger.Log($"Created replica directory: {_replicaPath}");
            }

            var sourceFiles = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);
            var replicaFiles = Directory.GetFiles(_replicaPath, "*", SearchOption.AllDirectories);

            var sourceRelative = new HashSet<string>();

            foreach (var file in sourceFiles)
            {
                var relative = Path.GetRelativePath(_sourcePath, file);
                sourceRelative.Add(relative);

                var destPath = Path.Combine(_replicaPath, relative);

                var action = File.Exists(destPath) ? "Edited" : "Copied";

                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

                var copy = true;

                if (File.Exists(destPath))
                {
                    var sourceInfo = new FileInfo(file);
                    var destInfo = new FileInfo(destPath);

                    if (sourceInfo.Length == destInfo.Length)
                    {
                        // Use hash if files are large or suspect differences
                        var hash1 = _hasher.GetHash(file);
                        var hash2 = _hasher.GetHash(destPath);
                        copy = hash1 != hash2;
                    }
                }

                if (copy)
                {
                    File.Copy(file, destPath, true);
                    _logger.Log($"{action}: {relative}");
                }
            }

            // Delete files not in source
            foreach (var file in replicaFiles)
            {
                var relative = Path.GetRelativePath(_replicaPath, file);

                if (!sourceRelative.Contains(relative))
                {
                    File.Delete(file);
                    _logger.Log($"Deleted: {relative}");
                }
            }
        }
    }
}