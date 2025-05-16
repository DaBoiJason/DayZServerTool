using System;
using System.IO;
using BytexDigital.BattlEye.Rcon;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Responses;
using BytexDigital.BattlEye.Rcon.Events;
using BytexDigital.BattlEye.Rcon.Domain;
using BytexDigital.BattlEye.Rcon.HashAlgorithms;
using BytexDigital.BattlEye.Rcon.Requests;
using System.Net;
using System.Text;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Diagnostics;
using System.Net.Sockets;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.ListView;
using System.Reflection.Emit;



namespace DayZ_Server_Tool
{
    public partial class Form1 : Form
    {
        // Properties to hold profile data NO TALL INCLUDED
        private string repoZipUrl = "https://github.com/DaBoiJason/DayZServerTool/archive/refs/heads/main.zip";
        private string CurrentVersion = "3.4.0";
        private string executablePath;
        private string parameters;
        private string port;
        private string config;
        private string cpu;
        private TimeSpan userDefinedInterval;
        private bool isManualStop = false;
        private System.Threading.Timer restartTimer;
        private DateTime endTime;
        private string webhookUrl;
        private PerformanceCounter cpuCounterApp;
        private PerformanceCounter ramCounterApp;
        private PerformanceCounter cpuCounterDayZ;
        private PerformanceCounter ramCounterDayZ;
        public Player Player { get; set; }
        private RconClient networkClient;

        public Form1()
        {
            InitializeComponent();
            LoadProfilesFromDirectory();
            InitializeCpuDropdown();
            textBoxParameters.Enabled = false;
            modDir.Enabled = false;
            comboBoxProfiles.SelectedIndex = -1;
            ToggleTimerFields();
            LoadLatestProfileOnStart();
            loadFileToolStripMenu_Click(null, null);
            tabControl1_SelectedIndexChanged(null, null);
            InitializeTooltip();
            CheckForUpdatesAsync();
            VersionNumber.Text = $"Version {CurrentVersion}";
            RconPasswordTextBox.PasswordChar = '*';
            ShowRconPass.Enabled = true;
            RconPasswordTextBox.TextChanged += RconPasswordTextBox_TextChanged;
            ShowRconPass.MouseDown += ShowRconPass_MouseDown;
            ShowRconPass.MouseUp += ShowRconPass_MouseUp;
            checkBoxAllowExtraParams.CheckedChanged += CheckBoxAllowExtraParams_CheckedChanged;
            RconEnableCheckBox.CheckedChanged += rconEnableCheckBox_CheckedChanged;
            GetLocalIPv4Address();
            PopulateRconConfigFileComboBox();
            // Update the initialization of the RconClient with the required parameters
            networkClient = new RconClient(GetLocalIPv4Address(), int.Parse(RconPortNumeric.Text), RconPasswordTextBox.Text);


            // Initialize performance counters for the app
            cpuCounterApp = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            ramCounterApp = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);

            // Initialize performance counters for the DayZServer_x64.exe process
            cpuCounterDayZ = new PerformanceCounter("Process", "% Processor Time", "DayZServer_x64");
            ramCounterDayZ = new PerformanceCounter("Process", "Working Set - Private", "DayZServer_x64");

            // Start the timer
            timerUpdateUsage.Tick += new EventHandler(timerUpdateUsage_Tick);
            timerUpdateUsage.Start();

            // Subscribe to player connect and disconnect events


            // Sample data for online players
            ListViewItem player1 = new ListViewItem("1");
            player1.SubItems.Add("Jason");
            player1.SubItems.Add("9db32ffdb8ba5875eb752dda6afb438e");
            player1.SubItems.Add("74.125.20.101");
            player1.SubItems.Add("42");
            onlinePlayerListView.Items.Add(player1);

            ListViewItem player2 = new ListViewItem("2");
            player2.SubItems.Add("John");
            player2.SubItems.Add("9db32ffdb8ba5875eb752dda6afb438e");
            player2.SubItems.Add("74.125.20.101");
            player2.SubItems.Add("72");
            onlinePlayerListView.Items.Add(player2);

