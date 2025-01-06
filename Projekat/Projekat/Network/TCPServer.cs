using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Projekat.Encryption;

namespace Projekat.Network
{
    internal class TCPServer
    {
        private readonly CancellationToken _cancellationToken;
        private readonly int _port;
        public byte[] IVKey { get; private set; }
        public string EncryptionKey { get; private set; }
        public bool UseXtea { get; private set; }
        public string FolderPathNetwork {  get; private set; }

        private TcpListener _tcpListener;
        private bool _isRunning;

        public TCPServer(CancellationToken cancellationToken, int port, byte[] ivKey, string encryptionKey, bool useXtea,string folderPathNetwork)
        {
            _cancellationToken = cancellationToken;
            _port = port;
            IVKey = ivKey;
            EncryptionKey = encryptionKey;
            UseXtea = useXtea;
            if(folderPathNetwork != null)
            {
                FolderPathNetwork = folderPathNetwork;
            }
            else
            {
                FolderPathNetwork = Properties.Settings.Default.NetworkFolderPath;
            }
           
        }

        public async Task StartAsync()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            _tcpListener.Start();
            _isRunning = true;

            Console.WriteLine($"Server started on port {_port}.");

            while (!_cancellationToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    Console.WriteLine("Waiting for client connection...");

                    var client = await _tcpListener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);

                    Console.WriteLine("Client connected.");
                }
                catch (Exception ex) when (_cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Server stopping due to cancellation...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }

            Console.WriteLine("Exiting server loop.");
            Stop();
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            Console.WriteLine("Client connected.");
            using (var networkStream = client.GetStream())
            {
                try
                {
                    // Use a BinaryReader for structured deserialization
                    using (var reader = new BinaryReader(networkStream, Encoding.UTF8, leaveOpen: true))
                    {
                        // Receive the filename
                        string fileName = reader.ReadString();

                        // Receive the file size (long)
                        long fileSize = reader.ReadInt64();

                        // Receive the hash length (int)
                        int hashLength = reader.ReadInt32();

                        // Receive the hash (byte[])
                        byte[] hashBuffer = reader.ReadBytes(hashLength);

                        // Get the folder path from settings
                        string folderPath = FolderPathNetwork;
                        if (string.IsNullOrEmpty(folderPath))
                        {
                            Console.WriteLine("Invalid folder path in settings.");
                            await networkStream.WriteAsync(Encoding.UTF8.GetBytes("Invalid server configuration."));
                            return;
                        }

                        // Ensure the folder exists
                        Directory.CreateDirectory(folderPath);

                        // Receive encrypted file in blocks
                        string encryptedFilePath = Path.Combine(folderPath, fileName);
                        using (var fileStream = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[4096];
                            long totalBytesReceived = 0;

                            while (totalBytesReceived < fileSize)
                            {
                                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead == 0) break; // End of stream

                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesReceived += bytesRead;
                            }
                        }

                        // Compute BLAKE3 hash of the received encrypted file
                        byte[] computedHash = BLAKE3.HashData(File.ReadAllBytes(encryptedFilePath));
                        if (!hashBuffer.SequenceEqual(computedHash))
                        {
                            Console.WriteLine("Hash validation failed. Deleting received files.");
                            File.Delete(encryptedFilePath);
                            await networkStream.WriteAsync(Encoding.UTF8.GetBytes("Hash mismatch. Resend file."));
                            return;
                        }

                        // Define the decrypted file path
                        string decryptedFilePath = Path.Combine(folderPath, "Decrypted", fileName);

                        // Ensure the "Decrypted" directory exists
                        Directory.CreateDirectory(Path.Combine(folderPath, "Decrypted"));

                        // Decrypt the file and get the decrypted content
                        bool decryptionSuccess = DecryptFile(encryptedFilePath, decryptedFilePath);
                        if (!decryptionSuccess)
                        {
                            Console.WriteLine("Failed to decrypt the file.");
                            await networkStream.WriteAsync(Encoding.UTF8.GetBytes("Decryption failed. Resend file."));
                            return;
                        }

                        Console.WriteLine("File received and validated successfully.");
                        await networkStream.WriteAsync(Encoding.UTF8.GetBytes("File received and saved successfully."));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
            }
            Console.WriteLine("Client disconnected.");
        }





        private bool DecryptFile(string encryptedFilePath, string decryptedFilePath)
        {
            try
            {
                // Ensure that the subfolder for decrypted files exists
                string decryptedFolderPath = Path.GetDirectoryName(decryptedFilePath);
                if (!Directory.Exists(decryptedFolderPath))
                {
                    Directory.CreateDirectory(decryptedFolderPath);
                }

                if (UseXtea)
                {
                    var decryptionService = new EncryptionService(xteaKey: Encoding.UTF8.GetBytes(EncryptionKey), rc4Key: EncryptionKey, iv: IVKey);

                    // Decrypt the file with XTEA
                    decryptionService.DecryptFileWithXTEA(encryptedFilePath, decryptedFilePath);
                }
                else
                {
                    var decryptionService = new EncryptionService(rc4Key: EncryptionKey);

                    // Decrypt the file with RC4
                    decryptionService.DecryptFileWithRC4(encryptedFilePath, decryptedFilePath);
                }

                return true; // Return true if decryption was successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                return false; // Return false if decryption fails
            }
        }



        public void Stop()
        {
            _isRunning = false;
            _tcpListener?.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}
