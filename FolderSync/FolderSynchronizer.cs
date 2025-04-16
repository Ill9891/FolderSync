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
            _logger = logger;
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

            var sourceDirs = Directory.GetDirectories(_sourcePath, "*", SearchOption.AllDirectories);
            var sourceFiles = Directory.GetFiles(_sourcePath, "*", SearchOption.AllDirectories);
            var replicaDirs = Directory.GetDirectories(_replicaPath, "*", SearchOption.AllDirectories);
            var replicaFiles = Directory.GetFiles(_replicaPath, "*", SearchOption.AllDirectories);

            var sourceFileSet = new HashSet<string>();
            var sourceDirSet = new HashSet<string>();

            // Ensure all directories exist in replica
            foreach (var dir in sourceDirs)
            {
                var relative = Path.GetRelativePath(_sourcePath, dir);
                sourceDirSet.Add(relative);

                var destDir = Path.Combine(_replicaPath, relative);

                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    _logger.Log($"Copied directory: {relative}");
                }
            }

            // Sync files
            foreach (var file in sourceFiles)
            {
                var relative = Path.GetRelativePath(_sourcePath, file);
                sourceFileSet.Add(relative);

                var destPath = Path.Combine(_replicaPath, relative);
                var copy = true;
                var action = File.Exists(destPath) ? "Edited" : "Copied";

                if (File.Exists(destPath))
                {
                    var sourceInfo = new FileInfo(file);
                    var destInfo = new FileInfo(destPath);

                    if (sourceInfo.Length == destInfo.Length)
                    {
                        var hash1 = _hasher.GetHash(file);
                        var hash2 = _hasher.GetHash(destPath);
                        copy = hash1 != hash2;
                    }
                }

                if (copy)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                    File.Copy(file, destPath, true);
                    _logger.Log($"{action}: {relative}");
                }
            }

            // Delete files not in source
            foreach (var file in replicaFiles)
            {
                var relative = Path.GetRelativePath(_replicaPath, file);

                if (!sourceFileSet.Contains(relative))
                {
                    File.Delete(file);
                    _logger.Log($"Deleted file: {relative}");
                }
            }

            // Delete directories not in source
            foreach (var dir in replicaDirs.OrderByDescending(d => d.Length))
            {
                var relative = Path.GetRelativePath(_replicaPath, dir);

                if (!sourceDirSet.Contains(relative))
                {
                    Directory.Delete(dir, true);
                    _logger.Log($"Deleted directory: {relative}");
                }
            }
        }
    }
}