            ListViewItem player3 = new ListViewItem("3");
            player3.SubItems.Add("Doe");
            player3.SubItems.Add("9db32ffdb8ba5875eb752dda6afb438e");
            player3.SubItems.Add("74.125.20.101");
            player3.SubItems.Add("44");
            onlinePlayerListView.Items.Add(player3);
        }
        public class LatestProfile
        {
            public string LatestProfileName { get; set; }
        }
        private void CheckModsInList(CheckedListBox checkedListBox, string modsText)
        {
            if (!string.IsNullOrWhiteSpace(modsText))
            {
                string[] selectedMods = modsText.Split(';').Select(mod => mod.Trim()).ToArray();
                for (int i = 0; i < checkedListBox.Items.Count; i++)
                {
                    string currentMod = checkedListBox.Items[i].ToString();
                    if (selectedMods.Contains(currentMod))
                    {
                        checkedListBox.SetItemChecked(i, true);
                    }
                }
            }
        }
        private void SaveLatestProfile(string profileName)
        {
            LatestProfile latestProfile = new LatestProfile
            {
                LatestProfileName = profileName
            };

            string json = JsonConvert.SerializeObject(latestProfile, Formatting.Indented);
            string latestProfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LatestProfile.json");
            File.WriteAllText(latestProfilePath, json);
        }
        private void LoadLatestProfileOnStart()
        {
            string latestProfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LatestProfile.json");

            if (File.Exists(latestProfilePath))
            {
                string json = File.ReadAllText(latestProfilePath);
                var latestProfile = JsonConvert.DeserializeObject<LatestProfile>(json);

                // Load the latest profile if it exists
                if (!string.IsNullOrEmpty(latestProfile.LatestProfileName))
                {
                    comboBoxProfiles.SelectedItem = latestProfile.LatestProfileName; // Set selected item
                }
            }
        }
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxProfiles.Text))
            {
                MessageBox.Show("Profile name cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string profileName = comboBoxProfiles.Text;
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(directoryPath, profileName + ".json");

            // Create an object to hold the profile data
            var profileData = new
            {
                ModsDir = modDir.Text,
                ExecutablePath = textBoxExePath.Text,
                Parameters = textBoxParameters.Text,
                Port = textBoxPort.Text,
                Config = textBoxConfig.Text,
                Cpu = comboBoxCpu.SelectedItem?.ToString(),
                FreezeCheckbox = FreezeCheckbox.Checked,
                CheckBoxDoLogs = checkBoxDoLogs.Checked,
                CheckBoxAdminLog = checkBoxAdminLog.Checked,
                CheckBoxNetLog = checkBoxNetLog.Checked,
                CheckBoxAllowExtraParams = checkBoxAllowExtraParams.Checked,
                TextBoxMods = textBoxMods.Text,
                TimerEnabled = checkBoxEnableTimer.Checked,
                Hours = numericUpDownHours.Value,
                Minutes = numericUpDownMinutes.Value,
                Seconds = numericUpDownSeconds.Value,
                WebhookURL = webhookTextBox.Text,
                WebhookCheckbox = EnableWebhookCheckbox.Checked,
                StartWebhook = StartWebhookCheckbox.Checked,
                RestartWebhook = RestartWebhookCheckbox.Checked,
                textBoxServerMods = textBoxServerMods.Text,
                profile = profileTextBox.Text,
                BEPath = textboxBePath.Text,
                DontShowAgain = DontShowAgainCheckbox.Checked,
                // ON START
                SavedEmbedOnStartDisplayTime = OnStartDisplayTime.Checked,
                SavedEmbedOnStartColor = OnStartEmbedColor.Text,
                SavedEmbedOnStartBotName = OnStartBotName.Text,
                SavedEmbedOnStartBotAvatar = OnStartBotAvatar.Text,
                SavedEmbedOnStartMessagePrior = OnStartMessagePrior.Text,
                SavedEmbedOnStartServerName = OnStartServerName.Text,
                SavedEmbedOnStartServerIcon = OnStartServerIcon.Text,
                SavedEmbedOnStartServerNameURL = OnStartServerNameURL.Text,
                SavedEmbedOnStartTitle = OnStartTitle.Text,
                SavedEmbedOnStartTitleURL = OnStartTitleURL.Text,
                SavedEmbedOnStartBottomTitle = OnStartBottomTitle.Text,
                SavedEmbedOnStartBottomText = OnStartBottomText.Text,
                SavedEmbedOnStartBottomTextURL = OnStartBottomTextURL.Text,
                SavedEmbedOnStartContentImage = OnStartContentImage.Text,
                SavedEmbedOnStartBigLogo = OnStartBigLogo.Text,
                // ON RESTART
                SavedEmbedOnRestartDisplayTime = OnRestartDisplayTime.Checked,
                SavedEmbedOnRestartColor = OnRestartEmbedColor.Text,
                SavedEmbedOnRestartBotName = OnRestartBotName.Text,
                SavedEmbedOnRestartBotAvatar = OnRestartBotAvatar.Text,
                SavedEmbedOnRestartMessagePrior = OnRestartMessagePrior.Text,
                SavedEmbedOnRestartServerName = OnRestartServerName.Text,
                SavedEmbedOnRestartServerIcon = OnRestartServerIcon.Text,
                SavedEmbedOnRestartServerNameURL = OnRestartServerNameURL.Text,
                SavedEmbedOnRestartTitle = OnRestartTitle.Text,
                SavedEmbedOnRestartTitleURL = OnRestartTitleURL.Text,
                SavedEmbedOnRestartBottomTitle = OnRestartBottomTitle.Text,
                SavedEmbedOnRestartBottomText = OnRestartBottomText.Text,
                SavedEmbedOnRestartBottomTextURL = OnRestartBottomTextURL.Text,
                SavedEmbedOnRestartContentImage = OnRestartContentImage.Text,
                SavedEmbedOnRestartBigLogo = OnRestartBigLogo.Text,

                //  RCON CONFIGURATION   RCON CONFIGURATION   RCON CONFIGURATION
                EnableRconConfiguration = RconEnableCheckBox.Checked,
                RconAniDupe = AntiDupeCheckbox.Checked,
                RestrictRconSelectedIndex = RestrictRcoComboBox.SelectedIndex,
                RconPassword = RconPasswordTextBox.Text,
                RconPort = RconPortNumeric.Text,
                PortTest = RconPortNumeric.Value,

                // Max Ping Allowance

                MaxPing = MaxPingNumericUpDown.Value,
            };

            // Serialize the profile data to JSON and save it to the file
            try
            {
                string json = JsonConvert.SerializeObject(profileData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                // Refresh the profile list after saving
                LoadProfilesFromDirectory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving: " + ex.Message);
            }
        }
        private void loadFileToolStripMenu_Click(object sender, EventArgs e)
        {
            if (comboBoxProfiles.SelectedItem != null)
            {
                string selectedFileName = comboBoxProfiles.SelectedItem.ToString();
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(directoryPath, selectedFileName + ".json");

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var profileData = JsonConvert.DeserializeObject<dynamic>(json);

                    // Populate the form fields with loaded data, using null-coalescing and explicit conversions
                    textBoxExePath.Text = profileData?.ExecutablePath ?? string.Empty;
                    textBoxParameters.Text = profileData?.Parameters ?? string.Empty;
                    textBoxPort.Text = profileData?.Port ?? string.Empty;
                    textBoxConfig.Text = profileData?.Config ?? string.Empty;
                    modDir.Text = profileData?.ModsDir ?? string.Empty;
                    comboBoxCpu.SelectedItem = profileData?.Cpu ?? string.Empty;
                    profileTextBox.Text = profileData?.profile ?? string.Empty;

                    // Safely load checkbox values (use 'true' or 'false' when null)
                    FreezeCheckbox.Checked = (bool?)profileData?.FreezeCheckbox ?? false;
                    checkBoxDoLogs.Checked = (bool?)profileData?.CheckBoxDoLogs ?? false;
                    checkBoxAdminLog.Checked = (bool?)profileData?.CheckBoxAdminLog ?? false;
                    checkBoxNetLog.Checked = (bool?)profileData?.CheckBoxNetLog ?? false;
                    checkBoxAllowExtraParams.Checked = (bool?)profileData?.CheckBoxAllowExtraParams ?? false;

                    // Load mods into textBoxMods
                    textBoxMods.Text = profileData?.TextBoxMods ?? string.Empty;

                    // Load Servermods into textBoxServerMods
                    textBoxServerMods.Text = profileData?.textBoxServerMods ?? string.Empty;

                    // Load timer settings
                    checkBoxEnableTimer.Checked = (bool?)profileData?.TimerEnabled ?? false;
                    numericUpDownHours.Value = (decimal)(profileData?.Hours ?? 0);
                    numericUpDownMinutes.Value = (decimal)(profileData?.Minutes ?? 0);
                    numericUpDownSeconds.Value = (decimal)(profileData?.Seconds ?? 0);

                    //Bepath
                    textboxBePath.Text = profileData?.BEPath ?? string.Empty;


                    // Clear and load ModsCheckedListBox with directories from modDir
                    ModsCheckedListBox.Items.Clear();
                    ServerModsCheckedListBox.Items.Clear();

                    //Webhook
                    webhookTextBox.Text = profileData?.WebhookURL ?? string.Empty;
                    EnableWebhookCheckbox.Checked = (bool?)profileData?.WebhookCheckbox ?? false;
                    StartWebhookCheckbox.Checked = (bool?)profileData?.StartWebhook ?? false;
                    RestartWebhookCheckbox.Checked = (bool?)profileData?.RestartWebhook ?? false;


                    // ON START

                    OnStartDisplayTime.Checked = (bool?)profileData?.SavedEmbedOnStartDisplayTime ?? false;
                    OnStartEmbedColor.Text = profileData?.SavedEmbedOnStartColor ?? string.Empty;
                    OnStartBotName.Text = profileData?.SavedEmbedOnStartBotName ?? string.Empty;
                    OnStartBotAvatar.Text = profileData?.SavedEmbedOnStartBotAvatar ?? string.Empty;
                    OnStartMessagePrior.Text = profileData?.SavedEmbedOnStartMessagePrior ?? string.Empty;
                    OnStartServerName.Text = profileData?.SavedEmbedOnStartServerName ?? string.Empty;
                    OnStartServerIcon.Text = profileData?.SavedEmbedOnStartServerIcon ?? string.Empty;
                    OnStartServerNameURL.Text = profileData?.SavedEmbedOnStartServerNameURL ?? string.Empty;
                    OnStartTitle.Text = profileData?.SavedEmbedOnStartTitle ?? string.Empty;
                    OnStartTitleURL.Text = profileData?.SavedEmbedOnStartTitleURL ?? string.Empty;
                    OnStartBottomTitle.Text = profileData?.SavedEmbedOnStartBottomTitle ?? string.Empty;
                    OnStartBottomText.Text = profileData?.SavedEmbedOnStartBottomText ?? string.Empty;
                    OnStartBottomTextURL.Text = profileData?.SavedEmbedOnStartBottomTextURL ?? string.Empty;
                    OnStartContentImage.Text = profileData?.SavedEmbedOnStartContentImage ?? string.Empty;
                    OnStartBigLogo.Text = profileData?.SavedEmbedOnStartBigLogo ?? string.Empty;

                    // ON RESTART

                    OnRestartDisplayTime.Checked = (bool?)profileData?.SavedEmbedOnRestartDisplayTime ?? false;
                    OnRestartEmbedColor.Text = profileData?.SavedEmbedOnRestartColor ?? string.Empty;
                    OnRestartBotName.Text = profileData?.SavedEmbedOnRestartBotName ?? string.Empty;
                    OnRestartBotAvatar.Text = profileData?.SavedEmbedOnRestartBotAvatar ?? string.Empty;
                    OnRestartMessagePrior.Text = profileData?.SavedEmbedOnRestartMessagePrior ?? string.Empty;
                    OnRestartServerName.Text = profileData?.SavedEmbedOnRestartServerName ?? string.Empty;
                    OnRestartServerIcon.Text = profileData?.SavedEmbedOnRestartServerIcon ?? string.Empty;
                    OnRestartServerNameURL.Text = profileData?.SavedEmbedOnRestartServerNameURL ?? string.Empty;
                    OnRestartTitle.Text = profileData?.SavedEmbedOnRestartTitle ?? string.Empty;
                    OnRestartTitleURL.Text = profileData?.SavedEmbedOnRestartTitleURL ?? string.Empty;
                    OnRestartBottomTitle.Text = profileData?.SavedEmbedOnRestartBottomTitle ?? string.Empty;
                    OnRestartBottomText.Text = profileData?.SavedEmbedOnRestartBottomText ?? string.Empty;
                    OnRestartBottomTextURL.Text = profileData?.SavedEmbedOnRestartBottomTextURL ?? string.Empty;
                    OnRestartContentImage.Text = profileData?.SavedEmbedOnRestartContentImage ?? string.Empty;
                    OnRestartBigLogo.Text = profileData?.SavedEmbedOnRestartBigLogo ?? string.Empty;

                    // RCON CONFIGURATION

                    RconEnableCheckBox.Checked = (bool?)profileData?.EnableRconConfiguration ?? false;
                    AntiDupeCheckbox.Checked = (bool?)profileData?.RconAniDupe ?? false;
                    RestrictRcoComboBox.SelectedIndex = profileData?.RestrictRconSelectedIndex ?? 1;
                    RconPasswordTextBox.Text = profileData?.RconPassword ?? string.Empty;
                    RconPortNumeric.Value = (decimal)(profileData?.PortTest ?? 0);

                    // Update Configuration

                    DontShowAgainCheckbox.Checked = (bool?)profileData?.DontShowAgain ?? false;

                    // Max Ping Allowance
                    MaxPingNumericUpDown.Value = (decimal)(profileData?.MaxPing ?? 200);


                    string modsDirectory = modDir.Text;
                    if (Directory.Exists(modsDirectory))
                    {
                        // Get all directories in modDir
                        string[] directories = Directory.GetDirectories(modsDirectory);

                        // Populate ModsCheckedListBox with folder names except the one to exclude
                        foreach (string directory in directories)
                        {
                            string folderName = Path.GetFileName(directory);
                            if (folderName != "!DO_NOT_CHANGE_FILES_IN_THESE_FOLDERS")
                            {
                                ModsCheckedListBox.Items.Add(folderName);

                                // Populate ServerModsCheckedListBox only if the folder starts with "@"
                                if (folderName.StartsWith("@"))
                                {
                                    ServerModsCheckedListBox.Items.Add(folderName);
                                }
                            }
                        }

                        // Check the items in ModsCheckedListBox based on the mods in textBoxMods
                        CheckModsInList(ModsCheckedListBox, textBoxMods.Text);
                        CheckModsInList(ServerModsCheckedListBox, textBoxServerMods.Text);
                        RconPasswordTextBox_TextChanged(null, null);
                        UpdateButtonsState();
                        ToggleTimerFields();
                        ToggleWebhookField();
                        rconEnableCheckBox_CheckedChanged(null, null);
                        SaveLatestProfile(selectedFileName);
                    }
                    else
                    {
                        MessageBox.Show("The mods directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selected profile file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No profile selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ToggleTextBoxParameters();
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            // Clear all fields to start a new profile
            textBoxExePath.Clear();
            textBoxParameters.Clear();
            textBoxPort.Clear();
            textBoxConfig.Clear();
            comboBoxCpu.SelectedIndex = 3; // Reset to "Automatic"
            FreezeCheckbox.Checked = false; // Reset Freezecheck
            comboBoxProfiles.Items.Clear(); // Clear the profile list for new profile
            LoadProfilesFromDirectory();
            comboBoxProfiles.SelectedIndex = -1; // Clear the selection
            textBoxMods.Clear();
            textBoxServerMods.Clear();
            modDir.Clear();
            profileTextBox.Clear();

            // Clear all entries from the CheckedListBox&ServerListBox
            ModsCheckedListBox.Items.Clear();
            ServerModsCheckedListBox.Items.Clear();
            UpdateButtonsState();

            // Uncheck all items in the ModsCheckedListBox
            for (int i = 0; i < ModsCheckedListBox.Items.Count; i++)
            {
                ModsCheckedListBox.SetItemChecked(i, false);
            }

            // Uncheck all items in the Server ModsCheckedListBox

            for (int i = 0; i < ServerModsCheckedListBox.Items.Count; i++)
            {
                ServerModsCheckedListBox.SetItemChecked(i, false);
            }

            checkBoxDoLogs.Checked = false;
            checkBoxAdminLog.Checked = false;
            checkBoxNetLog.Checked = false;
            checkBoxAllowExtraParams.Checked = false;
            textBoxParameters.Clear();

            // Timer Clear
            checkBoxEnableTimer.Checked = false;
            numericUpDownSeconds.Value = 0;
            numericUpDownMinutes.Value = 0;
            numericUpDownHours.Value = 0;

            //Webhook
            EnableWebhookCheckbox.Checked = false;
            webhookTextBox.Clear();
            StartWebhookCheckbox.Checked = false;
            RestartWebhookCheckbox.Checked = false;

            //BE
            BePathCheckbox.Checked = false;
            textboxBePath.Clear();

            //WEBHOOK
            OnStartDisplayTime.Checked = false;
            OnStartEmbedColor.Clear();
            OnStartBotName.Clear();
            OnStartBotAvatar.Clear();
            OnStartMessagePrior.Clear();
            OnStartServerName.Clear();
            OnStartServerIcon.Clear();
            OnStartServerNameURL.Clear();
            OnStartTitle.Clear();
            OnStartTitleURL.Clear();
            OnStartBottomTitle.Clear();
            OnStartBottomText.Clear();
            OnStartBottomTextURL.Clear();
            OnStartContentImage.Clear();
            OnStartBigLogo.Clear();
            OnRestartDisplayTime.Checked = false;
            OnRestartEmbedColor.Clear();
            OnRestartBotName.Clear();
            OnRestartBotAvatar.Clear();
            OnRestartMessagePrior.Clear();
            OnRestartServerName.Clear();
            OnRestartServerIcon.Clear();
            OnRestartServerNameURL.Clear();
            OnRestartTitle.Clear();
            OnRestartTitleURL.Clear();
            OnRestartBottomTitle.Clear();
            OnRestartBottomText.Clear();
            OnRestartBottomTextURL.Clear();
            OnRestartContentImage.Clear();
            OnRestartBigLogo.Clear();

            //RCON

            AntiDupeCheckbox.Checked = false;
            RestrictRcoComboBox.SelectedIndex = 1;
            RconEnableCheckBox.Checked = false;
            RconPasswordTextBox.Clear();
            RconPortNumeric.Value = 0;
            DontShowAgainCheckbox.Checked = false;
            MaxPingNumericUpDown.Value = 500;
        }


        /// End of Profile Region      End of Profile Region      End of Profile Region      



        private void InitializeCpuDropdown()
        {
            comboBoxCpu.Items.Clear();

            // Get the number of logical processors (physical or virtual)
            int cpuCount = Environment.ProcessorCount;

            // Populate the dropdown with CPU numbers from 1 up to the available CPU count
            for (int i = 1; i <= cpuCount; i++)
            {
                comboBoxCpu.Items.Add(i.ToString());
            }

            // Set default selection to either 4 cores (if available) or the maximum available
            comboBoxCpu.SelectedIndex = (cpuCount >= 4) ? 3 : cpuCount - 1;
        }

        private void LoadProfilesFromDirectory()
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory; // Get the directory of the executable
            var jsonFiles = Directory.GetFiles(directoryPath, "*.json")
                                     .Where(file =>
                                         !file.EndsWith("LatestProfile.json") &&
                                         !file.EndsWith("DayZ Server Tool.deps.json") &&
                                         !file.EndsWith("DayZ Server Tool.runtimeconfig.json"));

            comboBoxProfiles.Items.Clear(); // Clear existing items

            foreach (var file in jsonFiles)
            {
                comboBoxProfiles.Items.Add(Path.GetFileNameWithoutExtension(file)); // Add only the file name without extension
            }

            if (comboBoxProfiles.Items.Count > 0)
                comboBoxProfiles.SelectedIndex = 0; // Select the first item if any profiles are found
        }

        private void refreshProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProfilesFromDirectory(); // Reload profiles when refresh is clicked
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                // Set the flag to prevent auto-restart
                isManualStop = true;

                // Get all instances of DayZServer_x64
                var runningProcesses = Process.GetProcessesByName("DayZServer_x64");
                // Stop the restart timer
                restartTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Disable the timer

                // Update the label to show that the timer has been stopped
                timeremaininglabel.Text = "Timer stopped manually.";

                // Terminate each instance found
                foreach (var process in runningProcesses)
                {
                    process.Kill(); // Stop the process
                    //process.WaitForExit(); // Optionally wait for the process to exit
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during the process termination
                MessageBox.Show($"Error stopping the executable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void buttonBrowseExe_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable files (*.exe)|*.exe|Jason's Test Scripts (*.py)|*.py|All files (*.*)|*.*";
                openFileDialog.Title = "Select Executable";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    executablePath = openFileDialog.FileName;
                    textBoxExePath.Text = executablePath; // Update the TextBox with the selected path
                }
            }
        }

        private void comboBoxProfiles_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBoxProfiles.SelectedItem != null)
            {
                string selectedFileName = comboBoxProfiles.SelectedItem.ToString();
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(directoryPath, selectedFileName);

                // Load the selected profile into the form fields
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var profileData = JsonConvert.DeserializeObject<dynamic>(json);

                    textBoxExePath.Text = profileData.ExecutablePath;
                    textBoxParameters.Text = profileData.Parameters;
                    textBoxPort.Text = profileData.Port;
                    textBoxConfig.Text = profileData.Config;
                    comboBoxCpu.SelectedItem = profileData.Cpu;
                    RestrictRcoComboBox.SelectedItem = profileData.RestrictRcon;
                }
            }
        }

        private void DeleteProfile_Click(object sender, EventArgs e)
        {
            // Ensure that a profile is selected before attempting to delete
            if (comboBoxProfiles.SelectedItem != null)
            {
                string selectedProfile = comboBoxProfiles.SelectedItem.ToString();
                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = Path.Combine(directoryPath, selectedProfile + ".json");

                // Confirm deletion with the user
                var confirmationResult = MessageBox.Show(
                    $"Are you sure you want to delete the profile '{selectedProfile}'?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmationResult == DialogResult.Yes)
                {
                    try
                    {
                        // Check if the file exists and delete it
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            MessageBox.Show("Profile deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Profile file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Refresh the profile list after deletion
                        LoadProfilesFromDirectory();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting the profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No profile selected for deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ToggleTextBoxParameters()
        {
            // Enable or disable textBoxParameters based on checkBoxAllowExtraParams state
            textBoxParameters.Enabled = checkBoxAllowExtraParams.Checked;
        }
        private void CheckBoxAllowExtraParams_CheckedChanged(object sender, EventArgs e)
        {
            ToggleTextBoxParameters();
        }

        private void buttonUpdateMods_Click(object sender, EventArgs e)
        {
            // Create a string to hold the selected folder names
            string selectedMods = "";

            // Iterate through CheckedListBox items
            foreach (var item in ModsCheckedListBox.CheckedItems)
            {
                // Append the selected items to the string with a semicolon
                selectedMods += item.ToString() + ";";
            }

            // Remove the trailing semicolon if it exists
            if (selectedMods.EndsWith(";"))
            {
                selectedMods = selectedMods.Substring(0, selectedMods.Length - 1);
            }

            // Set the textBoxMods text to the concatenated folder names
            textBoxMods.Text = selectedMods;
        }
        private void buttonUpdateServerMods_Click(object sender, EventArgs e)
        {
            // Create a string to hold the selected folder names
            string selectedSMods = "";

            // Iterate through CheckedListBox items
            foreach (var item in ServerModsCheckedListBox.CheckedItems)
            {
                // Append the selected items to the string with a semicolon
                selectedSMods += item.ToString() + ";";
            }

            // Remove the trailing semicolon if it exists
            if (selectedSMods.EndsWith(";"))
            {
                selectedSMods = selectedSMods.Substring(0, selectedSMods.Length - 1);
            }

            // Set the textBoxMods text to the concatenated folder names
            textBoxServerMods.Text = selectedSMods;
        }

        private void discordServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Link to the Discord server
            string discordLink = "https://discord.com/invite/bvgNUJhg5K";

            try
            {
                // Open the Discord link in the default web browser
                Process.Start(new ProcessStartInfo(discordLink) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., if no browser is installed)
                MessageBox.Show("An error occurred while trying to open the Discord link: " + ex.Message);
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Link to the Discord server
            string discordLink = "https://github.com/DaBoiJason/DayZServerTool";

            try
            {
                // Open the Discord link in the default web browser
                Process.Start(new ProcessStartInfo(discordLink) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., if no browser is installed)
                MessageBox.Show("An error occurred while trying to open the Discord link: " + ex.Message);
            }
        }

        private void refreshProfilesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            LoadProfilesFromDirectory();
        }
        private async void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if DayZServer_x64.exe is already running
                var runningProcesses = Process.GetProcessesByName("DayZServer_x64");
                if (runningProcesses.Length > 0)
                {
                    MessageBox.Show("Warning: DayZServer_x64.exe is already running.", "Process Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method to prevent starting a new instance
                }

                // Retrieve the path to the executable from textBoxExePath
                string exePath = textBoxExePath.Text;

                // Retrieve port, cpuCount, and config values
                string port = textBoxPort.Text;
                string cpuCount = comboBoxCpu.SelectedItem?.ToString() ?? "4"; // Default to 4 if not set
                string config = textBoxConfig.Text;
                string profile = profileTextBox.Text;
                string bepath = textboxBePath.Text;

                // Retrieve mods and server mods from textBox
                string mods = textBoxMods.Text; // Get the mod string directly
                string serverMods = textBoxServerMods.Text; // Get the server mod string directly

                // Construct the command line arguments
                List<string> arguments = new List<string>
                {
                    $"-port={port}",
                    $"-cpuCount={cpuCount}",
                    $"-config={config}",
                    $"-profiles={profile}",
                    $"\"-mod={mods}\"",
                    $"\"-servermod={serverMods}\"",
                };
                if (BePathCheckbox.Checked)
                {
                    arguments.Add($"-BEpath={bepath}");
                }
                ;
                // Add optional parameters based on checkbox states
                if (checkBoxDoLogs.Checked) arguments.Add("-doLogs");
                if (checkBoxAdminLog.Checked) arguments.Add("-adminlog");
                if (checkBoxNetLog.Checked) arguments.Add("-netlog");
                if (FreezeCheckbox.Checked) arguments.Add("-freezecheck");

                // Start the process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = string.Join(" ", arguments), // Join the arguments into a single string with spaces
                    UseShellExecute = true // Use the shell to start the process
                };
                Process.Start(startInfo);
                await Task.Delay(3000);
                if (EnableWebhookCheckbox.Checked)
                {
                    WebhookStartMessageSend();
                }
                await Task.Delay(3000);
                if (checkBoxEnableTimer.Checked)
                {
                    StartRestartTimer();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, invalid path)
                MessageBox.Show($"Error starting the executable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void WebhookStartMessageSend()
        {
            // Retrieve inputs from UI elements
            string botName = OnStartBotName.Text;
            string botAvatar = OnStartBotAvatar.Text;
            string messageContent = OnStartMessagePrior.Text;
            string serverName = OnStartServerName.Text;
            string serverIcon = OnStartServerIcon.Text;
            string serverNameURL = OnStartServerNameURL.Text;
            string embedTitle = OnStartTitle.Text;
            string embedURL = OnStartTitleURL.Text;
            string embedBottomTitle = OnStartBottomTitle.Text;
            string embedBottomText = OnStartBottomText.Text;
            string embedBottomTextURL = OnStartBottomTextURL.Text;
            string contentImage = OnStartContentImage.Text;
            string bigLogo = OnStartBigLogo.Text;

            // Default color (red) if no color input
            int embedColor = 0xFF0000;
            if (!string.IsNullOrEmpty(OnStartEmbedColor.Text))
            {
                if (!int.TryParse(OnStartEmbedColor.Text.Trim(), System.Globalization.NumberStyles.HexNumber, null, out embedColor))
                {
                    MessageBox.Show("Invalid color format. Please provide a valid hex color.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Get the webhook URL from the text box
            string webhookUrl = webhookTextBox.Text.Trim();
            if (string.IsNullOrEmpty(webhookUrl) || !Uri.IsWellFormedUriString(webhookUrl, UriKind.Absolute))
            {
                MessageBox.Show("Invalid webhook URL. Please provide a valid URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Dynamically build the fields for the embed
            var fields = new List<object>
    {
        new
        {
            name = "Status",
            value = "Server Is Starting",
            inline = true
        }
    };

            // Add the server time field if the checkbox is checked
            if (OnStartDisplayTime.Checked)
            {
                fields.Add(new
                {
                    name = "Server Time",
                    value = DateTime.Now.ToString("HH:mm:ss"),
                    inline = true
                });
            }

            // Add the bottom section if necessary
            fields.Add(new
            {
                name = embedBottomTitle,
                value = string.IsNullOrEmpty(embedBottomText) ? "No additional info" : $"[{embedBottomText}]({embedBottomTextURL})",
                inline = false
            });

            // Construct the full payload
            var payload = new
            {
                username = botName,
                avatar_url = botAvatar,
                content = messageContent,
                embeds = new[]
                {
            new
            {
                title = embedTitle,
                description = "The DayZ server is starting.",
                url = embedURL,
                color = embedColor,
                author = new
                {
                    name = serverName,
                    icon_url = serverIcon,
                    url = serverNameURL
                },
                image = new
                {
                    url = contentImage
                },
                thumbnail = new
                {
                    url = bigLogo
                },
                fields = fields.ToArray(),
                footer = new
                {
                    text = "Powered By | DzST | DayZ Server Tool by DaBoiJason",
                    icon_url = "https://media.discordapp.net/attachments/1290605906689527828/1290605948049555486/images.png"
                },
                timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format for timestamp
            }
        }
            };

            // Serialize the payload to JSON
            string jsonPayload = JsonConvert.SerializeObject(payload);

            // Debugging: Log the payload for inspection
            Console.WriteLine("Payload: " + jsonPayload);

            // Send the webhook request using HttpClient
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, content);

                    // Check the response status
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error sending webhook: {response.StatusCode} - {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Webhook sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void buttonBrowseMods_Click(object sender, EventArgs e)
        {
            // Open a folder browser dialog
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select Mods Directory";

                // Show the dialog and check if the user clicked OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Clear previous items in both CheckedListBoxes
                    ModsCheckedListBox.Items.Clear();
                    ServerModsCheckedListBox.Items.Clear(); // Clear the new CheckedListBox

                    // Get all directories in the selected path
                    string[] directories = Directory.GetDirectories(folderBrowserDialog.SelectedPath);

                    // Populate both CheckedListBoxes with folder names
                    foreach (string directory in directories)
                    {
                        // Extract the folder name
                        string folderName = Path.GetFileName(directory);

                        // Exclude certain folders (like "!DO_NOT_CHANGE_FILES_IN_THESE_FOLDERS")
                        if (folderName != "!DO_NOT_CHANGE_FILES_IN_THESE_FOLDERS")
                        {
                            // Add to ModsCheckedListBox
                            ModsCheckedListBox.Items.Add(folderName);

                            // Add to ServerModsCheckedListBox only if it starts with "@"
                            if (folderName.StartsWith("@"))
                            {
                                ServerModsCheckedListBox.Items.Add(folderName);
                            }
                        }
                    }

                    // Set the selected path to the modDir textbox
                    modDir.Text = folderBrowserDialog.SelectedPath;

                    // Update button states based on the CheckedListBox contents
                    UpdateButtonsState();
                }
            }
        }

        private void UpdateButtonsState()
        {
            // Check if there are any items in the ModsCheckedListBox
            bool hasItems = ModsCheckedListBox.Items.Count > 0;

            // Enable or disable the buttons based on the presence of items
            ModKeys.Enabled = hasItems; // Assuming your button is named keysButton
            Mods.Enabled = hasItems;  // Assuming your button is named modsButton
            ServerKeys.Enabled = hasItems; // Assuming your button is named keysButton
            ServerMods.Enabled = hasItems;  // Assuming your button is named modsButton
        }

        // Optional: You may also want to call UpdateButtonsState when the CheckedListBox is cleared
        private void ModsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Optionally call UpdateButtonsState here if you allow items to be checked/unchecked
            UpdateButtonsState();
        }

        private void Keys_Click(object sender, EventArgs e)
        {
            // Ensure modDir and textBoxExePath are not empty
            if (string.IsNullOrEmpty(modDir.Text) || string.IsNullOrEmpty(textBoxExePath.Text))
            {
                MessageBox.Show("Please make sure modDir and textBoxExePath fields are populated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string modDirectory = modDir.Text;
            string destinationKeysPath = Path.Combine(Path.GetDirectoryName(textBoxExePath.Text), "keys");

            // Create the destination keys folder if it doesn't exist
            if (!Directory.Exists(destinationKeysPath))
            {
                Directory.CreateDirectory(destinationKeysPath);
            }

            // Collect selected mods from ModsCheckedListBox
            List<string> selectedMods = new List<string>();

            // Add checked items from ModsCheckedListBox
            foreach (object item in ModsCheckedListBox.CheckedItems)
            {
                selectedMods.Add(item.ToString());
            }

            // Add mods from textBoxMods, split by ';'
            if (!string.IsNullOrEmpty(textBoxMods.Text))
            {
                string[] modsFromTextBox = textBoxMods.Text.Split(';').Select(m => m.Trim()).ToArray();
                selectedMods.AddRange(modsFromTextBox);
            }

            // Remove duplicates in case a mod is in both places
            selectedMods = selectedMods.Distinct().ToList();

            // Initialize the progress bar
            progressBar.Value = 0;
            progressBar.Maximum = selectedMods.Count;

            // Iterate through each selected mod
            foreach (string mod in selectedMods)
            {
                string modPath = Path.Combine(modDirectory, mod);

                if (Directory.Exists(modPath))
                {
                    // Look for Keys/keys/key/Key folders within the mod directory
                    string[] keyFolders = Directory.GetDirectories(modPath, "*", SearchOption.TopDirectoryOnly)
                                                    .Where(dir => dir.EndsWith("Keys", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("keys", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("key", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("Key", StringComparison.OrdinalIgnoreCase))
                                                    .ToArray();

                    foreach (string keyFolder in keyFolders)
                    {
                        try
                        {
                            // Copy the contents of the keyFolder to the destinationKeysPath
                            CopyDirectoryContents(keyFolder, destinationKeysPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error copying files from {keyFolder}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Mod directory '{mod}' not found in '{modDirectory}'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Update progress bar
                progressBar.Value += 1;
            }

            MessageBox.Show("Key files copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Assuming your TabControl is named tabControl1+3
        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage11) // Only handle if tabPage11 is selected
            {
                AdjustLayoutBasedOnTabControl3();
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3)
            {

                this.Size = new Size(1214, 603); // Window size
                tabControl1.Size = new Size(1178, 519); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage4)
            {

                this.Size = new Size(629, 191); // Window size
                tabControl1.Size = new Size(592, 108); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage5)
            {

                this.Size = new Size(629, 260); // Window size
                tabControl1.Size = new Size(592, 174); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage1)
            {

                this.Size = new Size(881, 362); // Window size
                tabControl1.Size = new Size(841, 282); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {

                this.Size = new Size(1263, 470); // Window size
                tabControl1.Size = new Size(1222, 384); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage7)
            {

                this.Size = new Size(635, 708); // Window size
                tabControl1.Size = new Size(596, 632); // Tab size
            }
            else if (tabControl1.SelectedTab == tabPage11 && tabControl3.SelectedTab == tabPage13)
            {
                AdjustLayoutBasedOnTabControl3();
            }
            else if (tabControl1.SelectedTab == tabPage11 && tabControl3.SelectedTab == tabPage14)
            {
                AdjustLayoutBasedOnTabControl3();
            }
            else if (tabControl1.SelectedTab == tabPage11 && tabControl3.SelectedTab == tabPage15)
            {
                AdjustLayoutBasedOnTabControl3();
            }
            else if (tabControl1.SelectedTab == tabPage12)
            {

                this.Size = new Size(635, 708); // Window size
                tabControl1.Size = new Size(596, 632); // Tab size
            }
            else  // NORMAL  ====================   NORMAL   ==============
            {
                // When any other tab is selected, revert to the normal size
                this.Size = new Size(629, 603); // Window size
                tabControl1.Size = new Size(592, 519); // Normal tab size
            }
        }
        private void AdjustLayoutBasedOnTabControl3()
        {
            if (tabControl3.SelectedTab == tabPage13)
            {
                this.Size = new Size(717, 430);
                tabControl3.Size = new Size(671, 317);
                tabControl1.Size = new Size(683, 355);
                PopulateRconConfigFileComboBox();
            }
            else if (tabControl3.SelectedTab == tabPage14)
            {
                this.Size = new Size(921, 767);
                tabControl3.Size = new Size(871, 655);
                tabControl1.Size = new Size(889, 693);
            }
            else if (tabControl3.SelectedTab == tabPage15)
            {
                this.Size = new Size(1184, 924);
                tabControl3.Size = new Size(1138, 811);
                tabControl1.Size = new Size(1151, 849);
            }
        }
        /*
            When stretched
            tabPage3 Size
            1178, 519
            Window Size
            1214, 603

            ===================
            ===================
            ===================

            when normal
            tabPageX size
            592; 519
            window size
            629; 603
         */
        private void ServerKeys_Click(object sender, EventArgs e)
        {
            // Ensure modDir and textBoxExePath are not empty
            if (string.IsNullOrEmpty(modDir.Text) || string.IsNullOrEmpty(textBoxExePath.Text))
            {
                MessageBox.Show("Please make sure modDir and textBoxExePath fields are populated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string modDirectory = modDir.Text;
            string destinationKeysPath = Path.Combine(Path.GetDirectoryName(textBoxExePath.Text), "keys");

            // Create the destination keys folder if it doesn't exist
            if (!Directory.Exists(destinationKeysPath))
            {
                Directory.CreateDirectory(destinationKeysPath);
            }

            // Collect selected mods from ModsCheckedListBox
            List<string> selectedMods = new List<string>();

            // Add checked items from ModsCheckedListBox
            foreach (object item in ServerModsCheckedListBox.CheckedItems)
            {
                selectedMods.Add(item.ToString());
            }

            // Add mods from textBoxMods, split by ';'
            if (!string.IsNullOrEmpty(textBoxServerMods.Text))
            {
                string[] modsFromTextBox = textBoxServerMods.Text.Split(';').Select(m => m.Trim()).ToArray();
                selectedMods.AddRange(modsFromTextBox);
            }

            // Remove duplicates in case a mod is in both places
            selectedMods = selectedMods.Distinct().ToList();

            // Initialize the progress bar
            progressBar2.Value = 0;
            progressBar2.Maximum = selectedMods.Count;

            // Iterate through each selected mod
            foreach (string mod in selectedMods)
            {
                string modPath = Path.Combine(modDirectory, mod);

                if (Directory.Exists(modPath))
                {
                    // Look for Keys/keys/key/Key folders within the mod directory
                    string[] keyFolders = Directory.GetDirectories(modPath, "*", SearchOption.TopDirectoryOnly)
                                                    .Where(dir => dir.EndsWith("Keys", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("keys", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("key", StringComparison.OrdinalIgnoreCase) ||
                                                                  dir.EndsWith("Key", StringComparison.OrdinalIgnoreCase))
                                                    .ToArray();

                    foreach (string keyFolder in keyFolders)
                    {
                        try
                        {
                            // Copy the contents of the keyFolder to the destinationKeysPath
                            CopyDirectoryContents(keyFolder, destinationKeysPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error copying files from {keyFolder}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Mod directory '{mod}' not found in '{modDirectory}'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Update progress bar
                progressBar2.Value += 1;
            }

            MessageBox.Show("Key files copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CopyDirectoryContents(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            // Copy each file into the destination directory
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // Recursively copy subdirectories (if needed)
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destinationDir, subdir.Name);
                CopyDirectoryContents(subdir.FullName, tempPath);
            }
        }
        private void ServerMods_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(modDir.Text) || string.IsNullOrEmpty(textBoxExePath.Text))
            {
                MessageBox.Show("Please make sure modDir and textBoxExePath fields are populated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string destinationDir = Path.GetDirectoryName(textBoxExePath.Text);
            if (!Directory.Exists(destinationDir))
            {
                MessageBox.Show($"Destination directory '{destinationDir}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> selectedServerMods = new List<string>();
            foreach (object item in ServerModsCheckedListBox.CheckedItems)
            {
                selectedServerMods.Add(item.ToString());
            }

            progressBar3.Value = 0;
            progressBar3.Maximum = selectedServerMods.Count;

            foreach (string mod in selectedServerMods)
            {
                string sourceModPath = Path.Combine(modDir.Text, mod);
                string destinationModPath = Path.Combine(destinationDir, mod);

                if (Directory.Exists(sourceModPath))
                {
                    try
                    {
                        CopyDirectory(sourceModPath, destinationModPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying server mod '{mod}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                progressBar3.Value += 1;
            }

            MessageBox.Show("Server mods copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Mods_Click(object sender, EventArgs e)
        {
            // Ensure modDir and textBoxExePath are not empty
            if (string.IsNullOrEmpty(modDir.Text) || string.IsNullOrEmpty(textBoxExePath.Text))
            {
                MessageBox.Show("Please make sure modDir and textBoxExePath fields are populated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Remove \DayZServer_x64.exe from the path in textBoxExePath to get the destination directory
            string destinationDir = Path.GetDirectoryName(textBoxExePath.Text);
            if (!Directory.Exists(destinationDir))
            {
                MessageBox.Show($"Destination directory '{destinationDir}' does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Collect selected mods from ModsCheckedListBox and textBoxMods
            List<string> selectedMods = new List<string>();

            // Add checked items from ModsCheckedListBox
            foreach (object item in ModsCheckedListBox.CheckedItems)
            {
                selectedMods.Add(item.ToString());
            }

            // Add mods from textBoxMods, split by ';'
            if (!string.IsNullOrEmpty(textBoxMods.Text))
            {
                string[] modsFromTextBox = textBoxMods.Text.Split(';').Select(m => m.Trim()).ToArray();
                selectedMods.AddRange(modsFromTextBox);
            }

            // Remove duplicates
            selectedMods = selectedMods.Distinct().ToList();

            // Initialize progress bar
            progressBar1.Value = 0;
            progressBar1.Maximum = selectedMods.Count;

            // Copy each selected mod folder from modDir to destinationDir
            foreach (string mod in selectedMods)
            {
                string sourceModPath = Path.Combine(modDir.Text, mod);
                string destinationModPath = Path.Combine(destinationDir, mod);

                if (Directory.Exists(sourceModPath))
                {
                    try
                    {
                        // Copy the mod folder to the destination directory
                        CopyDirectory(sourceModPath, destinationModPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying mod '{mod}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Mod directory '{mod}' not found in '{modDir.Text}'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Update progress bar
                progressBar1.Value += 1;
            }

            MessageBox.Show("Mods copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create destination directory if it does not exist
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get all files in the source directory
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string destinationFilePath = Path.Combine(destinationDir, Path.GetFileName(filePath));
                File.Copy(filePath, destinationFilePath, true); // true to overwrite files if they already exist
            }

            // Recursively copy all subdirectories
            foreach (string directoryPath in Directory.GetDirectories(sourceDir))
            {
                string destinationSubDir = Path.Combine(destinationDir, Path.GetFileName(directoryPath));
                CopyDirectory(directoryPath, destinationSubDir);
            }
        }



        //        RESTART        RESTART        RESTART        RESTART        RESTART


        private DateTime startTime;

        private void ToggleTimerFields()
        {
            bool isTimerEnabled = checkBoxEnableTimer.Checked;

            // Enable or disable the numeric fields based on the checkbox state
            numericUpDownHours.Enabled = isTimerEnabled;
            numericUpDownMinutes.Enabled = isTimerEnabled;
            numericUpDownSeconds.Enabled = isTimerEnabled;
        }
        private void checkBoxEnableTimer_CheckedChanged(object sender, EventArgs e)
        {
            ToggleTimerFields();
        }
        private void StartRestartTimer()
        {
            try
            {
                // Retrieve the user-defined hours, minutes, and seconds from the input
                int hours = (int)numericUpDownHours.Value;
                int minutes = (int)numericUpDownMinutes.Value;
                int seconds = (int)numericUpDownSeconds.Value;

                // Create a TimeSpan from the user's input
                userDefinedInterval = new TimeSpan(hours, minutes, seconds);

                if (userDefinedInterval.TotalSeconds <= 0)
                {
                    MessageBox.Show("Please specify a valid interval (greater than 0 seconds).", "Invalid Interval", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                startTime = DateTime.Now;
                endTime = startTime.Add(userDefinedInterval);

                RestartProgressBar.Minimum = 0;
                RestartProgressBar.Maximum = 100;
                RestartProgressBar.Value = 0;
                endTime = DateTime.Now.Add(userDefinedInterval);

                // Initialize the restartTimer to tick every second (1000ms)
                restartTimer = new System.Threading.Timer(TimerTick, null, 1000, 1000); // Tick every 1 second
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting the process: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void KillAndRestartServer()
        {
            try
            {
                // Stop the server process
                var runningProcesses = Process.GetProcessesByName("DayZServer_x64");

                foreach (var process in runningProcesses)
                {
                    await Task.Delay(1000);
                    process.Kill();
                    process.WaitForExit(); // Wait for the process to terminate
                    SendRestartingWebhook();
                }

                await Task.Delay(10000);
                // Restart the server
                Invoke(new Action(() =>
                {
                    buttonStart_Click(null, null); // Call the button start function
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during restart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void SendRestartingWebhook()
        {
            string EmbedOnRestartBotName = OnRestartBotName.Text;
            string EmbedOnRestartBotAvatar = OnRestartBotAvatar.Text;
            string EmbedOnRestartMessagePrior = OnRestartMessagePrior.Text;
            string EmbedOnRestartServerName = OnRestartServerName.Text;
            string EmbedOnRestartServerIcon = OnRestartServerIcon.Text;
            string EmbedOnRestartServerNameURL = OnRestartServerNameURL.Text;
            string EmbedOnRestartTitle = OnRestartTitle.Text;
            string EmbedOnRestartTitleURL = OnRestartTitleURL.Text;
            string EmbedOnRestartBottomTitle = OnRestartBottomTitle.Text;
            string EmbedOnRestartBottomText = OnRestartBottomText.Text;
            string EmbedOnRestartBottomTextURL = OnRestartBottomTextURL.Text;
            string EmbedOnRestartContentImage = OnRestartContentImage.Text;
            string EmbedOnRestartBigLogo = OnRestartBigLogo.Text;
            int EmbedOnRestartColor = int.Parse(OnRestartEmbedColor.Text.Trim(), System.Globalization.NumberStyles.HexNumber);

            if (EnableWebhookCheckbox.Checked)
            {
                if (RestartWebhookCheckbox.Checked)
                {
                    // Get and validate webhook URL
                    string webhookUrl = webhookTextBox.Text.Trim();
                    if (string.IsNullOrEmpty(webhookUrl) || !Uri.IsWellFormedUriString(webhookUrl, UriKind.Absolute))
                    {
                        MessageBox.Show("Invalid webhook URL provided.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create fields array dynamically based on the checkbox
                    var fieldsList = new List<object>
                        {
                            new
                            {
                                name = "Status",
                                value = "Server Is Preparing For Restart",
                                inline = true
                            }
                        };

                    // Only add the time field if the checkbox is checked
                    if (OnRestartDisplayTime.Checked)
                    {
                        fieldsList.Add(new
                        {
                            name = "Server Time",
                            value = DateTime.Now.ToString("HH:mm:ss"),
                            inline = true
                        });
                    }

                    // Add other fields if needed
                    fieldsList.Add(new
                    {
                        name = $"{EmbedOnRestartBottomTitle}",
                        value = $"[{EmbedOnRestartBottomText}]({EmbedOnRestartBottomTextURL})",
                        inline = false
                    });

                    // Build the full payload
                    var payload = new
                    {
                        username = EmbedOnRestartBotName,
                        avatar_url = EmbedOnRestartBotAvatar,
                        content = EmbedOnRestartMessagePrior,

                        embeds = new[]
                        {
                                new
                                {
                                    title = EmbedOnRestartTitle,
                                    description = "The DayZ server is currently preparing for restart.",
                                    url = EmbedOnRestartTitleURL,
                                    color = EmbedOnRestartColor, // Pass integer color
                                    author = new
                                    {
                                        name = EmbedOnRestartServerName,
                                        icon_url = EmbedOnRestartServerIcon,
                                        url = EmbedOnRestartServerNameURL
                                    },
                                    image = new
                                    {
                                        url = EmbedOnRestartContentImage
                                    },
                                    thumbnail = new
                                    {
                                        url = EmbedOnRestartBigLogo
                                    },
                                    fields = fieldsList.ToArray(),
                                    footer = new
                                    {
                                        text = "Powered By | DzST | DayZ Server Tool by DaBoiJason",
                                        icon_url = "https://media.discordapp.net/attachments/1290605906689527828/1290605948049555486/images.png?ex=67118134&is=67102fb4&hm=349c22bc1c1c19e5f5aa6d4f2a57661121363ae8edb12b3d830f0825d866532f&=&format=webp&quality=lossless"
                                    },
                                    timestamp = DateTime.UtcNow.ToString("o")
                                }
                            }
                    };
                    // Send the webhook using HttpClient
                    using (HttpClient client = new HttpClient())
                    {
                        var jsonPayload = JsonConvert.SerializeObject(payload);
                        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(webhookUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Error sending webhook: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void TimerTick(object state)
        {
            TimeSpan totalDuration = endTime - startTime;
            TimeSpan remainingTime = endTime - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                restartTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                Invoke(new Action(() =>
                {
                    timeremaininglabel.Text = "Waiting for XML attribute to \nshut down the server...";
                    RestartProgressBar.Value = RestartProgressBar.Maximum;
                }));

                KillAndRestartServer();
            }
            else
            {
                double elapsedSeconds = totalDuration.TotalSeconds - remainingTime.TotalSeconds;
                int progress = (int)((elapsedSeconds / totalDuration.TotalSeconds) * 100);
                progress = Math.Clamp(progress, 0, 100); // for .NET 6+, use Clamp; if older, use Math.Min/Max

                Invoke(new Action(() =>
                {
                    RestartProgressBar.Value = progress;
                    timeremaininglabel.Text = $"Time remaining for override \nshutdown check: {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                }));
            }
        }



        //        Discord Webhook        Discord Webhook        Discord Webhook        Discord Webhook



        private void EnableWebhookCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleWebhookField();
        }
        private void ToggleWebhookField()
        {
            bool isWebhookEnabled = EnableWebhookCheckbox.Checked;
            webhookTextBox.Enabled = isWebhookEnabled;
            StartWebhookCheckbox.Enabled = isWebhookEnabled;
            RestartWebhookCheckbox.Enabled = isWebhookEnabled;
        }

        private void buttonBrowseBeExe_Click(object sender, EventArgs e)
        {
            // Create a new instance of FolderBrowserDialog
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Optionally set a description for the dialog
                folderBrowserDialog.Description = "Select the Server's BattleEye folder";

                // Optionally set the root folder (default is Desktop)
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                // Show the dialog and check if the user selected a folder
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path and set it to the textbox
                    textboxBePath.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void InitializeTooltip()
        {
            // Create the tooltip instance
            toolTip = new System.Windows.Forms.ToolTip();
            // Set up the tooltip for the checkbox
            toolTip.SetToolTip(checkBoxAllowExtraParams, "WARNING: Allowing extra parameters can cause issues if not used correctly.");
        }

        ////////////      UPDATE CHECKER      UPDATE CHECKER      UPDATE CHECKER      UPDATE CHECKER
        private async void UpdateChecker_Click(object sender, EventArgs e)
        {
            await CheckForUpdatesAsync();
        }

        // Function to check for updates from GitHub
        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");  // GitHub API requires a User-Agent header

                    // GitHub API URL for the latest release
                    string apiUrl = "https://api.github.com/repos/DaBoiJason/DayZServerTool/releases/latest";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response to get the latest version
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic releaseData = JsonConvert.DeserializeObject(jsonResponse);

                        // Extract the tag_name (like DzSTv3.0.0) and remove the "DzSTv" prefix
                        string latestVersion = releaseData.tag_name;
                        latestVersion = latestVersion.Replace("DzSTv", ""); // Extract version number, e.g., 3.0.0

                        // Compare the current version with the latest version
                        if (IsNewerVersion(latestVersion, CurrentVersion))
                        {
                            // If the version is outdated
                            UpdateVersionLabel.ForeColor = Color.Red;
                            UpdateVersionLabel.Text = $"Outdated version ..{CurrentVersion} \nLatest version ..{latestVersion}";

                            if (!DontShowAgainCheckbox.Checked)
                            {
                                var result = MessageBox.Show($"You are running {CurrentVersion}\nA new version {latestVersion} is available. Would you like to update now?",
                                                             "Update Available",
                                                             MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Information);

                                if (result == DialogResult.Yes)
                                {
                                    // Call the function to handle the update process (e.g., download and install)
                                    PerformUpdate(latestVersion);
                                }
                            }
                        }
                        else
                        {
                            // Notify user that they are on the latest version
                            UpdateVersionLabel.ForeColor = Color.Green;
                            UpdateVersionLabel.Text = $"Up To Date ..{latestVersion}";

                            if (!DontShowAgainCheckbox.Checked)
                            {
                                MessageBox.Show("You're using the latest version!", "No Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error checking for updates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking for updates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Compare version strings to determine if the new version is higher than the current version
        private bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            Version latest = new Version(latestVersion);
            Version current = new Version(currentVersion);
            return latest.CompareTo(current) > 0;
        }

        // Function to handle the update process (downloading and installing the new version)
        private void PerformUpdate(string latestVersion)
        {
            // Implement your logic for downloading and applying the update
            // For example, you might redirect the user to download the latest release from GitHub
            string updateUrl = "https://github.com/DaBoiJason/DayZServerTool/releases/latest";
            MessageBox.Show($"Updating the tool from {updateUrl} to install the latest version ({latestVersion}).",
                            "Update Instructions", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DownloadAndUpdateRepo();
        }
        public void DownloadAndUpdateRepo()
        {
            try
            {
                // Get the directory where the program is running
                string currentDir = AppDomain.CurrentDomain.BaseDirectory;

                // Temp file to store the downloaded zip file
                string tempZipPath = Path.Combine(Path.GetTempPath(), "DayZServerToolUpdate.zip");

                // Download the zip file
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(repoZipUrl, tempZipPath);
                }

                // Directory where the zip will be extracted (temp folder)
                string extractPath = Path.Combine(Path.GetTempPath(), "DayZServerToolUpdate");

                // Extract the zip file
                ZipFile.ExtractToDirectory(tempZipPath, extractPath);

                // Path to the specific subdirectory (DayZServerTool/DayZServerTool)
                string extractedRepoPath = Path.Combine(extractPath, "DayZServerTool-main", "DayZServerTool");

                if (!Directory.Exists(extractedRepoPath))
                {
                    MessageBox.Show("The DayZServerTool directory could not be found in the extracted content.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get current process ID
                int currentProcessId = Process.GetCurrentProcess().Id;

                // Create updater batch file
                string updaterScriptPath = Path.Combine(currentDir, "Updater.bat");

                File.WriteAllText(updaterScriptPath, CreateBatchScript(currentDir, extractedRepoPath, currentProcessId));

                // Start the updater batch script
                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterScriptPath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

                // Close the current app to allow file replacement
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"An error occurred while updating: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Function to generate the batch script content dynamically
        private string CreateBatchScript(string appDir, string extractedRepoPath, int processId)
        {
            return $@"
@echo off
:: Kill the current process by PID
taskkill /PID {processId} /F

:: Wait until the process is completely killed
:waitloop
tasklist /FI ""PID eq {processId}"" | find /I ""{processId}""
if not errorlevel 1 (
    timeout /t 1 /nobreak
    goto waitloop
)

:: Wait an additional second to be sure the process is terminated
timeout /t 1 /nobreak

:: Copy new files from temp folder to the main app directory
xcopy /E /Y ""{extractedRepoPath}\*"" ""{appDir}""
.
:: Clean up temp folder
rmdir /S /Q ""{Path.GetTempPath()}DayZServerToolUpdate""

:: Start the application again
start """" ""{"DayZ Server Tool.exe"}""

:: Log restart status to a file for debugging
echo Application restarted at %date% %time% >> ""{Path.Combine(appDir, "update_log.txt")}""
";
        }



        ////////////      RCON CONFIG         RCON CONFIG         RCON CONFIG         RCON CONFIG         RCON CONFIG         

        private void RconPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            // Enable the Show Password button only if the TextBox is not empty
            ShowRconPass.Enabled = RconPasswordTextBox.Text.Length > 0;
        }
        private void ShowRconPass_MouseDown(object sender, MouseEventArgs e)
        {
            if (RconPasswordTextBox.Text.Length > 0)
            {
                RconPasswordTextBox.PasswordChar = '\0'; // Unmask the password
            }
        }
        private void ShowRconPass_MouseUp(object sender, MouseEventArgs e)
        {
            RconPasswordTextBox.PasswordChar = '*'; // Mask the password
        }
        private string GenerateRandomPassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string symbolChars = "!@#$%^&*()_-+=<>?";

            string allChars = upperChars + lowerChars + numberChars + symbolChars;
            Random random = new Random();
            StringBuilder passwordBuilder = new StringBuilder();

            passwordBuilder.Append(upperChars[random.Next(upperChars.Length)]);
            passwordBuilder.Append(lowerChars[random.Next(lowerChars.Length)]);
            passwordBuilder.Append(numberChars[random.Next(numberChars.Length)]);
            passwordBuilder.Append(symbolChars[random.Next(symbolChars.Length)]);

            for (int i = 4; i < length; i++)
            {
                passwordBuilder.Append(allChars[random.Next(allChars.Length)]);
            }

            return new string(passwordBuilder.ToString().OrderBy(s => random.Next()).ToArray());
        }
        private void RandomGeneratePassowrd_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to generate a strong random 20-character password?",
                                                  "Generate Password",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string generatedPassword = GenerateRandomPassword(20);
                RconPasswordTextBox.Text = generatedPassword;
            }
        }
        private void rconEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = RconEnableCheckBox.Checked;
            RestrictRcoComboBox.Enabled = RconEnableCheckBox.Checked;
            label66.Enabled = RconEnableCheckBox.Checked;
            AntiDupeCheckbox.Enabled = RconEnableCheckBox.Checked;
            RconPasswordTextBox.Enabled = RconEnableCheckBox.Checked;
            ShowRconPass.Enabled = RconEnableCheckBox.Checked;
            RandomGeneratePassowrd.Enabled = RconEnableCheckBox.Checked;
            RconPortNumeric.Enabled = RconEnableCheckBox.Checked;
            RconConfigFileComboBox.Enabled = RconEnableCheckBox.Checked;
            WriteRconCFG.Enabled = RconEnableCheckBox.Checked;
            OverWriteButtonRcon.Enabled = RconEnableCheckBox.Checked;
            label60.Enabled = RconEnableCheckBox.Checked;
            label61.Enabled = RconEnableCheckBox.Checked;
            if (isChecked)
            {
                if (!tabControl3.TabPages.Contains(tabPage14))
                    tabControl3.TabPages.Add(tabPage14);

                if (!tabControl3.TabPages.Contains(tabPage15))
                    tabControl3.TabPages.Add(tabPage15);
            }
            else
            {
                if (tabControl3.TabPages.Contains(tabPage14))
                    tabControl3.TabPages.Remove(tabPage14);

                if (tabControl3.TabPages.Contains(tabPage15))
                    tabControl3.TabPages.Remove(tabPage15);
            }
        }
        private void OverWriteRconCFG_Click(object sender, EventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(textBoxExePath.Text);
            string BattlEyeRconDIR = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\";
            string BattlEyeRconCFG = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\BEServer_x64.cfg";
            string RRcon = "";
            try
            {
                // Check if the directory exists
                if (Directory.Exists(BattlEyeRconDIR))
                {
                    // Get all files with the .cfg extension in the specified directory
                    string[] configFiles = Directory.GetFiles(BattlEyeRconDIR, "*.cfg");

                    // Delete each file
                    foreach (string filePath in configFiles)
                    {
                        File.Delete(filePath);
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                RConsoleOutput.AppendText($"Error deleting files: {ex.Message}\r\n");
            }
            //MessageBox.Show($"Path{directoryPath}\nBEPATH{BattlEyeRconCFG}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (RestrictRcoComboBox.SelectedIndex == 0)
            {
                RRcon = "0";
            }
            else if (RestrictRcoComboBox.SelectedIndex == 1)
            {
                RRcon = "1";
            }
            string[] lines = { $"RConPassword {RconPasswordTextBox.Text}", $"RestrictRCon {RRcon}", $"RConPort {RconPortNumeric.Text}" };
            try
            {
                // Write lines to the file
                File.WriteAllLines(BattlEyeRconCFG, lines);
                Console.WriteLine("Lines written successfully to " + BattlEyeRconCFG);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            PopulateRconConfigFileComboBox();
        }
        private void WriteRconCFG_Click(object sender, EventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(textBoxExePath.Text);
            string BattlEyeRconDIR = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\";
            string BattlEyeRconCFG = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\{RconConfigFileComboBox.Text}";
            string RRcon = "";
            try
            {
                // Check if the directory exists
                if (Directory.Exists(BattlEyeRconDIR))
                {
                    string[] configFiles = Directory.GetFiles(BattlEyeRconDIR, "*.cfg");
                }
                else
                {
                    MessageBox.Show($"Make sure that you Chose an server executable OR...\nOne of the Paths doesnt exist\nPath{directoryPath}\nBEPATH{BattlEyeRconCFG}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Make sure that you Chose an server executable OR...\nOne of the Paths doesnt exist\nPath{directoryPath}\nBEPATH{BattlEyeRconCFG}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //MessageBox.Show($"Path{directoryPath}\nBEPATH{BattlEyeRconCFG}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (RestrictRcoComboBox.SelectedIndex == 0)
            {
                RRcon = "0";
            }
            else if (RestrictRcoComboBox.SelectedIndex == 1)
            {
                RRcon = "1";
            }
            string[] lines = { $"RConPassword {RconPasswordTextBox.Text}", $"RestrictRCon {RRcon}", $"RConPort {RconPortNumeric.Text}" };
            try
            {
                // Write lines to the file
                File.WriteAllLines(BattlEyeRconCFG, lines);
                Console.WriteLine("Lines written successfully to " + BattlEyeRconCFG);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        private void PopulateRconConfigFileComboBox()
        {
            string directoryPath = Path.GetDirectoryName(textBoxExePath.Text);
            string BattlEyeRconDIR = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\";
            // Clear existing items
            RconConfigFileComboBox.Items.Clear();

            // Check if directory exists
            if (Directory.Exists(BattlEyeRconDIR))
            {
                // Get all .cfg files in the directory
                string[] cfgFiles = Directory.GetFiles(BattlEyeRconDIR, "*.cfg");

                // Add each file name (without the path) to the ComboBox
                foreach (string filePath in cfgFiles)
                {
                    RconConfigFileComboBox.Items.Add(Path.GetFileName(filePath));
                }
            }
            else
            {
                MessageBox.Show("Directory not found: " + BattlEyeRconDIR + "\nChoose a server executable in the Main tab or ");
            }
        }

        private void RconConfigFileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string directoryPath = Path.GetDirectoryName(textBoxExePath.Text);
            string BattlEyeRconDIR = $"{directoryPath}\\{profileTextBox.Text}\\BattlEye\\";
            // Get the selected file name
            string selectedFile = RconConfigFileComboBox.SelectedItem?.ToString();
            if (selectedFile != null)
            {
                string filePath = Path.Combine(BattlEyeRconDIR, selectedFile);
                LoadConfigFile(filePath);
            }
        }

        private void LoadConfigFile(string filePath)
        {
            // Read all lines of the config file
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length == 3)
            {
                // Parse each line to extract the relevant data
                string passwordLine = lines[0];
                string restrictLine = lines[1];
                string portLine = lines[2];

                // Parse the RConPassword
                if (passwordLine.StartsWith("RConPassword"))
                {
                    string password = passwordLine.Split(' ')[1];
                    RconPasswordTextBox.Text = password;
                }

                // Parse the RestrictRCon setting
                if (restrictLine.StartsWith("RestrictRCon"))
                {
                    int restrictValue = int.Parse(restrictLine.Split(' ')[1]);
                    RestrictRcoComboBox.SelectedItem = restrictValue == 0 ? "NO" : "YES";
                }

                // Parse the RConPort
                if (portLine.StartsWith("RConPort"))
                {
                    int port = int.Parse(portLine.Split(' ')[1]);
                    RconPortNumeric.Value = port;
                }
            }
            else
            {
                MessageBox.Show("The configuration file format is incorrect.");
            }
        }


        // LOCAL IP      LOCAL IP      LOCAL IP      LOCAL IP      LOCAL IP      LOCAL IP
        private string GetLocalIPv4Address()
        {
            string localIP = "";
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        // MONITOR     MONITOR     MONITOR     MONITOR     MONITOR     MONITOR     MONITOR     MONITOR     MONITOR     MONITOR


        private void timerUpdateUsage_Tick(object sender, EventArgs e)
        {
            try
            {
                // Fetch current values for this application
                float appCpu = cpuCounterApp.NextValue() / Environment.ProcessorCount; // Divide by processor count for accurate percentage
                float appRam = ramCounterApp.NextValue() / (1024 * 1024); // Convert bytes to MB

                // Update labels for the application
                lblAppCpuUsage.Text = $"App CPU Usage: {appCpu:F2}%";
                lblAppRamUsage.Text = $"App RAM Usage: {appRam:F2} MB";

                // Check if DayZServer_x64 is running
                if (IsProcessRunning("DayZServer_x64"))
                {
                    // Fetch current values for DayZServer_x64.exe if running
                    float processCpu = cpuCounterDayZ.NextValue() / Environment.ProcessorCount;
                    float processRam = ramCounterDayZ.NextValue() / (1024 * 1024); // Convert bytes to MB

                    // Update labels for the DayZServer_x64.exe process
                    lblProcessCpuUsage.Text = $"DayZServer CPU Usage: {processCpu:F2}%";
                    lblProcessRamUsage.Text = $"DayZServer RAM Usage: {processRam:F2} MB";
                }
                else
                {
                    // Update labels to indicate waiting status if not running
                    lblProcessCpuUsage.Text = "DayZServer CPU Usage: Waiting For DayZServer_x64";
                    lblProcessRamUsage.Text = "DayZServer RAM Usage: Waiting For DayZServer_x64";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving usage data: " + ex.Message);
            }
        }

        private bool IsProcessRunning(string processName)
        {
            // Check if any processes with the specified name are running
            return Process.GetProcessesByName(processName).Length > 0;
        }







        //////////// RCON WRITEN IN THE WORST WAY POSSIBLE        RCON WRITEN IN THE WORST WAY POSSIBLE        RCON WRITEN IN THE WORST WAY POSSIBLE        

        // Adjusted OnMessageReceived to match expected EventHandler<string> signature
        private void OnMessageReceived(object sender, string message)
        {
            AppendToConsole(message);
        }

        private void AppendToConsole(string content)
        {
            if (RConsoleOutput.InvokeRequired)
            {
                RConsoleOutput.Invoke(new Action(() => AppendToConsole(content)));
            }
            else
            {
                RConsoleOutput.AppendText(content + Environment.NewLine);
            }
        }

        private CancellationTokenSource _keepAliveTokenSource;

        private void StartKeepAlive()
        {
            _keepAliveTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_keepAliveTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Send an empty string as a keep-alive packet
                        networkClient.Send(""); // Sending empty string as keep-alive
                        await Task.Delay(5000, _keepAliveTokenSource.Token); // 40 seconds delay
                    }
                    catch (TaskCanceledException)
                    {
                        // Task was canceled, exit the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions (like disconnection issues)
                        Console.WriteLine("Error during keep-alive: " + ex.Message);
                        break;
                    }
                }
            });
        }

        private void RconConnectButton_Click(object sender, EventArgs e)
        {
            // Check if there is an existing connection
            if (networkClient != null && networkClient.IsConnected)
            {
                MessageBox.Show("Already connected to RCON. Please disconnect before making a new connection.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int rconPort = Decimal.ToInt32(RconPortNumeric.Value);

                // Initialize the RconClient only once with the class-level variable
                networkClient = new RconClient(GetLocalIPv4Address(), rconPort, RconPasswordTextBox.Text);

                // Subscribe to the MessageReceived event
                networkClient.MessageReceived += OnMessageReceived;

                bool initialConnectSuccessful = networkClient.Connect();

                if (networkClient.IsConnected)
                {
                    MessageBox.Show(
                        $"You have been successfully connected\nRcon has been connected locally via\n\nIP:         {GetLocalIPv4Address()}\nPort:      {rconPort}",
                        "Rcon Connected Successfully",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Start the keep-alive task
                    StartKeepAlive();

                    RconConnectButton.Enabled = false;
                }
                else
                {
                    MessageBox.Show(
                        $"There was an error connecting to:\n\nIP:         {GetLocalIPv4Address()}\nPort:      {rconPort}",
                        "Unable To Connect",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RconDisconnectButton_Click(object sender, EventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.MessageReceived -= OnMessageReceived;
                networkClient.Disconnect();
                MessageBox.Show("Disconnected from Rcon", "Disconnected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RconConnectButton.Enabled = true;

                // Cancel the keep-alive task
                _keepAliveTokenSource?.Cancel();
                _keepAliveTokenSource = null;
            }

            // Reset networkClient to allow new connections
            networkClient = null;
        }



        private void ConsoleSendButton_Click(object sender, EventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new SendMessageCommand($"{RconCommandLineTextBox.Text}"));
                RconCommandLineTextBox.Clear();
                var confirmationResult = MessageBox.Show(
                    $"COMMAND EXECUTED {RconCommandLineTextBox.Text}",
                    "Command Debugger",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Not connected to Rcon", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearConsoleButton_Click(object sender, EventArgs e)
        {
            RConsoleOutput.Clear();
        }

        private void onlinePlayerListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OnlinePlayerListView_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the item at the mouse click position
            ListViewItem item = onlinePlayerListView.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                // Select the entire row
                onlinePlayerListView.SelectedItems.Clear(); // Clear previous selections
                item.Selected = true; // Highlight the selected item
                item.Focused = true; // Set focus on the item
            }
        }
        private void CopyIP_Click(object sender, EventArgs e)
        {
            if (onlinePlayerListView.SelectedItems.Count > 0)
            {
                var selectedItem = onlinePlayerListView.SelectedItems[0];
                string valueToCopy = selectedItem.SubItems[3].Text;
                Clipboard.SetText(valueToCopy);
            }
        }

        private void CopyGUID_Click(object sender, EventArgs e)
        {
            if (onlinePlayerListView.SelectedItems.Count > 0)
            {
                var selectedItem = onlinePlayerListView.SelectedItems[0];
                string valueToCopy = selectedItem.SubItems[2].Text;
                Clipboard.SetText(valueToCopy);
            }
        }

        private void CopyNameMenueItem_Click(object sender, EventArgs e)
        {
            if (onlinePlayerListView.SelectedItems.Count > 0)
            {
                var selectedItem = onlinePlayerListView.SelectedItems[0];
                string valueToCopy = selectedItem.SubItems[1].Text;
                Clipboard.SetText(valueToCopy);
            }
        }

        private void VPNChekerMenueItem_Click(object sender, EventArgs e)
        {
            if (onlinePlayerListView.SelectedItems.Count > 0)
            {
                var selectedItem = onlinePlayerListView.SelectedItems[0];
                string valueToCopy = selectedItem.SubItems[3].Text;
                string tempsteamidukprofilelink = $"https://www.ipqualityscore.com/vpn-ip-address-check/lookup/{valueToCopy}";

                try
                {
                    // Open the Discord link in the default web browser
                    Process.Start(new ProcessStartInfo(tempsteamidukprofilelink) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., if no browser is installed)
                    MessageBox.Show("An error occurred while trying to open the Discord link: " + ex.Message);
                }
            }
        }

        // Add event subscriptions when initializing your RconClient
        

        private void LockServerButton_Click(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show(
                    $"Do you want to lock the server ?",
                    "Server lock status about to change !",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
            if (confirmationResult == DialogResult.Yes)
            {
                try
                {
                    networkClient.Send(new LockServerCommand());
                    MessageBox.Show($"The server has been locked!", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while locking the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UnlockServerButton_Click(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show(
                    $"Do you want to unlock the server ?",
                    "Server lock status about to change !",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
            if (confirmationResult == DialogResult.Yes)
            {
                try
                {
                    networkClient.Send(new UnlockCommand());
                    MessageBox.Show($"The server has been unlocked!", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while unlocking the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ServerShutdownViaRcon_Click(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show(
                    $"Are you sure you want to shutdown the server?",
                    "Confirm Server Shutdown",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

            if (confirmationResult == DialogResult.Yes)
            {
                try
                {
                    networkClient.Send(new ShutdownCommand());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while shuting down the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private int MaxPingAllowance;

        private void SetMaxPingButton_Click(object sender, EventArgs e)
        {
            SetMaxPingFromNumericUpDown();
        }
        private void SetMaxPingFromNumericUpDown()
        {
            if (networkClient == null)
            {
                MessageBox.Show("Network client is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int maxPingValue = (int)MaxPingNumericUpDown.Value;
            networkClient.Send(new SetMaxPingCommand(maxPingValue));
            MessageBox.Show($"The max ping value has been set to {maxPingValue}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    public static class ListViewExtensions
    {
        public static int GetColumnIndexAt(this ColumnHeaderCollection columns, int x)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                if (x < columns[i].Width)
                {
                    return i;
                }
                x -= columns[i].Width;
            }
            return -1; // Not found
        }
    }
}