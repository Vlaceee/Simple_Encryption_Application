using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Projekat.Encryption;

namespace Projekat.Network
{
    internal class TCPClient
    {
        public string FileToSendPath { get; private set; }
        public string ClientIPadress { get; private set; }
        public string EncryptionKey { get; private set; }
        public int ServerPortInt { get; private set; }
        public byte[] IVKey { get; private set; }
        public string EncryptionMethod { get; private set; }

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        public TCPClient(string fileToSendPath, string Encryptionkey, string clientIPadress, int serverPortInt, byte[] ivKey, string encryptionMethod)
        {
            EncryptionKey = Encryptionkey;
            FileToSendPath = fileToSendPath;
            ClientIPadress = clientIPadress;
            ServerPortInt = serverPortInt;
            IVKey = ivKey;
            EncryptionMethod = encryptionMethod;
        }

        public async Task StartSendingAsync()
        {
            try
            {
                _tcpClient = new TcpClient(ClientIPadress, ServerPortInt);
                _networkStream = _tcpClient.GetStream();

                // Get the filename
                string fileName = Path.GetFileName(FileToSendPath);

                // Encrypt the file and get the temporary file path
                string encryptedFilePath = Path.Combine(Properties.Settings.Default.TemporaryFiles, fileName);
                bool encryptionSuccess = await EncryptFileAsync(FileToSendPath, encryptedFilePath);

                if (!encryptionSuccess)
                {
                    Console.WriteLine("Encryption failed.");
                    return;
                }

                // Get the file size of the encrypted file
                long fileSize = new FileInfo(encryptedFilePath).Length;

                // Generate the BLAKE3 hash of the encrypted file
                byte[] fileHash = GetFileHash(encryptedFilePath);
                int hashLength = fileHash.Length;

                // Use a BinaryWriter for better serialization
                using (var writer = new BinaryWriter(_networkStream, Encoding.UTF8, leaveOpen: true))
                {
                    // Send the file name (string)
                    writer.Write(fileName);

                    // Send the file size (long)
                    writer.Write(fileSize);

                    // Send the hash length (int)
                    writer.Write(hashLength);

                    // Send the hash (byte[])
                    writer.Write(fileHash);
                }

                // Send the encrypted file in blocks
                await SendFileAsync(encryptedFilePath);

                // Clean up the temporary encrypted file
                File.Delete(encryptedFilePath);
                Console.WriteLine("File sent and temporary file deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending file: {ex.Message}");
            }
            finally
            {
                _networkStream?.Close();
                _tcpClient?.Close();
            }
        }


        private async Task<bool> EncryptFileAsync(string inputFilePath, string outputFilePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (EncryptionMethod == "XTEA")
                    {
                        var encryptionService = new EncryptionService(xteaKey: Encoding.UTF8.GetBytes(EncryptionKey), rc4Key: EncryptionKey, iv: IVKey);
                        encryptionService.EncryptFileWithXTEA(inputFilePath, outputFilePath);
                    }
                    else if (EncryptionMethod == "RC4")
                    {
                        var encryptionService = new EncryptionService(rc4Key: EncryptionKey);
                        encryptionService.EncryptFileWithRC4(inputFilePath, outputFilePath);
                    }
                    else
                    {
                        Console.WriteLine("Invalid encryption method.");
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Encryption failed: {ex.Message}");
                    return false;
                }
            });
        }


        private async Task SendFileAsync(string filePath)
        {
            byte[] buffer = new byte[4096];
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await _networkStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
        }

        private byte[] GetFileHash(string encryptedFilePath)
        {
            // Use BLAKE3 hashing to compute the hash of the encrypted file
            return BLAKE3.HashData(File.ReadAllBytes(encryptedFilePath));
        }
    }
}
