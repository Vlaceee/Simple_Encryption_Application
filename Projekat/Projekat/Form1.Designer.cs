namespace Projekat
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ComboBoxEncryption = new ComboBox();
            RC4KeyLabel = new Label();
            EncryptionAlgorithmLabel = new Label();
            RC4TextBox = new TextBox();
            FileDialog = new OpenFileDialog();
            timer1 = new System.Windows.Forms.Timer(components);
            BrowseButton = new Button();
            checkBoxFSW = new CheckBox();
            XTEAIVTextBox = new TextBox();
            XTEALabel = new Label();
            DecodeButton = new Button();
            TargetView = new ListView();
            XView = new ListView();
            AddToTarget = new Button();
            DeleteFromX = new Button();
            RemoveTargetButton = new Button();
            menuStrip1 = new MenuStrip();
            goToToolStripMenuItem = new ToolStripMenuItem();
            networkWindowToolStripMenuItem = new ToolStripMenuItem();
            fSWWindowToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ComboBoxEncryption
            // 
            ComboBoxEncryption.AutoCompleteCustomSource.AddRange(new string[] { "RC4", "XTEA" });
            ComboBoxEncryption.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxEncryption.FormattingEnabled = true;
            ComboBoxEncryption.Items.AddRange(new object[] { "RC4", "XTEA" });
            ComboBoxEncryption.Location = new Point(409, 36);
            ComboBoxEncryption.Name = "ComboBoxEncryption";
            ComboBoxEncryption.Size = new Size(151, 28);
            ComboBoxEncryption.TabIndex = 0;
            ComboBoxEncryption.SelectedIndexChanged += ComboBoxEncryption_SelectedIndexChanged;
            // 
            // RC4KeyLabel
            // 
            RC4KeyLabel.AutoSize = true;
            RC4KeyLabel.Location = new Point(450, 77);
            RC4KeyLabel.Name = "RC4KeyLabel";
            RC4KeyLabel.Size = new Size(61, 20);
            RC4KeyLabel.TabIndex = 1;
            RC4KeyLabel.Tag = "RC4";
            RC4KeyLabel.Text = "The Key";
            RC4KeyLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EncryptionAlgorithmLabel
            // 
            EncryptionAlgorithmLabel.AutoSize = true;
            EncryptionAlgorithmLabel.Location = new Point(367, 9);
            EncryptionAlgorithmLabel.Name = "EncryptionAlgorithmLabel";
            EncryptionAlgorithmLabel.Size = new Size(230, 20);
            EncryptionAlgorithmLabel.TabIndex = 2;
            EncryptionAlgorithmLabel.Text = "Set Desired Encryption Algorithm";
            // 
            // RC4TextBox
            // 
            RC4TextBox.BackColor = SystemColors.InactiveBorder;
            RC4TextBox.Location = new Point(373, 109);
            RC4TextBox.Name = "RC4TextBox";
            RC4TextBox.PlaceholderText = "Enter the encryption key here...";
            RC4TextBox.Size = new Size(216, 27);
            RC4TextBox.TabIndex = 3;
            RC4TextBox.KeyDown += RC4TextBox_KeyDown;
            RC4TextBox.Leave += RC4TextBox_Leave;
            // 
            // FileDialog
            // 
            FileDialog.FileName = "openFileDialog1";
            FileDialog.FileOk += FileDialog_FileOk;
            // 
            // BrowseButton
            // 
            BrowseButton.Location = new Point(333, 254);
            BrowseButton.Name = "BrowseButton";
            BrowseButton.Size = new Size(151, 29);
            BrowseButton.TabIndex = 6;
            BrowseButton.Text = "Browse Files";
            BrowseButton.UseVisualStyleBackColor = true;
            BrowseButton.Click += BrowseButton_Click;
            // 
            // checkBoxFSW
            // 
            checkBoxFSW.AutoSize = true;
            checkBoxFSW.Location = new Point(515, 256);
            checkBoxFSW.Name = "checkBoxFSW";
            checkBoxFSW.Size = new Size(114, 24);
            checkBoxFSW.TabIndex = 7;
            checkBoxFSW.Text = "Turn on FSW";
            checkBoxFSW.UseVisualStyleBackColor = true;
            checkBoxFSW.CheckedChanged += CheckBoxFSW_CheckedChanged;
            // 
            // XTEAIVTextBox
            // 
            XTEAIVTextBox.BackColor = SystemColors.InactiveBorder;
            XTEAIVTextBox.Location = new Point(373, 190);
            XTEAIVTextBox.Name = "XTEAIVTextBox";
            XTEAIVTextBox.PlaceholderText = "Enter the encryption key here...";
            XTEAIVTextBox.Size = new Size(216, 27);
            XTEAIVTextBox.TabIndex = 8;
            XTEAIVTextBox.KeyDown += XTEAIVTextBox_KeyDown;
            XTEAIVTextBox.Leave += XTEAIVTextBox_LeaveAsync;
            // 
            // XTEALabel
            // 
            XTEALabel.AutoSize = true;
            XTEALabel.Location = new Point(454, 156);
            XTEALabel.Name = "XTEALabel";
            XTEALabel.Size = new Size(50, 20);
            XTEALabel.TabIndex = 9;
            XTEALabel.Tag = "XTEA";
            XTEALabel.Text = "IV Key";
            XTEALabel.TextAlign = ContentAlignment.MiddleCenter;
            XTEALabel.Click += XTEALabel_Click;
            // 
            // DecodeButton
            // 
            DecodeButton.Location = new Point(409, 313);
            DecodeButton.Name = "DecodeButton";
            DecodeButton.Size = new Size(151, 31);
            DecodeButton.TabIndex = 11;
            DecodeButton.Text = "Decode Files";
            DecodeButton.UseVisualStyleBackColor = true;
            DecodeButton.Click += DecodingButton_Click;
            // 
            // TargetView
            // 
            TargetView.Location = new Point(12, 46);
            TargetView.Name = "TargetView";
            TargetView.Size = new Size(290, 309);
            TargetView.TabIndex = 12;
            TargetView.UseCompatibleStateImageBehavior = false;
            TargetView.View = View.Tile;
            // 
            // XView
            // 
            XView.Location = new Point(654, 46);
            XView.Name = "XView";
            XView.Size = new Size(290, 309);
            XView.TabIndex = 13;
            XView.UseCompatibleStateImageBehavior = false;
            XView.View = View.Tile;
            // 
            // AddToTarget
            // 
            AddToTarget.Location = new Point(12, 371);
            AddToTarget.Name = "AddToTarget";
            AddToTarget.Size = new Size(151, 29);
            AddToTarget.TabIndex = 14;
            AddToTarget.Text = "Add File to Target";
            AddToTarget.UseVisualStyleBackColor = true;
            AddToTarget.Click += AddToTarget_Click;
            // 
            // DeleteFromX
            // 
            DeleteFromX.Location = new Point(692, 371);
            DeleteFromX.Name = "DeleteFromX";
            DeleteFromX.Size = new Size(207, 29);
            DeleteFromX.TabIndex = 15;
            DeleteFromX.Text = "Remove Selected from X";
            DeleteFromX.UseVisualStyleBackColor = true;
            DeleteFromX.Click += DeleteFromX_Click;
            // 
            // RemoveTargetButton
            // 
            RemoveTargetButton.Location = new Point(169, 371);
            RemoveTargetButton.Name = "RemoveTargetButton";
            RemoveTargetButton.Size = new Size(145, 29);
            RemoveTargetButton.TabIndex = 16;
            RemoveTargetButton.Text = "Remove Selected ";
            RemoveTargetButton.UseVisualStyleBackColor = true;
            RemoveTargetButton.Click += RemoveTargetButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { goToToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(956, 28);
            menuStrip1.TabIndex = 17;
            menuStrip1.Text = "menuStrip1";
            // 
            // goToToolStripMenuItem
            // 
            goToToolStripMenuItem.BackColor = SystemColors.AppWorkspace;
            goToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { networkWindowToolStripMenuItem, fSWWindowToolStripMenuItem });
            goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            goToToolStripMenuItem.Size = new Size(132, 24);
            goToToolStripMenuItem.Text = "Change Window";
            // 
            // networkWindowToolStripMenuItem
            // 
            networkWindowToolStripMenuItem.Name = "networkWindowToolStripMenuItem";
            networkWindowToolStripMenuItem.Size = new Size(207, 26);
            networkWindowToolStripMenuItem.Text = "Network Window";
            networkWindowToolStripMenuItem.Click += networkWindowToolStripMenuItem_Click;
            // 
            // fSWWindowToolStripMenuItem
            // 
            fSWWindowToolStripMenuItem.Name = "fSWWindowToolStripMenuItem";
            fSWWindowToolStripMenuItem.Size = new Size(207, 26);
            fSWWindowToolStripMenuItem.Text = "FSW Window";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(956, 447);
            Controls.Add(RemoveTargetButton);
            Controls.Add(DeleteFromX);
            Controls.Add(AddToTarget);
            Controls.Add(XView);
            Controls.Add(TargetView);
            Controls.Add(DecodeButton);
            Controls.Add(XTEALabel);
            Controls.Add(XTEAIVTextBox);
            Controls.Add(checkBoxFSW);
            Controls.Add(BrowseButton);
            Controls.Add(RC4TextBox);
            Controls.Add(EncryptionAlgorithmLabel);
            Controls.Add(RC4KeyLabel);
            Controls.Add(ComboBoxEncryption);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Tag = "RC4";
            Text = "ZastitaInformacija";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void RC4TextBox_KeyDown1(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal ComboBox ComboBoxEncryption;
        private Label RC4KeyLabel;
        private Label EncryptionAlgorithmLabel;
        private TextBox RC4TextBox;
        private OpenFileDialog FileDialog;
        private System.Windows.Forms.Timer timer1;
        private Button BrowseButton;
        private CheckBox checkBoxFSW;
        private TextBox XTEAIVTextBox;
        private Label XTEALabel;
        private Button DecodeButton;
        private ListView TargetView;
        private ListView XView;
        private Button AddToTarget;
        private Button DeleteFromX;
        private Button RemoveTargetButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem goToToolStripMenuItem;
        private ToolStripMenuItem networkWindowToolStripMenuItem;
        private ToolStripMenuItem fSWWindowToolStripMenuItem;
    }
}
