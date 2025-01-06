using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;


namespace Projekat.Encryption
{
    internal class XTEA
    {
        private const int BlockSize = 8; // 64-bit blocks
        private const int KeySize = 16; // 128-bit key
        private const int NumRounds = 32;
        private const int BufferSize = 8192; // 8 KB buffer for large files

        private byte[] Key;
        private byte[] IV;

        public XTEA(byte[] key, byte[] iv)
        {
            if (key.Length != KeySize)
                throw new ArgumentException($"Key must be {KeySize} bytes.");

            if (iv.Length != BlockSize)
                throw new ArgumentException($"IV must be {BlockSize} bytes.");

            Key = key;
            IV = iv;
        }

        // Encryption and decryption methods remain unchanged
        private void EncryptBlock(byte[] block, int offset)
        {
            uint v0 = BitConverter.ToUInt32(block, offset);
            uint v1 = BitConverter.ToUInt32(block, offset + 4);
            uint[] key = new uint[4];
            for (int i = 0; i < 4; i++)
                key[i] = BitConverter.ToUInt32(Key, i * 4);

            uint sum = 0;
            const uint delta = 0x9E3779B9;

            for (int i = 0; i < NumRounds; i++)
            {
                v0 += ((v1 << 4 ^ v1 >> 5) + v1) ^ (sum + key[sum & 3]);
                sum += delta;
                v1 += ((v0 << 4 ^ v0 >> 5) + v0) ^ (sum + key[(sum >> 11) & 3]);
            }

            BitConverter.GetBytes(v0).CopyTo(block, offset);
            BitConverter.GetBytes(v1).CopyTo(block, offset + 4);
        }

        private void DecryptBlock(byte[] block, int offset)
        {
            uint v0 = BitConverter.ToUInt32(block, offset);
            uint v1 = BitConverter.ToUInt32(block, offset + 4);
            uint[] key = new uint[4];
            for (int i = 0; i < 4; i++)
                key[i] = BitConverter.ToUInt32(Key, i * 4);

            const uint delta = 0x9E3779B9;
            uint sum = unchecked(delta * (uint)NumRounds);

            for (int i = 0; i < NumRounds; i++)
            {
                v1 -= ((v0 << 4 ^ v0 >> 5) + v0) ^ (sum + key[(sum >> 11) & 3]);
                sum -= delta;
                v0 -= ((v1 << 4 ^ v1 >> 5) + v1) ^ (sum + key[sum & 3]);
            }

            BitConverter.GetBytes(v0).CopyTo(block, offset);
            BitConverter.GetBytes(v1).CopyTo(block, offset + 4);
        }
        public void EncryptFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[BufferSize];
                byte[] prevBlock = (byte[])IV.Clone();

                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, BufferSize)) > 0)
                {
                    int fullBlocks = bytesRead / BlockSize;
                    int remainingBytes = bytesRead % BlockSize;

                    // Process full blocks
                    for (int i = 0; i < fullBlocks; i++)
                    {
                        int offset = i * BlockSize;
                        for (int j = 0; j < BlockSize; j++)
                            buffer[offset + j] ^= prevBlock[j];

                        EncryptBlock(buffer, offset);
                        Array.Copy(buffer, offset, prevBlock, 0, BlockSize);
                    }

                    // Handle remaining bytes with padding
                    if (remainingBytes > 0)
                    {
                        byte[] paddedBlock = new byte[BlockSize];
                        Array.Copy(buffer, fullBlocks * BlockSize, paddedBlock, 0, remainingBytes);
                        for (int i = remainingBytes; i < BlockSize; i++)
                            paddedBlock[i] = (byte)(BlockSize - remainingBytes);

                        for (int i = 0; i < BlockSize; i++)
                            paddedBlock[i] ^= prevBlock[i];

                        EncryptBlock(paddedBlock, 0);
                        outputStream.Write(paddedBlock, 0, BlockSize);
                        Array.Copy(paddedBlock, prevBlock, BlockSize);
                    }

                    outputStream.Write(buffer, 0, fullBlocks * BlockSize);
                }
            }
        }

        public void DecryptFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[BufferSize];
                byte[] prevBlock = (byte[])IV.Clone();
                byte[] tempBlock = new byte[BlockSize];

                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, BufferSize)) > 0)
                {
                    int fullBlocks = bytesRead / BlockSize;

                    // Process full blocks
                    for (int i = 0; i < fullBlocks; i++)
                    {
                        int offset = i * BlockSize;
                        Array.Copy(buffer, offset, tempBlock, 0, BlockSize);

                        DecryptBlock(buffer, offset);

                        for (int j = 0; j < BlockSize; j++)
                            buffer[offset + j] ^= prevBlock[j];

                        outputStream.Write(buffer, offset, BlockSize);
                        Array.Copy(tempBlock, prevBlock, BlockSize);
                    }
                }

                // Remove padding if this is the last block
                int paddingLength = buffer[bytesRead - 1];
                if (paddingLength > 0 && paddingLength <= BlockSize)
                {
                    outputStream.SetLength(outputStream.Length - paddingLength);
                }
            }
        }
    }
}
