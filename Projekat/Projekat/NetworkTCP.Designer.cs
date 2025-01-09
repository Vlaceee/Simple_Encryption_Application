namespace Projekat
{
    partial class NetworkTCP
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LabelClient = new Label();
            ServerLabel = new Label();
            ClientIP = new MaskedTextBox();
            ServerIP = new MaskedTextBox();
            PortServer = new TextBox();
            ServerPort = new TextBox();
            ServerStartButton = new Button();
            SendButton = new Button();
            BrowseFileButton = new Button();
            EncryptionAlgorithmLabel = new Label();
            ComboBoxEncryption = new ComboBox();
            RC4KeyLabel = new Label();
            RC4TextBox = new TextBox();
            XTEALabel = new Label();
            XTEAIVTextBox = new TextBox();
            FolderChangeButton = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            menuStrip1 = new MenuStrip();
            changeWindowToolStripMenuItem = new ToolStripMenuItem();
            networkWindowToolStripMenuItem = new ToolStripMenuItem();
            fSWWindowToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // LabelClient
            // 
            LabelClient.AutoSize = true;
            LabelClient.Font = new Font("Segoe UI", 16F);
            LabelClient.Location = new Point(65, 53);
            LabelClient.Name = "LabelClient";
            LabelClient.Size = new Size(161, 37);
            LabelClient.TabIndex = 0;
            LabelClient.Text = "Client Setup";
            // 
            // ServerLabel
            // 
            ServerLabel.AutoSize = true;
            ServerLabel.Font = new Font("Segoe UI", 16F);
            ServerLabel.Location = new Point(583, 53);
            ServerLabel.Name = "ServerLabel";
            ServerLabel.Size = new Size(165, 37);
            ServerLabel.TabIndex = 1;
            ServerLabel.Text = "Server Setup";
            // 
            // ClientIP
            // 
            ClientIP.Font = new Font("Segoe UI", 12F);
            ClientIP.Location = new Point(65, 132);
            ClientIP.Mask = "000\\.000\\.000\\.000";
            ClientIP.Name = "ClientIP";
            ClientIP.Size = new Size(155, 34);
            ClientIP.TabIndex = 2;
            ClientIP.Leave += ClientIP_Leave;
            // 
            // ServerIP
            // 
            ServerIP.Font = new Font("Segoe UI", 12F);
            ServerIP.Location = new Point(583, 131);
            ServerIP.Mask = "000\\.000\\.000\\.000";
            ServerIP.Name = "ServerIP";
            ServerIP.Size = new Size(158, 34);
            ServerIP.TabIndex = 3;
            ServerIP.Text = "127000000001";
            ServerIP.Leave += ServerIP_Leave;
            // 
            // PortServer
            // 
            PortServer.Location = new Point(76, 214);
            PortServer.Name = "PortServer";
            PortServer.Size = new Size(125, 27);
            PortServer.TabIndex = 4;
            PortServer.Text = "8000";
            PortServer.TextChanged += PortServer_TextChanged;
            PortServer.Leave += PortServer_Leave;
            // 
            // ServerPort
            // 
            ServerPort.Location = new Point(598, 214);
            ServerPort.Name = "ServerPort";
            ServerPort.Size = new Size(125, 27);
            ServerPort.TabIndex = 5;
            ServerPort.Text = "8000";
            ServerPort.TextChanged += ServerPort_TextChanged;
            ServerPort.Leave += ServerPort_Leave;
            // 
            // ServerStartButton
            // 
            ServerStartButton.Location = new Point(676, 299);
            ServerStartButton.Name = "ServerStartButton";
            ServerStartButton.Size = new Size(112, 29);
            ServerStartButton.TabIndex = 6;
            ServerStartButton.Text = "Start Server";
            ServerStartButton.UseVisualStyleBackColor = true;
            ServerStartButton.Click += ServerStartButton_Click;
            // 
            // SendButton
            // 
            SendButton.Location = new Point(12, 299);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(124, 29);
            SendButton.TabIndex = 7;
            SendButton.Text = "Send To Server";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // BrowseFileButton
            // 
            BrowseFileButton.Location = new Point(156, 299);
            BrowseFileButton.Name = "BrowseFileButton";
            BrowseFileButton.RightToLeft = RightToLeft.No;
            BrowseFileButton.Size = new Size(118, 29);
            BrowseFileButton.TabIndex = 8;
            BrowseFileButton.Text = "Browse File";
            BrowseFileButton.UseVisualStyleBackColor = true;
            BrowseFileButton.Click += BrowseFileButton_Click;
            // 
            // EncryptionAlgorithmLabel
            // 
            EncryptionAlgorithmLabel.AutoSize = true;
            EncryptionAlgorithmLabel.Location = new Point(293, 44);
            EncryptionAlgorithmLabel.Name = "EncryptionAlgorithmLabel";
            EncryptionAlgorithmLabel.Size = new Size(230, 20);
            EncryptionAlgorithmLabel.TabIndex = 9;
            EncryptionAlgorithmLabel.Text = "Set Desired Encryption Algorithm";
            // 
            // ComboBoxEncryption
            // 
            ComboBoxEncryption.AutoCompleteCustomSource.AddRange(new string[] { "RC4", "XTEA" });
            ComboBoxEncryption.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxEncryption.FormattingEnabled = true;
            ComboBoxEncryption.Items.AddRange(new object[] { "RC4", "XTEA" });
            ComboBoxEncryption.Location = new Point(328, 83);
            ComboBoxEncryption.Name = "ComboBoxEncryption";
            ComboBoxEncryption.Size = new Size(151, 28);
            ComboBoxEncryption.TabIndex = 10;
            ComboBoxEncryption.SelectedIndexChanged += ComboBoxEncryption_SelectedIndexChanged;
            // 
            // RC4KeyLabel
            // 
            RC4KeyLabel.AutoSize = true;
            RC4KeyLabel.Location = new Point(366, 132);
            RC4KeyLabel.Name = "RC4KeyLabel";
            RC4KeyLabel.Size = new Size(61, 20);
            RC4KeyLabel.TabIndex = 11;
            RC4KeyLabel.Tag = "RC4";
            RC4KeyLabel.Text = "The Key";
            RC4KeyLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RC4TextBox
            // 
            RC4TextBox.BackColor = SystemColors.InactiveBorder;
            RC4TextBox.Location = new Point(293, 167);
            RC4TextBox.Name = "RC4TextBox";
            RC4TextBox.PlaceholderText = "Enter the encryption key here...";
            RC4TextBox.Size = new Size(216, 27);
            RC4TextBox.TabIndex = 12;
            RC4TextBox.KeyDown += RC4TextBox_KeyDown;
            RC4TextBox.Leave += RC4TextBox_Leave;
            // 
            // XTEALabel
            // 
            XTEALabel.AutoSize = true;
            XTEALabel.Location = new Point(370, 214);
            XTEALabel.Name = "XTEALabel";
            XTEALabel.Size = new Size(50, 20);
            XTEALabel.TabIndex = 13;
            XTEALabel.Tag = "XTEA";
            XTEALabel.Text = "IV Key";
            XTEALabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // XTEAIVTextBox
            // 
            XTEAIVTextBox.BackColor = SystemColors.InactiveBorder;
            XTEAIVTextBox.Location = new Point(293, 257);
            XTEAIVTextBox.Name = "XTEAIVTextBox";
            XTEAIVTextBox.PlaceholderText = "Enter the encryption key here...";
            XTEAIVTextBox.Size = new Size(216, 27);
            XTEAIVTextBox.TabIndex = 14;
            XTEAIVTextBox.KeyDown += XTEAIVTextBox_KeyDown;
            XTEAIVTextBox.Leave += XTEAIVTextBox_LeaveAsync;
            // 
            // FolderChangeButton
            // 
            FolderChangeButton.Location = new Point(553, 299);
            FolderChangeButton.Name = "FolderChangeButton";
            FolderChangeButton.Size = new Size(117, 29);
            FolderChangeButton.TabIndex = 15;
            FolderChangeButton.Text = "Change Folder";
            FolderChangeButton.UseVisualStyleBackColor = true;
            FolderChangeButton.Click += FolderChangeButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(540, 263);
            label1.Name = "label1";
            label1.Size = new Size(253, 20);
            label1.TabIndex = 16;
            label1.Text = "Set Receiving Folder and Start Server";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(36, 264);
            label2.Name = "label2";
            label2.Size = new Size(213, 20);
            label2.TabIndex = 17;
            label2.Text = "Choose a File to send to Server";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(99, 187);
            label3.Name = "label3";
            label3.Size = new Size(80, 20);
            label3.TabIndex = 18;
            label3.Text = "Server Port";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(89, 101);
            label4.Name = "label4";
            label4.Size = new Size(112, 20);
            label4.TabIndex = 19;
            label4.Text = "Server IP adress";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(570, 101);
            label5.Name = "label5";
            label5.Size = new Size(184, 20);
            label5.TabIndex = 20;
            label5.Text = "IP example for local server";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(608, 187);
            label6.Name = "label6";
            label6.Size = new Size(105, 20);
            label6.TabIndex = 21;
            label6.Text = "Receiving port";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { changeWindowToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 22;
            menuStrip1.Text = "menuStrip1";
            // 
            // changeWindowToolStripMenuItem
            // 
            changeWindowToolStripMenuItem.BackColor = SystemColors.AppWorkspace;
            changeWindowToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { networkWindowToolStripMenuItem, fSWWindowToolStripMenuItem });
            changeWindowToolStripMenuItem.Name = "changeWindowToolStripMenuItem";
            changeWindowToolStripMenuItem.Size = new Size(132, 24);
            changeWindowToolStripMenuItem.Text = "Change Window";
            // 
            // networkWindowToolStripMenuItem
            // 
            networkWindowToolStripMenuItem.Name = "networkWindowToolStripMenuItem";
            networkWindowToolStripMenuItem.Size = new Size(207, 26);
            networkWindowToolStripMenuItem.Text = "Network Window";
            // 
            // fSWWindowToolStripMenuItem
            // 
            fSWWindowToolStripMenuItem.Name = "fSWWindowToolStripMenuItem";
            fSWWindowToolStripMenuItem.Size = new Size(207, 26);
            fSWWindowToolStripMenuItem.Text = "FSW Window";
            fSWWindowToolStripMenuItem.Click += fSWWindowToolStripMenuItem_Click;
            // 
            // NetworkTCP
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(FolderChangeButton);
            Controls.Add(XTEAIVTextBox);
            Controls.Add(XTEALabel);
            Controls.Add(RC4TextBox);
            Controls.Add(RC4KeyLabel);
            Controls.Add(ComboBoxEncryption);
            Controls.Add(EncryptionAlgorithmLabel);
            Controls.Add(BrowseFileButton);
            Controls.Add(SendButton);
            Controls.Add(ServerStartButton);
            Controls.Add(ServerPort);
            Controls.Add(PortServer);
            Controls.Add(ServerIP);
            Controls.Add(ClientIP);
            Controls.Add(ServerLabel);
            Controls.Add(LabelClient);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "NetworkTCP";
            Text = "NetworkTCP";
            Load += NetworkTCP_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LabelClient;
        private Label ServerLabel;
        private MaskedTextBox ClientIP;
        private MaskedTextBox ServerIP;
        private TextBox PortServer;
        private TextBox ServerPort;
        private Button ServerStartButton;
        private Button SendButton;
        private Button BrowseFileButton;
        private Label EncryptionAlgorithmLabel;
        internal ComboBox ComboBoxEncryption;
        private Label RC4KeyLabel;
        private TextBox RC4TextBox;
        private Label XTEALabel;
        private TextBox XTEAIVTextBox;
        private Button FolderChangeButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem changeWindowToolStripMenuItem;
        private ToolStripMenuItem networkWindowToolStripMenuItem;
        private ToolStripMenuItem fSWWindowToolStripMenuItem;
    }
}