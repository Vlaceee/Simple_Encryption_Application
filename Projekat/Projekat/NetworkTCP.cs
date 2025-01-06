using Projekat.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Projekat
{
    public partial class NetworkTCP : Form
    {
        private readonly CancellationTokenSource _cts;
        private TCPServer _tcpServer; // Server instance
        public string NetworkFolderPath { get; private set; }
        public string FileToSendPath { get; private set; }
        public string EncryptionKey { get; private set; }

        private bool ChangingForms = false;

        private bool ServerRunning {  get; set; }

        public string ClientIPadress { get; private set; } ///Where to send
        public int ServerPortInt { get; private set; } // To where the Client is sending

        public string ServerIPadress { get; private set; } //What to accept
        public int ConfiguredPort { get; private set; } //On which port server is listening

        public byte[] IVKey { get; private set; }

        public string EncryptionMethod { get; private set; }
        public string InputFilePath { get; private set; }
        public string EncryptedFilePath { get; private set; }
        public NetworkTCP(CancellationTokenSource cts)
        {
            NetworkFolderPath = Properties.Settings.Default.NetworkFolderPath;
            ServerRunning = false;
            EncryptionKey = Properties.Settings.Default.DefaultEncryptionKey;
            _cts = cts;

            InitializeComponent();
        }

        private void NetworkTCP_Load(object sender, EventArgs e)
        {
            ComboBoxEncryption.SelectedIndex = 0;
            IVKey = System.Text.Encoding.UTF8.GetBytes(Properties.Settings.Default.DefaultIVKey);
            EncryptionKey = Properties.Settings.Default.DefaultEncryptionKey;
            NetworkFolderPath = Properties.Settings.Default.NetworkFolderPath;

            FileToSendPath = null;
            ServerIPadress = "127.0.0.1";
            ServerPortInt = 8000;
            ClientIPadress = "127.0.0.1";
            ConfiguredPort = 8000;

        }
        private void UpdateUIThreadSafe(Action uiAction)
        {
            if (InvokeRequired)
                this.Invoke(uiAction);
            else
                uiAction();
        }
        private void ShowThreadSafeMessageBox(string message, string title = "Notification")
        {
            UpdateUIThreadSafe(() => MessageBox.Show(message, title));
        }

        private void ComboBoxEncryption_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUIThreadSafe(() =>
            {
                if (ComboBoxEncryption.SelectedItem.ToString() == "RC4")
                {

                    XTEAIVTextBox.Enabled = false;
                    EncryptionMethod = "RC4";
                }
                else
                {
                    EncryptionMethod = "XTEA";
                    XTEAIVTextBox.Enabled = true;
                    RC4TextBox.Text = Properties.Settings.Default.DefaultXTEAKey;
                    EncryptionKey = Properties.Settings.Default.DefaultXTEAKey;
                    IVKey = System.Text.Encoding.UTF8.GetBytes(Properties.Settings.Default.DefaultIVKey);
                    XTEAIVTextBox.Text = Properties.Settings.Default.DefaultIVKey;

                }
            });
        }
        private async void RC4TextBox_Leave(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                ValidateTextLength();
            }, _cts.Token);
        }
        private void RC4TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {


                //BrowseButton.Focus();
            }
        }

        private void ValidateTextLength()
        {
            string text = RC4TextBox.Text;

            // Convert text to byte array
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);

            if (EncryptionMethod == "RC4")
            {
                if (textBytes.Length < 5)
                {
                    MessageBox.Show("The key length is too short! Minimum advised length is 5 bytes. Please change the key.", "WARNING");
                }
                else if (textBytes.Length > 256)
                {
                    MessageBox.Show("The key length is too long! Maximum advised length is 256 bytes. Please shorten the key.", "WARNING");
                }
                else
                {
                    EncryptionKey = text; // Valid key length
                }
            }
            else if (EncryptionMethod == "XTEA")
            {
                if (textBytes.Length != 16)
                {
                    MessageBox.Show("The key length is invalid! It should be exactly 16 bytes (16 characters). Default value will be used.", "WARNING");
                    EncryptionKey = Properties.Settings.Default.DefaultXTEAKey;
                }
                else
                {
                    EncryptionKey = text; // Valid key length
                }
            }
        }

        private void ValidateXTEAKeyLength()
        {
            // Get the text from the IV TextBox (where the user enters the IV key)
            string ivText = XTEAIVTextBox.Text;

            // Convert the text to a byte array (UTF-8 encoding)
            byte[] ivBytes = System.Text.Encoding.UTF8.GetBytes(ivText);

            // Check if the byte length is exactly 8 bytes for XTEA IV key
            if (ivBytes.Length != 8)
            {
                // Inform the user if the IV key is not 8 bytes
                MessageBox.Show("The IV length must be exactly 8 bytes for XTEA encryption(8 characters). The default IV will be used.", "WARNING");
            }
            else
            {
                // If the length is correct, set the IV (this assumes you're storing it in a class-level variable)
                IVKey = ivBytes;

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel background threads when the form is closing
            
            if (ChangingForms == false)
            {
                _cts.Cancel();
                Program.AppContext.EnsureExit();
            }
        }

        private void XTEAIVTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //BrowseButton.Focus();
            }

        }

        private async void XTEAIVTextBox_LeaveAsync(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                ValidateXTEAKeyLength();
            }, _cts.Token);
        }

        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            // Create a new OpenFileDialog instance
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Configure OpenFileDialog settings
                openFileDialog.Title = "Select a File";
                openFileDialog.Filter = "All Files (*.*)|*.*"; // Adjust filter as needed
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Store the selected file's path in the FileToSendPath variable
                    FileToSendPath = openFileDialog.FileName;


                }
                else
                {

                }
            }
        }

        private void FolderChangeButton_Click(object sender, EventArgs e)
        {
            // Create a new FolderBrowserDialog instance
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Configure FolderBrowserDialog settings
                folderBrowserDialog.Description = "Select a folder to use as the network folder";
                folderBrowserDialog.ShowNewFolderButton = true; // Allow creating new folders
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                // Show the dialog and check if the user selected a folder
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Store the selected folder's path in the NetworkFolderPath variable
                    NetworkFolderPath = folderBrowserDialog.SelectedPath;

                    // Optionally display the selected path in a label or textbox
                    MessageBox.Show($"Folder selected: {NetworkFolderPath}", "Folder Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No folder selected.", "Operation Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private bool IsValidIPAddress(string ipAddress)
        {
            string[] segments = ipAddress.Split('.');
            if (segments.Length != 4) return false;

            foreach (string segment in segments)
            {
                if (!int.TryParse(segment, out int value) || value < 0 || value > 255)
                {
                    return false;
                }
            }
            return true;
        }
        private string ValidateAndFormatIPAddress(MaskedTextBox ipMaskBox, string ipAddress)
        {
            // Split the IP address into segments by the dot
            string[] segments = ipMaskBox.Text.Split('.');

            // Check if the IP address has exactly 4 segments
            if (segments.Length == 4)
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    // Count the number of spaces in the segment
                    int spaceCount = segments[i].Count(c => c == ' ');

                    // Remove spaces from the segment, keeping only the numbers
                    string segment = segments[i].Replace(" ", "");

                    // Pad the segment with leading zeros based on the number of spaces
                    segment = segment.PadLeft(segment.Length + spaceCount, '0');

                    // Ensure the segment is between 0 and 255, and pad to ensure 3 digits
                    int segmentValue;
                    if (int.TryParse(segment, out segmentValue))
                    {
                        if (segmentValue < 0 || segmentValue > 255)
                        {
                            segment = "255"; // Max valid value for an IP segment
                        }
                        else
                        {
                            segment = segmentValue.ToString("000"); // Pad to 3 digits
                        }
                    }
                    else
                    {
                        segment = "000"; // Invalid segment, set to 0
                    }

                    // Update the segment back into the array
                    segments[i] = segment;
                }

                // Reconstruct the formatted IP address with the padded segments
                string formattedIP = string.Join(".", segments);

                // Return formatted IP address
                return formattedIP;
            }
            else
            {
                // If the input doesn't have exactly 4 segments, revert to the last valid IP
                return ipAddress;
            }
        }
        private void ClientIP_Leave(object sender, EventArgs e)
        {
            string formattedIP = ValidateAndFormatIPAddress(ClientIP, ClientIPadress);

            // If the IP is valid, update the IP address
            if (IsValidIPAddress(formattedIP))
            {
                ClientIP.Text = formattedIP;
                ClientIPadress = string.Join(".", formattedIP.Split('.').Select(segment => int.Parse(segment).ToString()));
            }
            else
            {
                // If the IP is invalid, revert to the last valid IP address
                MessageBox.Show("Invalid IP address format. Defaulting to 127.0.0.1",
                                 "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClientIP.Text = "127.000.000.001";
                ClientIPadress = "127.0.0.1";
            }

            // Move focus to the next field (for example, PortServer)
            PortServer.Focus();
        }
        private void ServerIP_Leave(object sender, EventArgs e)
        {
            string formattedIP = ValidateAndFormatIPAddress(ServerIP, ServerIPadress);

            // If the IP is valid, update the IP address
            if (IsValidIPAddress(formattedIP))
            {
                ServerIP.Text = formattedIP;
                ServerIPadress = string.Join(".", formattedIP.Split('.').Select(segment => int.Parse(segment).ToString())); // Update the last valid IP address
            }
            else
            {
                // If the IP is invalid, revert to the last valid IP address
                MessageBox.Show("Invalid IP address format. Defaulting to 127.000.000.001",
                                 "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ServerIP.Text = "127.000.000.001";
                ServerIPadress = "127.0.0.1";
            }

            // Optionally, move focus to the next field
            PortServer.Focus();
        }

        private void ServerStartButton_Click(object sender, EventArgs e)
        {
            // Stop any previously running server
            ServerRunning = !ServerRunning;
         
            if(ServerRunning) 
            {
                ServerStartButton.Text = "Stop Server";
                try
                {
                    // Determine the encryption method (XTEA or RC4)
                    bool useXtea = EncryptionMethod.Equals("XTEA", StringComparison.OrdinalIgnoreCase);

                    // Initialize the server
                    _tcpServer = new TCPServer(
                        _cts.Token,
                        ConfiguredPort,
                        IVKey,
                        EncryptionKey,
                        useXtea,
                        NetworkFolderPath
                    );

                    // Run the server in a separate thread
                    Thread serverThread = new Thread(() =>
                    {
                        try
                        {
                            // Start the server asynchronously within the thread
                            _tcpServer.StartAsync().GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions in the background thread
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }
                    });

                    // Start the server thread
                    serverThread.IsBackground = true;
                    serverThread.Start();

                    // Inform the user that the server started successfully
                    MessageBox.Show("Server started successfully!", "Server Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Handle exceptions when initializing the server
                    MessageBox.Show($"Error starting server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                ServerStartButton.Text = "Start Server";
                _cts.Cancel(); // Signal cancellation
                _tcpServer?.Stop();  // is the thread stopping
                MessageBox.Show("Server stopped successfully!", "Server Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
           
        }


        private void ServerPort_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            if (textBox == null) return;

            // Remove any non-numeric characters
            textBox.Text = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Move the caret to the end of the text
            textBox.SelectionStart = textBox.Text.Length;

        }

        private void ServerPort_Leave(object sender, EventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            if (textBox == null) return;

            // Try to parse the text as an integer
            if (int.TryParse(textBox.Text, out int portNumber))
            {
                // Check if the port number is within the valid range
                if (portNumber < 0 || portNumber > 65535)
                {
                    // If not, reset to 8000 and show a message
                    textBox.Text = "8000";
                    ConfiguredPort = 8000; // Update ConfiguredPort to the default value
                    MessageBox.Show("Port value must be between 0 and 65535. Defaulting to 8000.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Update ConfiguredPort with the valid port number
                    ConfiguredPort = portNumber;
                }
            }
            else
            {
                // If parsing fails, reset to 8000 and show a message
                textBox.Text = "8000";
                ConfiguredPort = 8000; // Update ConfiguredPort to the default value
                MessageBox.Show("Port value must be a valid number. Defaulting to 8000.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PortServer_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            if (textBox == null) return;

            // Remove any non-numeric characters
            textBox.Text = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Move the caret to the end of the text
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void PortServer_Leave(object sender, EventArgs e)
        {
            var textBox = sender as System.Windows.Forms.TextBox;
            if (textBox == null) return;

            // Try to parse the text as an integer
            if (int.TryParse(textBox.Text, out int portNumber))
            {
                // Check if the port number is within the valid range
                if (portNumber < 0 || portNumber > 65535)
                {
                    // If not, reset to 8000 and show a message
                    textBox.Text = "8000";
                    ServerPortInt = 8000; // Update ConfiguredPort to the default value
                    MessageBox.Show("Port value must be between 0 and 65535. Defaulting to 8000.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Update ConfiguredPort with the valid port number
                    ConfiguredPort = portNumber;
                }
            }
            else
            {
                // If parsing fails, reset to 8000 and show a message
                textBox.Text = "8000";
                ServerPortInt = 8000; // Update ConfiguredPort to the default value
                MessageBox.Show("Port value must be a valid number. Defaulting to 8000.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private async void SendButton_Click(object sender, EventArgs e)
        {
            try
            {

                var tcpClient = new TCPClient(
                    fileToSendPath: FileToSendPath,
                    Encryptionkey: EncryptionKey,
                    clientIPadress: ClientIPadress,  // Server IP address (127.0.0.1 for local machine)
                    serverPortInt: ServerPortInt,  // Server port number
                    ivKey: IVKey,
                    encryptionMethod: EncryptionMethod
                );

                // Start the file sending process asynchronously
                await tcpClient.StartSendingAsync();

                // Optional: Show a message or update UI after sending the file
                MessageBox.Show("File sent successfully!");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the sending process
                MessageBox.Show($"Error sending file: {ex.Message}");
            }
        }

        private void fSWWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.AppContext == null)
            {
                MessageBox.Show("Application context is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ChangingForms = true;
            Program.AppContext.SwitchToForm(typeof(Form1));
        }
    }
}
