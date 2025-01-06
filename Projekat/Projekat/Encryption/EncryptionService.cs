using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Security.Cryptography.Xml;

namespace Projekat.Encryption
{
    public class EncryptionService : IEncryption
    {
        private string _rc4Key = "your-rc4-key"; // Replace with your RC4 key
        private byte[] _xteaKey = Encoding.UTF8.GetBytes("your-xtea-key"); // Replace with your XTEA key
        private byte[] _iv = new byte[8]; // Initialize the IV (IV should be 8 bytes for XTEA)

        private CancellationToken _token;
        private string _inputPath;
        private string _outputPath;
        private string _encryptionType; // "RC4" or "XTEA"

        public EncryptionService(string rc4Key = null, byte[] xteaKey = null, byte[] iv = null)
        {
            if (rc4Key != null)
            {
                _rc4Key = rc4Key;
            }
            if (xteaKey != null)
            {
                _xteaKey = xteaKey;
            }
            if (iv != null)
            {
                _iv = iv;
            }
        }
        public void Configure(string inputPath, string outputPath, string encryptionType, string rc4Key, byte[] xteaKey, byte[] iv, CancellationToken token)
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
            _encryptionType = encryptionType;
            _rc4Key = rc4Key ?? _rc4Key; // Default to the provided key if available
            _xteaKey = xteaKey ?? _xteaKey; // Default to the provided XTEA key if available
            _iv = iv ?? _iv; // Default to the provided IV if available
            _token = token;
        }
        // Start encryption process (will keep looping as long as cancellation is not requested)
        public void Start()
        {
            try
            {
                while (!_token.IsCancellationRequested)
                {
                    if (File.Exists(_inputPath))
                    {
                        if (_encryptionType == "RC4")
                        {
                            EncryptFileWithRC4(_inputPath, _outputPath);
                        }
                        else if (_encryptionType == "XTEA")
                        {
                            EncryptFileWithXTEA(_inputPath, _outputPath);
                        }

                        Console.WriteLine($"File encrypted successfully from {_inputPath} to {_outputPath}.");
                    }
                    else
                    {
                        Console.WriteLine("Input file not found. Waiting...");
                    }

                    Thread.Sleep(5000); // Wait for 5 seconds before rechecking
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EncryptionService encountered an error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("EncryptionService stopped.");
            }
        }
        // Start encryption process (can be run in the background thread)
        public async Task StartAsync()
        {
            try
            {
                if (File.Exists(_inputPath))
                {
                    if (_encryptionType == "RC4")
                    {
                        await Task.Run(() => EncryptFileWithRC4(_inputPath, _outputPath));
                    }
                    else if (_encryptionType == "XTEA")
                    {
                        await Task.Run(() => EncryptFileWithXTEA(_inputPath, _outputPath));
                    }
                    Console.WriteLine($"File encrypted successfully from {_inputPath} to {_outputPath}.");
                }
                else
                {
                    Console.WriteLine("Input file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EncryptionService encountered an error: {ex.Message}");
            }
        }

        public async Task StartDecodingAsync()
        {
            try
            {
                if (File.Exists(_inputPath))
                {
                    if (_encryptionType == "RC4")
                    {
                        await Task.Run(() => DecryptFileWithRC4(_inputPath, _outputPath));
                    }
                    else if (_encryptionType == "XTEA")
                    {
                        await Task.Run(() => DecryptFileWithXTEA(_inputPath, _outputPath));
                    }
                    Console.WriteLine($"File decoded successfully from {_inputPath} to {_outputPath}.");
                }
                else
                {
                    Console.WriteLine("Input file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EncryptionService encountered an error: {ex.Message}");
            }
        }

        internal void EncryptFileWithRC4(string inputPath, string outputPath)
        {
            using (FileStream inputFileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                RC4 rc4 = RC4.GetInstance();
                rc4.Key = _rc4Key; // Set RC4 key

                byte[] buffer = new byte[4096]; // Read in chunks of 4KB 45 137 52
                int bytesRead;

                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Encrypt the chunk
                    byte[] encryptedChunk = rc4.Run(buffer.Take(bytesRead).ToArray());
                    outputFileStream.Write(encryptedChunk, 0, encryptedChunk.Length);
                }
            }
        }

        internal void DecryptFileWithRC4(string inputPath, string outputPath)
        {
            using (FileStream inputFileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                RC4 rc4 = RC4.GetInstance();
                rc4.Key = _rc4Key; // Set RC4 key

                byte[] buffer = new byte[4096]; // Read in chunks of 4KB
                int bytesRead;

                while ((bytesRead = inputFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Decrypt the chunk
                    byte[] decryptedChunk = rc4.Run(buffer.Take(bytesRead).ToArray());
                    outputFileStream.Write(decryptedChunk, 0, decryptedChunk.Length);
                }
            }
        }

        internal void EncryptFileWithXTEA(string inputPath, string outputPath)  // so FSW can access it
        {
            // Create XTEA encryption object and perform encryption
            XTEA xtea = new XTEA(_xteaKey, _iv);
            xtea.EncryptFile(inputPath, outputPath); // This will handle the file encryption
        }

        internal void DecryptFileWithXTEA(string inputPath, string outputPath)
        {
            XTEA xtea = new XTEA(_xteaKey, _iv);
            xtea.DecryptFile(inputPath, outputPath); // This will handle the file decryption

        }

    }
}

