using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Blake3;
namespace Projekat.Encryption
{
    internal class BLAKE3
    {
        // Function to hash a byte array using BLAKE3
        public static byte[] HashData(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var blake3Stream = new Blake3Stream(ms))
            {
                // Compute the hash using the stream
                var hash = blake3Stream.ComputeHash();

                // Convert the hash to a byte array
                return hash.AsSpan().ToArray();
            }
        }

        // Function to hash a file using BLAKE3
        public static byte[] HashFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var blake3Stream = new Blake3Stream(fs))
            {
                // Compute the hash using the stream
                var hash = blake3Stream.ComputeHash();

                // Convert the hash to a byte array
                return hash.AsSpan().ToArray();
            }
        }

        // Async version to hash a byte array using BLAKE3
        public static async Task<byte[]> HashDataAsync(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var blake3Stream = new Blake3Stream(ms))
            {
                // Compute the hash using the stream (no need to wrap in Task.Run)
                var hash = await Task.FromResult(blake3Stream.ComputeHash());

                // Convert the hash to a byte array
                return hash.AsSpan().ToArray();
            }
        }

        // Async version to hash a file using BLAKE3
        public static async Task<byte[]> HashFileAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var blake3Stream = new Blake3Stream(fs))
            {
                // Compute the hash using the stream (no need to wrap in Task.Run)
                var hash = await Task.FromResult(blake3Stream.ComputeHash());

                // Convert the hash to a byte array
                return hash.AsSpan().ToArray();
            }
        }

        // Example for incremental update with Blake3.Hasher
        public static byte[] HashIncremental(byte[] data)
        {
            using var hasher = Blake3.Hasher.New();
            hasher.Update(data);
            return hasher.Finalize().AsSpan().ToArray();
        }
    }
}
