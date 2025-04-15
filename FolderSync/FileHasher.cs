using System.Security.Cryptography;

namespace FolderSync
{
    public class FileHasher
    {
        private readonly Dictionary<string, string> _hashCache = new();

        public string GetHash(string path)
        {
            if (_hashCache.TryGetValue(path, out var cached))
                return cached;

            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            var result = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            _hashCache[path] = result;
            return result;
        }
    }
}