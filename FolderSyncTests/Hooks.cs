using FolderSync;
using NUnit.Framework;
using System;
namespace FolderSyncTests
{
    public class Hooks
    {
        protected string? _sourceDir;
        protected string? _replicaDir;
        protected string? _logDir;
        protected Logger? _logger;

        [SetUp]
        public void Setup()
        {
            _sourceDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _replicaDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _logDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(_sourceDir);
            Directory.CreateDirectory(_replicaDir);
            Directory.CreateDirectory(_logDir);

            _logger = new Logger(Path.Combine(_logDir, "TestLogs"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_sourceDir)) Directory.Delete(_sourceDir, true);
            if (Directory.Exists(_replicaDir)) Directory.Delete(_replicaDir, true);
            if (Directory.Exists(_logDir)) Directory.Delete(_logDir, true);
        }
    }
}