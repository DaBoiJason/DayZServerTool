

namespace DayZ_Server_Tool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            refreshProfilesToolStripMenuItem = new ToolStripMenuItem();
            serverToolStripMenuItem = new ToolStripMenuItem();
            startToolStripMenuItem = new ToolStripMenuItem();
            stopToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            discordServerToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdatesToolStripMenuItem = new ToolStripMenuItem();
            version100ToolStripMenuItem = new ToolStripMenuItem();
            tabPage7 = new TabPage();
            label7 = new Label();
            webhookTextBox = new TextBox();
            StartWebhookCheckbox = new CheckBox();
            label12 = new Label();
            StopWebhookCheckbox = new CheckBox();
            EnableWebhookCheckbox = new CheckBox();
            RestartWebhookCheckbox = new CheckBox();
            tabPage6 = new TabPage();
            timeremaininglabel = new Label();
            RestartProgressBar = new ProgressBar();
            numericUpDownSeconds = new NumericUpDown();
            label6 = new Label();
            numericUpDownMinutes = new NumericUpDown();
            label10 = new Label();
            numericUpDownHours = new NumericUpDown();
            label11 = new Label();
            checkBoxEnableTimer = new CheckBox();
            tabPage5 = new TabPage();
            checkBoxAllowExtraParams = new CheckBox();
            textBoxParameters = new TextBox();
            label9 = new Label();
            tabPage4 = new TabPage();
            checkBoxNetLog = new CheckBox();
            checkBoxAdminLog = new CheckBox();
            checkBoxDoLogs = new CheckBox();
            tabPage3 = new TabPage();
            progressBar1 = new ProgressBar();
            progressBar = new ProgressBar();
            modDir = new TextBox();
            textBoxMods = new TextBox();
            Mods = new Button();
            Keys = new Button();
            label4 = new Label();
            ModsCheckedListBox = new CheckedListBox();
            buttonUpdateMods = new Button();
            buttonBrowseMods = new Button();
            label8 = new Label();
            tabPage2 = new TabPage();
            label3 = new Label();
            comboBoxCpu = new ComboBox();
            textBoxConfig = new TextBox();
            textBoxPort = new TextBox();
            textBoxExePath = new TextBox();
            checkBox1 = new CheckBox();
            label5 = new Label();
            ConfigLabel = new Label();
            PortLabel = new Label();
            buttonBrowseExe = new Button();
            label2 = new Label();
            tabPage1 = new TabPage();
            DeleteProfile = new Button();
            loadFileToolStripMenu = new Button();
            comboBoxProfiles = new ComboBox();
            buttonStop = new Button();
            buttonStart = new Button();
            newProfileButton = new Button();
            saveFileToolStripMenuItem = new Button();
            label1 = new Label();
            tabControl1 = new TabControl();
            menuStrip1.SuspendLayout();
            tabPage7.SuspendLayout();
            tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHours).BeginInit();
            tabPage5.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.Gainsboro;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, serverToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(612, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "Menu Strip Main";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshProfilesToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(72, 24);
            fileToolStripMenuItem.Text = "Profiles";
            // 
            // refreshProfilesToolStripMenuItem
            // 
            refreshProfilesToolStripMenuItem.Name = "refreshProfilesToolStripMenuItem";
            refreshProfilesToolStripMenuItem.Size = new Size(194, 26);
            refreshProfilesToolStripMenuItem.Text = "Refresh Profiles";
            refreshProfilesToolStripMenuItem.Click += refreshProfilesToolStripMenuItem_Click_1;
            // 
            // serverToolStripMenuItem
            // 
            serverToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startToolStripMenuItem, stopToolStripMenuItem });
            serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            serverToolStripMenuItem.Size = new Size(64, 24);
            serverToolStripMenuItem.Text = "Server";
            // 
            // startToolStripMenuItem
            // 
            startToolStripMenuItem.Name = "startToolStripMenuItem";
            startToolStripMenuItem.Size = new Size(123, 26);
            startToolStripMenuItem.Text = "Start";
            // 
            // stopToolStripMenuItem
            // 
            stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            stopToolStripMenuItem.Size = new Size(123, 26);
            stopToolStripMenuItem.Text = "Stop";
            stopToolStripMenuItem.Click += stopToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { discordServerToolStripMenuItem, checkForUpdatesToolStripMenuItem, version100ToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(64, 24);
            aboutToolStripMenuItem.Text = "About";
            // 
            // discordServerToolStripMenuItem
            // 
            discordServerToolStripMenuItem.Name = "discordServerToolStripMenuItem";
            discordServerToolStripMenuItem.Size = new Size(215, 26);
            discordServerToolStripMenuItem.Text = "Discord Server";
            discordServerToolStripMenuItem.Click += discordServerToolStripMenuItem_Click;
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            checkForUpdatesToolStripMenuItem.Size = new Size(215, 26);
            checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            checkForUpdatesToolStripMenuItem.Click += checkForUpdatesToolStripMenuItem_Click;
            // 
            // version100ToolStripMenuItem
            // 
            version100ToolStripMenuItem.Name = "version100ToolStripMenuItem";
            version100ToolStripMenuItem.Size = new Size(215, 26);
            version100ToolStripMenuItem.Text = "Version 2.0.0";
            // 
            // tabPage7
            // 
            tabPage7.Controls.Add(label7);
            tabPage7.Controls.Add(webhookTextBox);
            tabPage7.Controls.Add(StartWebhookCheckbox);
            tabPage7.Controls.Add(label12);
            tabPage7.Controls.Add(StopWebhookCheckbox);
            tabPage7.Controls.Add(EnableWebhookCheckbox);
            tabPage7.Controls.Add(RestartWebhookCheckbox);
            tabPage7.Location = new Point(4, 29);
            tabPage7.Name = "tabPage7";
            tabPage7.Size = new Size(583, 484);
            tabPage7.TabIndex = 6;
            tabPage7.Text = "Discord Webhook";
            tabPage7.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(55, 5);
            label7.Name = "label7";
            label7.Size = new Size(452, 31);
            label7.TabIndex = 1;
            label7.Text = "Discord Webhook Menue (Coming Soon)";
            // 
            // webhookTextBox
            // 
            webhookTextBox.Enabled = false;
            webhookTextBox.Location = new Point(3, 100);
            webhookTextBox.Name = "webhookTextBox";
            webhookTextBox.Size = new Size(577, 27);
            webhookTextBox.TabIndex = 2;
            // 
            // StartWebhookCheckbox
            // 
            StartWebhookCheckbox.AutoSize = true;
            StartWebhookCheckbox.Enabled = false;
            StartWebhookCheckbox.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            StartWebhookCheckbox.ForeColor = Color.Green;
            StartWebhookCheckbox.Location = new Point(55, 133);
            StartWebhookCheckbox.Name = "StartWebhookCheckbox";
            StartWebhookCheckbox.Size = new Size(104, 29);
            StartWebhookCheckbox.TabIndex = 3;
            StartWebhookCheckbox.Text = "On Start";
            StartWebhookCheckbox.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.Location = new Point(209, 74);
            label12.Name = "label12";
            label12.Size = new Size(153, 23);
            label12.TabIndex = 1;
            label12.Text = "Discord Webhook :";
            // 
            // StopWebhookCheckbox
            // 
            StopWebhookCheckbox.AutoSize = true;
            StopWebhookCheckbox.Enabled = false;
            StopWebhookCheckbox.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            StopWebhookCheckbox.ForeColor = Color.Red;
            StopWebhookCheckbox.Location = new Point(196, 133);
            StopWebhookCheckbox.Name = "StopWebhookCheckbox";
            StopWebhookCheckbox.Size = new Size(181, 29);
            StopWebhookCheckbox.TabIndex = 3;
            StopWebhookCheckbox.Text = "On (Manual) Stop";
            StopWebhookCheckbox.UseVisualStyleBackColor = true;
            // 
            // EnableWebhookCheckbox
            // 
            EnableWebhookCheckbox.AutoSize = true;
            EnableWebhookCheckbox.Enabled = false;
            EnableWebhookCheckbox.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            EnableWebhookCheckbox.ForeColor = Color.Red;
            EnableWebhookCheckbox.Location = new Point(144, 39);
            EnableWebhookCheckbox.Name = "EnableWebhookCheckbox";
            EnableWebhookCheckbox.Size = new Size(292, 32);
            EnableWebhookCheckbox.TabIndex = 0;
            EnableWebhookCheckbox.Text = "Enable Webhook Messaging";
            EnableWebhookCheckbox.UseVisualStyleBackColor = true;
            EnableWebhookCheckbox.CheckedChanged += EnableWebhookCheckbox_CheckedChanged;
            // 
            // RestartWebhookCheckbox
            // 
            RestartWebhookCheckbox.AutoSize = true;
            RestartWebhookCheckbox.Enabled = false;
            RestartWebhookCheckbox.Font = new Font("Segoe UI Semibold", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            RestartWebhookCheckbox.ForeColor = Color.FromArgb(0, 0, 192);
            RestartWebhookCheckbox.Location = new Point(409, 133);
            RestartWebhookCheckbox.Name = "RestartWebhookCheckbox";
            RestartWebhookCheckbox.Size = new Size(124, 29);
            RestartWebhookCheckbox.TabIndex = 3;
            RestartWebhookCheckbox.Text = "On Restart";
            RestartWebhookCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(timeremaininglabel);
            tabPage6.Controls.Add(RestartProgressBar);
            tabPage6.Controls.Add(numericUpDownSeconds);
            tabPage6.Controls.Add(label6);
            tabPage6.Controls.Add(numericUpDownMinutes);
            tabPage6.Controls.Add(label10);
            tabPage6.Controls.Add(numericUpDownHours);
            tabPage6.Controls.Add(label11);
            tabPage6.Controls.Add(checkBoxEnableTimer);
            tabPage6.Location = new Point(4, 29);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new Size(583, 484);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "Restart";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // timeremaininglabel
            // 
            timeremaininglabel.AutoSize = true;
            timeremaininglabel.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            timeremaininglabel.ForeColor = Color.FromArgb(0, 0, 192);
            timeremaininglabel.Location = new Point(4, 208);
            timeremaininglabel.Name = "timeremaininglabel";
            timeremaininglabel.Size = new Size(230, 38);
            timeremaininglabel.TabIndex = 5;
            timeremaininglabel.Text = "Time Remaining";
            // 
            // RestartProgressBar
            // 
            RestartProgressBar.Location = new Point(12, 279);
            RestartProgressBar.Name = "RestartProgressBar";
            RestartProgressBar.Size = new Size(541, 33);
            RestartProgressBar.TabIndex = 4;
            // 
            // numericUpDownSeconds
            // 
            numericUpDownSeconds.Enabled = false;
            numericUpDownSeconds.Font = new Font("Segoe UI", 12F);
            numericUpDownSeconds.Location = new Point(190, 68);
            numericUpDownSeconds.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numericUpDownSeconds.Name = "numericUpDownSeconds";
            numericUpDownSeconds.Size = new Size(63, 34);
            numericUpDownSeconds.TabIndex = 3;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(11, 14);
            label6.Name = "label6";
            label6.Size = new Size(64, 28);
            label6.TabIndex = 1;
            label6.Text = "Hours";
            // 
            // numericUpDownMinutes
            // 
            numericUpDownMinutes.Enabled = false;
            numericUpDownMinutes.Font = new Font("Segoe UI", 12F);
            numericUpDownMinutes.Location = new Point(99, 68);
            numericUpDownMinutes.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numericUpDownMinutes.Name = "numericUpDownMinutes";
            numericUpDownMinutes.Size = new Size(63, 34);
            numericUpDownMinutes.TabIndex = 3;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(89, 14);
            label10.Name = "label10";
            label10.Size = new Size(82, 28);
            label10.TabIndex = 1;
            label10.Text = "Minutes";
            // 
            // numericUpDownHours
            // 
            numericUpDownHours.Enabled = false;
            numericUpDownHours.Font = new Font("Segoe UI", 12F);
            numericUpDownHours.Location = new Point(12, 68);
            numericUpDownHours.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            numericUpDownHours.Name = "numericUpDownHours";
            numericUpDownHours.Size = new Size(63, 34);
            numericUpDownHours.TabIndex = 3;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(177, 14);
            label11.Name = "label11";
            label11.Size = new Size(85, 28);
            label11.TabIndex = 1;
            label11.Text = "Seconds";
            // 
            // checkBoxEnableTimer
            // 
            checkBoxEnableTimer.AutoSize = true;
            checkBoxEnableTimer.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxEnableTimer.ForeColor = Color.Red;
            checkBoxEnableTimer.Location = new Point(11, 141);
            checkBoxEnableTimer.Name = "checkBoxEnableTimer";
            checkBoxEnableTimer.Size = new Size(208, 35);
            checkBoxEnableTimer.TabIndex = 2;
            checkBoxEnableTimer.Text = "⚠Enable Timer";
            checkBoxEnableTimer.UseVisualStyleBackColor = true;
            checkBoxEnableTimer.CheckedChanged += checkBoxEnableTimer_CheckedChanged;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(checkBoxAllowExtraParams);
            tabPage5.Controls.Add(textBoxParameters);
            tabPage5.Controls.Add(label9);
            tabPage5.Location = new Point(4, 29);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(583, 484);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Parameters";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // checkBoxAllowExtraParams
            // 
            checkBoxAllowExtraParams.AutoSize = true;
            checkBoxAllowExtraParams.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkBoxAllowExtraParams.ForeColor = Color.Red;
            checkBoxAllowExtraParams.Location = new Point(142, 13);
            checkBoxAllowExtraParams.Name = "checkBoxAllowExtraParams";
            checkBoxAllowExtraParams.Size = new Size(290, 32);
            checkBoxAllowExtraParams.TabIndex = 6;
            checkBoxAllowExtraParams.Text = "⚠️ Allow Extra Parameters";
            checkBoxAllowExtraParams.UseVisualStyleBackColor = true;
            // 
            // textBoxParameters
            // 
            textBoxParameters.Location = new Point(18, 95);
            textBoxParameters.Name = "textBoxParameters";
            textBoxParameters.Size = new Size(513, 27);
            textBoxParameters.TabIndex = 5;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(243, 54);
            label9.Name = "label9";
            label9.Size = new Size(103, 25);
            label9.TabIndex = 4;
            label9.Text = "Parameters:";
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(checkBoxNetLog);
            tabPage4.Controls.Add(checkBoxAdminLog);
            tabPage4.Controls.Add(checkBoxDoLogs);
            tabPage4.Location = new Point(4, 29);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(583, 484);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Logs";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // checkBoxNetLog
            // 
            checkBoxNetLog.AutoSize = true;
            checkBoxNetLog.Location = new Point(22, 123);
            checkBoxNetLog.Name = "checkBoxNetLog";
            checkBoxNetLog.Size = new Size(90, 24);
            checkBoxNetLog.TabIndex = 2;
            checkBoxNetLog.Text = "Net Logs";
            checkBoxNetLog.UseVisualStyleBackColor = true;
            // 
            // checkBoxAdminLog
            // 
            checkBoxAdminLog.AutoSize = true;
            checkBoxAdminLog.Location = new Point(22, 73);
            checkBoxAdminLog.Name = "checkBoxAdminLog";
            checkBoxAdminLog.Size = new Size(110, 24);
            checkBoxAdminLog.TabIndex = 1;
            checkBoxAdminLog.Text = "Admin Logs";
            checkBoxAdminLog.UseVisualStyleBackColor = true;
            // 
            // checkBoxDoLogs
            // 
            checkBoxDoLogs.AutoSize = true;
            checkBoxDoLogs.Location = new Point(22, 25);
            checkBoxDoLogs.Name = "checkBoxDoLogs";
            checkBoxDoLogs.Size = new Size(86, 24);
            checkBoxDoLogs.TabIndex = 0;
            checkBoxDoLogs.Text = "Do Logs";
            checkBoxDoLogs.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(progressBar1);
            tabPage3.Controls.Add(progressBar);
            tabPage3.Controls.Add(modDir);
            tabPage3.Controls.Add(textBoxMods);
            tabPage3.Controls.Add(Mods);
            tabPage3.Controls.Add(Keys);
            tabPage3.Controls.Add(label4);
            tabPage3.Controls.Add(ModsCheckedListBox);
            tabPage3.Controls.Add(buttonUpdateMods);
            tabPage3.Controls.Add(buttonBrowseMods);
            tabPage3.Controls.Add(label8);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(583, 484);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Mods";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(399, 174);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(181, 29);
            progressBar1.TabIndex = 12;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(399, 139);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(181, 29);
            progressBar.TabIndex = 11;
            // 
            // modDir
            // 
            modDir.Location = new Point(3, 76);
            modDir.Name = "modDir";
            modDir.ReadOnly = true;
            modDir.Size = new Size(577, 27);
            modDir.TabIndex = 10;
            // 
            // textBoxMods
            // 
            textBoxMods.Location = new Point(3, 37);
            textBoxMods.Name = "textBoxMods";
            textBoxMods.Size = new Size(577, 27);
            textBoxMods.TabIndex = 0;
            // 
            // Mods
            // 
            Mods.Enabled = false;
            Mods.Location = new Point(132, 174);
            Mods.Name = "Mods";
            Mods.Size = new Size(261, 29);
            Mods.TabIndex = 9;
            Mods.Text = "Copy Paste Selected Mods";
            Mods.UseVisualStyleBackColor = true;
            Mods.Click += Mods_Click;
            // 
            // Keys
            // 
            Keys.Enabled = false;
            Keys.Location = new Point(132, 139);
            Keys.Name = "Keys";
            Keys.Size = new Size(261, 29);
            Keys.TabIndex = 8;
            Keys.Text = "Copy Paste Keys Of Selected Mods";
            Keys.UseVisualStyleBackColor = true;
            Keys.Click += Keys_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(33, 106);
            label4.Name = "label4";
            label4.Size = new Size(447, 20);
            label4.TabIndex = 7;
            label4.Text = " Select !Workshop Folder in steamapps\\common\\DayZ\\!Workshop";
            // 
            // ModsCheckedListBox
            // 
            ModsCheckedListBox.FormattingEnabled = true;
            ModsCheckedListBox.Location = new Point(3, 214);
            ModsCheckedListBox.Name = "ModsCheckedListBox";
            ModsCheckedListBox.Size = new Size(577, 268);
            ModsCheckedListBox.TabIndex = 6;
            // 
            // buttonUpdateMods
            // 
            buttonUpdateMods.Location = new Point(3, 174);
            buttonUpdateMods.Name = "buttonUpdateMods";
            buttonUpdateMods.Size = new Size(123, 29);
            buttonUpdateMods.TabIndex = 5;
            buttonUpdateMods.Text = "Select Mods";
            buttonUpdateMods.UseVisualStyleBackColor = true;
            buttonUpdateMods.Click += buttonUpdateMods_Click;
            // 
            // buttonBrowseMods
            // 
            buttonBrowseMods.Location = new Point(3, 139);
            buttonBrowseMods.Name = "buttonBrowseMods";
            buttonBrowseMods.Size = new Size(123, 29);
            buttonBrowseMods.TabIndex = 2;
            buttonBrowseMods.Text = "Browse Mods";
            buttonBrowseMods.UseVisualStyleBackColor = true;
            buttonBrowseMods.Click += buttonBrowseMods_Click;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(231, 9);
            label8.Name = "label8";
            label8.Size = new Size(93, 25);
            label8.TabIndex = 4;
            label8.Text = "Mods List:";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(comboBoxCpu);
            tabPage2.Controls.Add(textBoxConfig);
            tabPage2.Controls.Add(textBoxPort);
            tabPage2.Controls.Add(textBoxExePath);
            tabPage2.Controls.Add(checkBox1);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(ConfigLabel);
            tabPage2.Controls.Add(PortLabel);
            tabPage2.Controls.Add(buttonBrowseExe);
            tabPage2.Controls.Add(label2);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(583, 484);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Main";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(350, 375);
            label3.Name = "label3";
            label3.Size = new Size(123, 20);
            label3.TabIndex = 10;
            label3.Text = "Default is 4 cores";
            // 
            // comboBoxCpu
            // 
            comboBoxCpu.FormattingEnabled = true;
            comboBoxCpu.Location = new Point(193, 372);
            comboBoxCpu.Name = "comboBoxCpu";
            comboBoxCpu.Size = new Size(151, 28);
            comboBoxCpu.TabIndex = 9;
            // 
            // textBoxConfig
            // 
            textBoxConfig.Location = new Point(6, 293);
            textBoxConfig.Name = "textBoxConfig";
            textBoxConfig.Size = new Size(571, 27);
            textBoxConfig.TabIndex = 8;
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(6, 176);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(571, 27);
            textBoxPort.TabIndex = 7;
            // 
            // textBoxExePath
            // 
            textBoxExePath.Location = new Point(6, 58);
            textBoxExePath.Name = "textBoxExePath";
            textBoxExePath.Size = new Size(571, 27);
            textBoxExePath.TabIndex = 1;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(212, 441);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(117, 24);
            checkBox1.TabIndex = 6;
            checkBox1.Text = "Freeze Check";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(245, 335);
            label5.Name = "label5";
            label5.Size = new Size(49, 25);
            label5.TabIndex = 5;
            label5.Text = "CPU:";
            // 
            // ConfigLabel
            // 
            ConfigLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ConfigLabel.AutoSize = true;
            ConfigLabel.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ConfigLabel.Location = new Point(245, 239);
            ConfigLabel.Name = "ConfigLabel";
            ConfigLabel.Size = new Size(69, 25);
            ConfigLabel.TabIndex = 4;
            ConfigLabel.Text = "Config:";
            // 
            // PortLabel
            // 
            PortLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PortLabel.AutoSize = true;
            PortLabel.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PortLabel.Location = new Point(245, 133);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(48, 25);
            PortLabel.TabIndex = 3;
            PortLabel.Text = "Port:";
            // 
            // buttonBrowseExe
            // 
            buttonBrowseExe.Location = new Point(223, 89);
            buttonBrowseExe.Name = "buttonBrowseExe";
            buttonBrowseExe.Size = new Size(94, 29);
            buttonBrowseExe.TabIndex = 2;
            buttonBrowseExe.Text = "Browse";
            buttonBrowseExe.UseVisualStyleBackColor = true;
            buttonBrowseExe.Click += buttonBrowseExe_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(165, 28);
            label2.Name = "label2";
            label2.Size = new Size(200, 25);
            label2.TabIndex = 0;
            label2.Text = "Server's Executable Path";
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.Transparent;
            tabPage1.Controls.Add(DeleteProfile);
            tabPage1.Controls.Add(loadFileToolStripMenu);
            tabPage1.Controls.Add(comboBoxProfiles);
            tabPage1.Controls.Add(buttonStop);
            tabPage1.Controls.Add(buttonStart);
            tabPage1.Controls.Add(newProfileButton);
            tabPage1.Controls.Add(saveFileToolStripMenuItem);
            tabPage1.Controls.Add(label1);
            tabPage1.ImageKey = "(none)";
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(583, 484);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Server";
            // 
            // DeleteProfile
            // 
            DeleteProfile.BackColor = Color.Red;
            DeleteProfile.ForeColor = Color.Black;
            DeleteProfile.Location = new Point(244, 62);
            DeleteProfile.Name = "DeleteProfile";
            DeleteProfile.Size = new Size(220, 43);
            DeleteProfile.TabIndex = 11;
            DeleteProfile.Text = "Delete";
            DeleteProfile.UseVisualStyleBackColor = false;
            DeleteProfile.Click += DeleteProfile_Click;
            // 
            // loadFileToolStripMenu
            // 
            loadFileToolStripMenu.BackColor = Color.Transparent;
            loadFileToolStripMenu.Location = new Point(244, 12);
            loadFileToolStripMenu.Name = "loadFileToolStripMenu";
            loadFileToolStripMenu.Size = new Size(114, 44);
            loadFileToolStripMenu.TabIndex = 10;
            loadFileToolStripMenu.Text = "Load";
            loadFileToolStripMenu.UseVisualStyleBackColor = false;
            loadFileToolStripMenu.Click += loadFileToolStripMenu_Click;
            // 
            // comboBoxProfiles
            // 
            comboBoxProfiles.FormattingEnabled = true;
            comboBoxProfiles.Location = new Point(19, 62);
            comboBoxProfiles.Name = "comboBoxProfiles";
            comboBoxProfiles.Size = new Size(195, 28);
            comboBoxProfiles.TabIndex = 9;
            comboBoxProfiles.SelectedIndexChanged += comboBoxProfiles_SelectedIndexChanged_1;
            // 
            // buttonStop
            // 
            buttonStop.BackColor = Color.IndianRed;
            buttonStop.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold);
            buttonStop.Location = new Point(329, 286);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(135, 63);
            buttonStop.TabIndex = 6;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = false;
            buttonStop.Click += buttonStop_Click;
            // 
            // buttonStart
            // 
            buttonStart.BackColor = Color.SpringGreen;
            buttonStart.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonStart.Location = new Point(101, 286);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(135, 63);
            buttonStart.TabIndex = 5;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += buttonStart_Click;
            // 
            // newProfileButton
            // 
            newProfileButton.BackColor = Color.Transparent;
            newProfileButton.Location = new Point(470, 12);
            newProfileButton.Name = "newProfileButton";
            newProfileButton.Size = new Size(94, 93);
            newProfileButton.TabIndex = 4;
            newProfileButton.Text = "New Profile";
            newProfileButton.UseVisualStyleBackColor = false;
            newProfileButton.Click += newProfileButton_Click;
            // 
            // saveFileToolStripMenuItem
            // 
            saveFileToolStripMenuItem.BackColor = Color.Transparent;
            saveFileToolStripMenuItem.Location = new Point(364, 12);
            saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
            saveFileToolStripMenuItem.Size = new Size(100, 44);
            saveFileToolStripMenuItem.TabIndex = 3;
            saveFileToolStripMenuItem.Text = "Save";
            saveFileToolStripMenuItem.UseVisualStyleBackColor = false;
            saveFileToolStripMenuItem.Click += saveFileToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(57, 18);
            label1.Name = "label1";
            label1.Size = new Size(112, 38);
            label1.TabIndex = 0;
            label1.Text = "Profile:";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Controls.Add(tabPage7);
            tabControl1.Location = new Point(12, 31);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(591, 517);
            tabControl1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.DarkGray;
            ClientSize = new Size(612, 556);
            Controls.Add(tabControl1);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            Tag = "";
            Text = "DaBoiJason's DayZ Server Tool";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tabPage7.ResumeLayout(false);
            tabPage7.PerformLayout();
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownHours).EndInit();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem serverToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem discordServerToolStripMenuItem;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private Button LoadProfile;
        private ToolStripMenuItem refreshProfilesToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ToolStripMenuItem version100ToolStripMenuItem;
        private TabPage tabPage7;
        private TabPage tabPage6;
        private Label timeremaininglabel;
        private ProgressBar RestartProgressBar;
        private NumericUpDown numericUpDownSeconds;
        private NumericUpDown numericUpDownMinutes;
        private NumericUpDown numericUpDownHours;
        private CheckBox checkBoxEnableTimer;
        private Label label11;
        private Label label10;
        private Label label6;
        private TabPage tabPage5;
        private CheckBox checkBoxAllowExtraParams;
        private TextBox textBoxParameters;
        private Label label9;
        private TabPage tabPage4;
        private CheckBox checkBoxNetLog;
        private CheckBox checkBoxAdminLog;
        private CheckBox checkBoxDoLogs;
        private TabPage tabPage3;
        private ProgressBar progressBar1;
        private ProgressBar progressBar;
        private TextBox modDir;
        private TextBox textBoxMods;
        private Button Mods;
        private Button Keys;
        private Label label4;
        private CheckedListBox ModsCheckedListBox;
        private Button buttonUpdateMods;
        private Button buttonBrowseMods;
        private Label label8;
        private TabPage tabPage2;
        private Label label3;
        private ComboBox comboBoxCpu;
        private TextBox textBoxConfig;
        private TextBox textBoxPort;
        private TextBox textBoxExePath;
        private CheckBox checkBox1;
        private Label label5;
        private Label ConfigLabel;
        private Label PortLabel;
        private Button buttonBrowseExe;
        private Label label2;
        private TabPage tabPage1;
        private Button DeleteProfile;
        private Button loadFileToolStripMenu;
        private ComboBox comboBoxProfiles;
        private Button buttonStop;
        private Button buttonStart;
        private Button newProfileButton;
        private Button saveFileToolStripMenuItem;
        private Label label1;
        private TabControl tabControl1;
        private TextBox webhookTextBox;
        private Label label12;
        private Label label7;
        private CheckBox EnableWebhookCheckbox;
        private CheckBox RestartWebhookCheckbox;
        private CheckBox StopWebhookCheckbox;
        private CheckBox StartWebhookCheckbox;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private Label webhookColor;
    }
}
