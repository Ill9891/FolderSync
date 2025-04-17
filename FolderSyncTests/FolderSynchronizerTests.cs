using FolderSync;
using NUnit.Framework;

namespace FolderSyncTests
{
    [TestFixture]
    public class FolderSynchronizerTests : Hooks
    {
        [Test]
        public void Synchronize_CopiesFilesFromSourceToReplica()
        {
            // Arrange
            var fileName = "test.txt";
            var filePath = Path.Combine(_sourceDir, fileName);
            var fileText = "Hello, world!";
            File.WriteAllText(filePath, fileText);

            var synchronizer = new FolderSynchronizer(_sourceDir, _replicaDir, _logger);

            // Act
            synchronizer.Synchronize();

            var replicaFilePath = Path.Combine(_replicaDir, fileName);

            // Assert
            Assert.That(File.Exists(replicaFilePath), Is.True, $"File '{replicaFilePath}' doesn't exist.");
            var replicaContent = File.ReadAllText(replicaFilePath);
            Assert.That(replicaContent, Is.EqualTo(fileText), $"Incorrect content of the file '{replicaFilePath}'.");
        }
    }
}