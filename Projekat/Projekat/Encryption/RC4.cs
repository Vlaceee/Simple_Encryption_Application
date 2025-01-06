using Projekat.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Basic Use
//RC4 rc4 = RC4.GetInstance();
//rc4.Key = "YourSecretKey"; // Set the key to user-defined input
//byte[] encrypted = rc4.EncodeStream("Hello, World!");
//string decrypted = rc4.DecodeStream(encrypted);
//Console.WriteLine($"Encrypted: {BitConverter.ToString(encrypted)}");
//Console.WriteLine($"Decrypted: {decrypted}");

namespace Projekat.Encryption
{
    internal class RC4
    {
        private static RC4 instance = null;
        private string key;
        private string latestData;
        private readonly byte[] s = new byte[256];
        private bool ctrModeActive;
        private readonly byte[] InitialCounter;

        #region Constructor
        private RC4()
        {
            key = null;
            latestData = null;
            ctrModeActive = false;
            InitialCounter = new byte[1];
            Random random = new();
            random.NextBytes(InitialCounter);

            for (int i = 0; i < 256; i++)
                s[i] = (byte)i;
        }
        #endregion

        #region Instance
        public static RC4 GetInstance()
        {
            if (instance != null)
                return instance;
            instance = new RC4();
            return instance;
        }
        #endregion

        #region Properties
        public string Key
        {
            get { return key; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Key cannot be null or empty.");

                // Convert the key to ASCII bytes
                byte[] asciiByteKey = Encoding.ASCII.GetBytes(value);
                key = value;

                // Initialize s-array
                for (int i = 0; i < 256; i++)
                    s[i] = (byte)i;

                // Shuffle with ASCII byte key
                Shuffle(asciiByteKey);
            }
        }
        private string EnsureTxtExtension(string filePath)
        {
            if (!Path.HasExtension(filePath) || Path.GetExtension(filePath) != ".txt")
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                return Path.Combine(directory, $"{fileNameWithoutExtension}.txt");
            }
            return filePath;
        }
        public void EncryptFile(string inputFilePath, string outputFilePath)
        {
            if (string.IsNullOrEmpty(Key))
                throw new InvalidOperationException("Key must be set before encryption.");

            // Ensure the output file has a .txt extension if it doesn't exist
            if (!File.Exists(outputFilePath))
            {
                outputFilePath = EnsureTxtExtension(outputFilePath);
            }

            // Read the input file, encrypt it, and write to the output file
            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] encryptedData = CtrModeActive ? EncryptWithCTRMode(fileData) : Run(fileData);

            File.WriteAllBytes(outputFilePath, encryptedData);
        }

        public void DecryptFile(string inputFilePath, string outputFilePath)
        {
            if (string.IsNullOrEmpty(Key))
                throw new InvalidOperationException("Key must be set before decryption.");

            // Ensure the output file has a .txt extension if it doesn't exist
            if (!File.Exists(outputFilePath))
            {
                outputFilePath = EnsureTxtExtension(outputFilePath);
            }

            // Read the encrypted file, decrypt it, and write to the output file
            byte[] encryptedData = File.ReadAllBytes(inputFilePath);
            byte[] decryptedData = CtrModeActive ? EncryptWithCTRMode(encryptedData) : Run(encryptedData);

            File.WriteAllBytes(outputFilePath, decryptedData);
        }

        public string LatestData
        {
            get { return latestData; }
        }

        public bool CtrModeActive
        {
            get { return ctrModeActive; }
            set { ctrModeActive = value; }
        }
        #endregion

        #region Methods
        private void Shuffle(byte[] key)
        {
            int j = 0;

            for (int i = 0; i < 256; i++)
            {
                j = (j + s[i] + key[i % key.Length]) % 256;

                byte temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }
        }

        public byte[] Run(byte[] data)
        {
            if (key == null)
                return null;

            int j = 0;
            byte[] results = new byte[data.Length];

            byte[] s = new byte[256];
            this.s.CopyTo(s, 0); // Copy original s array
            int i = 0;
            byte keyCurrent;
            byte temp;

            for (int iData = 0; iData < data.Length; iData++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;

                temp = s[i];
                s[i] = s[j];
                s[j] = temp;

                keyCurrent = s[(s[i] + s[j]) % 256];
                results[iData] = (byte)(data[iData] ^ keyCurrent);
            }

            char[] resultChars = new char[Encoding.ASCII.GetCharCount(results, 0, results.Length)];
            Encoding.ASCII.GetChars(results, 0, results.Length, resultChars, 0);
            this.latestData = new string(resultChars);
            return results;
        }

        private byte[] EncryptWithCTRMode(byte[] data)
        {
            byte[] result = new byte[data.Length];

            byte[] counter = new byte[1];
            counter[0] = InitialCounter[0];
            for (int k = 0; k < data.Length; k++)
            {
                byte[] counterOutput = Run(counter);
                result[k] = (byte)(data[k] ^ counterOutput[0]);
                counter[0] = (byte)((counter[0] + 1) % 256);
            }

            return result;
        }
        #endregion

        #region Encryption and Decryption
        public byte[] EncodeStream(string plainText)
        {
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);
            byte[] encrypted = CtrModeActive ? EncryptWithCTRMode(plainBytes) : Run(plainBytes);
            return encrypted;
        }

        public byte[] DecodeStream(byte[] cipherText)
        {
            byte[] decrypted = CtrModeActive ? EncryptWithCTRMode(cipherText) : Run(cipherText);
            string plainText = Encoding.ASCII.GetString(decrypted);
            return decrypted;
        }
        #endregion
    }


}