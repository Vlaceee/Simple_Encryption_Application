using Projekat.Encryption;
using Projekat.SystemWatcher;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Projekat
{
    public partial class Form1 : Form
    {
        private readonly CancellationTokenSource _cts;

        private TargetWatcher _targetWatcher;
        public string InputFolderPath { get; private set; }
        public string EncryptedFolderPath { get; private set; }
        public string EncryptionKey { get; private set; }

        private bool ChangingForms = false;

        public byte[] IVKey { get; private set; }

        public string EncryptionMethod { get; private set; }
        public string InputFilePath { get; private set; }
        public string EncryptedFilePath { get; private set; }


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

        public Form1(CancellationTokenSource cts)
        {
            InputFolderPath = Properties.Settings.Default.XTargetPath;
            EncryptedFolderPath = Properties.Settings.Default.TargetPath;
            EncryptionKey = Properties.Settings.Default.DefaultEncryptionKey;
            _cts = cts;
            InitializeComponent();
        }

        public void UpdateUI(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    // Perform UI updates here
                    MessageBox.Show(message, "Thread Update");
                }));
            }
            else
            {
                // Perform UI updates directly if on the UI thread
                MessageBox.Show(message, "Thread Update");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            ComboBoxEncryption.SelectedIndex = 0;
            IVKey = System.Text.Encoding.UTF8.GetBytes(Properties.Settings.Default.DefaultIVKey);
            EncryptionKey = Properties.Settings.Default.DefaultEncryptionKey;
            InputFolderPath = Properties.Settings.Default.TargetPath;
            EncryptedFolderPath = Properties.Settings.Default.XTargetPath;
            AddToTarget.Enabled = false;
            DeleteFromX.Enabled = false;
            RemoveTargetButton.Enabled = false;

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


                BrowseButton.Focus();
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
        private void FileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Get the selected file's full path (using verbatim string literal)
            string filePath = (sender as OpenFileDialog).FileName;

            // Example of using the @ symbol for verbatim string literals (no need to escape backslashes)
            string fullPathWithAtSymbol = @$"{filePath}";

            // Display the path or use it wherever necessary
            MessageBox.Show($"Selected file path: {fullPathWithAtSymbol}");
        }

        private async void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Title = "Select a File to Encrypt" };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                InputFilePath = fileDialog.FileName;
                ShowThreadSafeMessageBox($"Selected file: {InputFilePath}", "File Selected");

                // Generate the output encrypted file path
                string encryptedFilePath = Path.Combine(EncryptedFolderPath, Path.GetFileName(InputFilePath));

                // Create a cancellation token source for managing cancellation requests
                var cts = new CancellationTokenSource();

                // Create an instance of EncryptionService and configure it
                var encryptionService = new EncryptionService(EncryptionKey, System.Text.Encoding.UTF8.GetBytes(EncryptionKey), IVKey);
                encryptionService.Configure(
                    inputPath: InputFilePath,
                    outputPath: encryptedFilePath,
                    encryptionType: EncryptionMethod,
                    rc4Key: EncryptionKey, // Use the RC4 key from the text box
                    xteaKey: System.Text.Encoding.UTF8.GetBytes(EncryptionKey), // Use the IVKey for XTEA encryption
                    iv: IVKey,
                    token: cts.Token // Pass the cancellation token
                );

                // Inform the user the encryption is starting

                //ShowThreadSafeMessageBox($"Starting encryption...", "Encryption In Progress");

                // Run the encryption process on a background thread
                await encryptionService.StartAsync();

                // Inform the user the file has been encrypted
                ShowThreadSafeMessageBox($"File encrypted successfully. Saved at: {encryptedFilePath}", "Encryption Complete");
            }
        }

        private async Task EncryptFileAsync(string inputFilePath, string outputFilePath)
        {
            // Read the file data
            byte[] fileData = await File.ReadAllBytesAsync(inputFilePath);

            // Compute the Blake3 hash for the file data
            byte[] encryptedData = await BLAKE3.HashDataAsync(fileData);

            // Write the encrypted data to the output file
            await File.WriteAllBytesAsync(outputFilePath, encryptedData);

            ShowThreadSafeMessageBox($"File encrypted successfully. Saved at: {outputFilePath}", "Encryption Complete");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel background threads when the form is closing
            _cts.Cancel(); //telling decoding button to stop once it is finished

            if(_targetWatcher!=null)
            {
                _targetWatcher.Stop();
            }
          
            if (ChangingForms==false)
            {
                Program.AppContext.EnsureExit();
            }
        }

        private void CheckBoxFSW_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the checkbox is checked
            if (checkBoxFSW.Checked)
            {
                BrowseButton.Enabled = false;
                XTEAIVTextBox.Enabled = false;
                RC4TextBox.Enabled = false;
                ComboBoxEncryption.Enabled = false;
                AddToTarget.Enabled = true;
                DeleteFromX.Enabled = true;
                RemoveTargetButton.Enabled = true;

                // Define the encryption configuration
                _targetWatcher = new TargetWatcher(_cts,InputFolderPath, EncryptedFolderPath, EncryptionMethod, EncryptionKey, System.Text.Encoding.UTF8.GetBytes(EncryptionKey), IVKey, this);
                // Run the Start method on a background thread
                Task.Run(() => _targetWatcher.Start(_cts.Token)); //was not cts
            }
            else
            {
                // Enable BrowseButton when the checkbox is unchecked
                RemoveTargetButton.Enabled = false;
                AddToTarget.Enabled = false;
                DeleteFromX.Enabled = false;
                BrowseButton.Enabled = true;
                ComboBoxEncryption.Enabled = true;
                RC4TextBox.Enabled = true;
                if (EncryptionMethod == "XTEA")
                {
                    XTEAIVTextBox.Enabled = true;
                }

                // Stop the FileSystemWatcher
                _targetWatcher?.Stop();
            }
        }

        private void XTEAIVTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BrowseButton.Focus();
            }

        }

        private async void XTEAIVTextBox_LeaveAsync(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                ValidateXTEAKeyLength();
            }, _cts.Token);
        }

        private async void DecodingButton_Click(object sender, EventArgs e)
        {
            DecodeButton.Enabled = false;
            DecodeButton.Text = "Decoding...";
            // Open a file dialog to select the file to decode
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a File to Decode"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected file path
                string inputFilePath = openFileDialog.FileName;
                ShowThreadSafeMessageBox($"Selected file to decode: {inputFilePath}", "File Selected");

                // Open a file dialog to select the destination for the decoded file
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Select Destination for Decoded File",
                    FileName = Path.GetFileNameWithoutExtension(inputFilePath) + "_decoded" + Path.GetExtension(inputFilePath),
                    DefaultExt = Path.GetExtension(inputFilePath),
                    Filter = "All Files|*.*"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the destination file path
                    string outputFilePath = saveFileDialog.FileName;
                    // ShowThreadSafeMessageBox($"Decoding to: {outputFilePath}", "Destination Selected");

                    // Create a cancellation token source for managing cancellation requests
                    var cts = new CancellationTokenSource();

                    // Create an instance of EncryptionService and configure it for decoding
                    var encryptionService = new EncryptionService(EncryptionKey, System.Text.Encoding.UTF8.GetBytes(EncryptionKey), IVKey);
                    encryptionService.Configure(
                        inputPath: inputFilePath,
                        outputPath: outputFilePath,
                        encryptionType: EncryptionMethod,
                        rc4Key: EncryptionKey, // Use the RC4 key from the text box
                        xteaKey: System.Text.Encoding.UTF8.GetBytes(EncryptionKey), // Use the IVKey for XTEA decryption
                        iv: IVKey,
                        token: cts.Token // Pass the cancellation token
                    );

                    try
                    {
                        // Inform the user the decoding is starting
                        //ShowThreadSafeMessageBox("Starting decoding...", "Decoding In Progress");
                        
                        // Run the decoding process asynchronously
                        await Task.Run(() => encryptionService.StartDecodingAsync());

                        // Inform the user the file has been decoded
                        ShowThreadSafeMessageBox($"File decoded successfully. Saved at: {outputFilePath}", "Decoding Complete");
                    }
                    catch (OperationCanceledException)
                    {
                        ShowThreadSafeMessageBox("Decoding was canceled.", "Canceled");
                    }
                    catch (Exception ex)
                    {
                        ShowThreadSafeMessageBox($"An error occurred during decoding: {ex.Message}", "Error");
                    }
                 
                }
            }

            DecodeButton.Enabled = true;
            DecodeButton.Text = "Decode Files";
        }


        private void XTEALabel_Click(object sender, EventArgs e)
        {

        }
        internal async Task UpdateTargetViewAsync(List<string> targetData)
        {
            await Task.Run(() =>
            {
                // Ensure UI updates are thread-safe
                UpdateUIThreadSafe(() =>
                {
                    // Assuming TargetView is a ListView (or similar control)
                    TargetView.Items.Clear();  // Clear the existing items

                    // Add the new data (assuming targetData is a List of strings)
                    foreach (var item in targetData)
                    {
                        TargetView.Items.Add(new ListViewItem(item));  // Add new item
                    }
                });
            });
        }

        internal async Task UpdateXViewAsync(List<string> xData)
        {
            await Task.Run(() =>
            {
                // Ensure UI updates are thread-safe
                UpdateUIThreadSafe(() =>
                {
                    // Assuming XView is a ListView (or similar control)
                    XView.Items.Clear();  // Clear the existing items

                    // Add the new data (assuming xData is a List of strings)
                    foreach (var item in xData)
                    {
                        XView.Items.Add(new ListViewItem(item));  // Add new item
                    }
                });
            });
        }

        private void AddToTarget_Click(object sender, EventArgs e)
        {
            // Open a file dialog to select a file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a File to Add to Target";
                openFileDialog.Filter = "All Files (*.*)|*.*";

                // Show the dialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Define the target folder and file path
                    string targetFolderPath = InputFolderPath; // Replace with your folder path
                    string targetFilePath = Path.Combine(targetFolderPath, Path.GetFileName(selectedFilePath));

                    try
                    {
                        // Ensure the target folder exists
                        Directory.CreateDirectory(targetFolderPath);

                        // Copy the file to the target folder, overwriting if it already exists
                        File.Copy(selectedFilePath, targetFilePath, true);

                        // Notify the user of success
                        MessageBox.Show($"File successfully added to target:\n{targetFilePath}",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that occur during the copy process
                        MessageBox.Show($"An error occurred while adding the file:\n{ex.Message}",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DeleteFromX_Click(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (XView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in XView.SelectedItems)
                {
                    // Remove the item from the ListView
                    XView.Items.Remove(item);

                    // Construct the file path based on EncryptedFolderPath and item text
                    string filePath = Path.Combine(EncryptedFolderPath, item.Text);

                    try
                    {
                        // Check if the file exists and delete it
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle errors (e.g., file in use, access denied)
                        MessageBox.Show($"Error deleting file '{filePath}': {ex.Message}",
                                        "Delete File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void RemoveTargetButton_Click(object sender, EventArgs e)
        {
            if (TargetView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in TargetView.SelectedItems)
                {
                    // Remove the item from the ListView
                    TargetView.Items.Remove(item);

                    // Construct the file path based on InputFolderPath and item text
                    string filePath = Path.Combine(InputFolderPath, item.Text);

                    try
                    {
                        // Check if the file exists and delete it
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle errors (e.g., file in use, access denied)
                        MessageBox.Show($"Error deleting file '{filePath}': {ex.Message}",
                                        "Delete File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

   

        private void networkWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.AppContext == null)
            {
                MessageBox.Show("Application context is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ChangingForms = true;
            Program.AppContext.SwitchToForm(typeof(NetworkTCP));
        }

   
    }
}
