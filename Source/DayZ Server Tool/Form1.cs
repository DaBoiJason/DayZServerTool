using System;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.IO.Compression;
using System.Net;
using System.Reflection;



namespace DayZ_Server_Tool
{
    public partial class Form1 : Form
    {
        // Properties to hold profile data NO TALL INCLUDED
        private string repoZipUrl = "https://github.com/DaBoiJason/DayZServerTool/archive/refs/heads/main.zip";
        private string CurrentVersion = "3.1.0";
        private string executablePath;
        private string parameters;
        private string port;
        private string config;
        private string cpu;
        private TimeSpan userDefinedInterval; // To store the interval
        private bool isManualStop = false; // Flag to control manual stop
        private System.Threading.Timer restartTimer;
        private DateTime endTime; // To track when the countdown ends
        private string webhookUrl;

        public Form1()
        {
            InitializeComponent();
            LoadProfilesFromDirectory();
            InitializeCpuDropdown();
            checkBoxAllowExtraParams.CheckedChanged += CheckBoxAllowExtraParams_CheckedChanged;
            textBoxParameters.Enabled = false;
            modDir.Enabled = false;
            comboBoxProfiles.SelectedIndex = -1;
            ToggleTimerFields();
            LoadLatestProfileOnStart();
            tabControl1_SelectedIndexChanged(null, null);
            InitializeTooltip();
            CheckForUpdatesAsync();
            VersionNumber.Text = $"Version {CurrentVersion}";
        }
        public class LatestProfile
        {
            public string LatestProfileName { get; set; }
        }
        private void CheckModsInList(CheckedListBox checkedListBox, string modsText)
        {
            if (!string.IsNullOrWhiteSpace(modsText))
            {
                // Split the mods string by semicolon (;) and trim the parts
                string[] selectedMods = modsText.Split(';').Select(mod => mod.Trim()).ToArray();

                // Iterate over checkedListBox items and check those present in selectedMods
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
                    loadFileToolStripMenu_Click(null, null); // Load the selected profile
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
                CheckBox1 = checkBox1.Checked,
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
                EnableRcon = enableRconCheckBox.Checked,
                BEPath = textboxBePath.Text,
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
            };

            // Serialize the profile data to JSON and save it to the file
            string json = JsonConvert.SerializeObject(profileData, Formatting.Indented);
            File.WriteAllText(filePath, json);

            // Refresh the profile list after saving
            LoadProfilesFromDirectory();
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
                    checkBox1.Checked = (bool?)profileData?.CheckBox1 ?? false;
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

                    //RCON
                    enableRconCheckBox.Checked = (bool?)profileData?.EnableRcon ?? false;
                    textboxBePath.Text = profileData?.BEPath ?? string.Empty;

                    // Save the latest profile name
                    SaveLatestProfile(selectedFileName);

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

                        UpdateButtonsState();
                        ToggleTimerFields();
                        ToggleWebhookField();
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
            checkBox1.Checked = false; // Reset Freezecheck
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

            //Rcon
            enableRconCheckBox.Checked = false;

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

        }




        /// <summary>
        /// End of Profile Region      End of Profile Region      End of Profile Region      
        /// </summary>





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

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProfilesFromDirectory(); // Load all profiles on startup
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
                    process.WaitForExit(); // Optionally wait for the process to exit
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

        private void comboBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
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
            string discordLink = "https://discord.gg/JYdvWZgaEb";

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
                };
                // Add optional parameters based on checkbox states
                if (checkBoxDoLogs.Checked) arguments.Add("-doLogs");
                if (checkBoxAdminLog.Checked) arguments.Add("-adminlog");
                if (checkBoxNetLog.Checked) arguments.Add("-netlog");
                if (checkBox1.Checked) arguments.Add("-freezecheck");

                // Start the process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = string.Join(" ", arguments), // Join the arguments into a single string with spaces
                    UseShellExecute = true // Use the shell to start the process
                };
                Process.Start(startInfo);
                await Task.Delay(3000);
                WebhookStartMessageSend();
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
            string EmbedOnStartBotName = OnStartBotName.Text;
            string EmbedOnStartBotAvatar = OnStartBotAvatar.Text;
            string EmbedOnStartMessagePrior = OnStartMessagePrior.Text;
            string EmbedOnStartServerName = OnStartServerName.Text;
            string EmbedOnStartServerIcon = OnStartServerIcon.Text;
            string EmbedOnStartServerNameURL = OnStartServerNameURL.Text;
            string EmbedOnStartTitle = OnStartTitle.Text;
            string EmbedOnStartTitleURL = OnStartTitleURL.Text;
            string EmbedOnStartBottomTitle = OnStartBottomTitle.Text;
            string EmbedOnStartBottomText = OnStartBottomText.Text;
            string EmbedOnStartBottomTextURL = OnStartBottomTextURL.Text;
            string EmbedOnStartContentImage = OnStartContentImage.Text;
            string EmbedOnStartBigLogo = OnStartBigLogo.Text;
            int EmbedOnStartColor = int.Parse(OnStartEmbedColor.Text.Trim(), System.Globalization.NumberStyles.HexNumber);

            if (EnableWebhookCheckbox.Checked)
            {
                if (StartWebhookCheckbox.Checked)
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
                                value = "Server Is Starting",
                                inline = true
                            }
                        };

                    // Only add the time field if the checkbox is checked
                    if (OnStartDisplayTime.Checked)
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
                        name = $"{EmbedOnStartBottomTitle}",
                        value = $"[{EmbedOnStartBottomText}]({EmbedOnStartBottomTextURL})",
                        inline = false
                    });

                    // Build the full payload
                    var payload = new
                    {
                        username = EmbedOnStartBotName,
                        avatar_url = EmbedOnStartBotAvatar,
                        content = EmbedOnStartMessagePrior,

                        embeds = new[]
                        {
                                new
                                {
                                    title = EmbedOnStartTitle,
                                    description = "The DayZ server is starting.",
                                    url = EmbedOnStartTitleURL,
                                    color = EmbedOnStartColor, // Pass integer color
                                    author = new
                                    {
                                        name = EmbedOnStartServerName,
                                        icon_url = EmbedOnStartServerIcon,
                                        url = EmbedOnStartServerNameURL
                                    },
                                    image = new
                                    {
                                        url = EmbedOnStartContentImage
                                    },
                                    thumbnail = new
                                    {
                                        url = EmbedOnStartBigLogo
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
            Keys.Enabled = hasItems; // Assuming your button is named keysButton
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
        // Assuming your TabControl is named tabControl1
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
                tabControl1.Size = new Size(841, 272); // Tab size
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
            else if (tabControl1.SelectedTab == tabPage11)
            {

                this.Size = new Size(635, 708); // Window size
                tabControl1.Size = new Size(596, 632); // Tab size
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

                // Set the end time
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
            // Calculate the remaining time for the countdown
            TimeSpan remainingTime = endTime - DateTime.Now;

            // If time is up, stop the timer and restart the server
            if (remainingTime.TotalSeconds <= 0)
            {
                restartTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
                Invoke(new Action(() => timeremaininglabel.Text = "Waiting for XML attribute to \nshut down the server...")); // Update the UI

                // Kill the process and restart the server
                KillAndRestartServer();
            }
            else
            {
                // Update the remaining time in the label
                Invoke(new Action(() =>
                {
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
                        latestVersion = latestVersion.Replace("DzSTv", ""); // Extract version number, e.g. 3.0.0

                        // Compare the current version with the latest version
                        if (IsNewerVersion(latestVersion, CurrentVersion))
                        {
                            // Prompt the user to update if a newer version is available
                            var result = MessageBox.Show($"A new version ({latestVersion}) is available. Would you like to update now?",
                                                         "Update Available",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Information);

                            if (result == DialogResult.Yes)
                            {
                                // Call the function to handle the update process (e.g., download and install)
                                PerformUpdate(latestVersion);
                            }
                        }
                        else
                        {
                            // Notify user that they are on the latest version
                            MessageBox.Show("You're using the latest version!", "No Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Application.Exit();
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

:: Clean up temp folder
rmdir /S /Q ""{Path.GetTempPath()}DayZServerToolUpdate""

:: Start the application again
start """" ""{"DayZ Server Tool.exe"}""

:: Log restart status to a file for debugging
echo Application restarted at %date% %time% >> ""{Path.Combine(appDir, "update_log.txt")}""
";
        }
        ///////////      WORST ATTEMPT HUMANLY POSSIBLE TO MAKE RCON CONSOLE YOU HAVE BEEN WARNED (Jason)
    }
}