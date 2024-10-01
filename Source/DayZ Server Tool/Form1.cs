using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.IO;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text;


namespace DayZ_Server_Tool
{
    public partial class Form1 : Form
    {
        // Properties to hold profile data
        private string executablePath;
        private string parameters;
        private string port;
        private string config;
        private string cpu;
        private TimeSpan userDefinedInterval; // To store the interval
        private bool isManualStop = false; // Flag to control manual stop
        private System.Threading.Timer restartTimer;
        private DateTime endTime; // To track when the countdown ends
        private string WebhookURL;

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
                StopWebhook = StopWebhookCheckbox.Checked,
                RestartWebhook = RestartWebhookCheckbox.Checked,
            };

            // Serialize the profile data to JSON and save it to the file
            string json = JsonConvert.SerializeObject(profileData, Formatting.Indented);
            File.WriteAllText(filePath, json);

            // Refresh the profile list after saving
            LoadProfilesFromDirectory();
        }
        private void LoadSelectedProfile()
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

                    // Safely load checkbox values (use 'true' or 'false' when null)
                    checkBox1.Checked = (bool?)profileData?.CheckBox1 ?? false;
                    checkBoxDoLogs.Checked = (bool?)profileData?.CheckBoxDoLogs ?? false;
                    checkBoxAdminLog.Checked = (bool?)profileData?.CheckBoxAdminLog ?? false;
                    checkBoxNetLog.Checked = (bool?)profileData?.CheckBoxNetLog ?? false;
                    checkBoxAllowExtraParams.Checked = (bool?)profileData?.CheckBoxAllowExtraParams ?? false;

                    // Load mods into textBoxMods
                    textBoxMods.Text = profileData?.TextBoxMods ?? string.Empty;

                    // Load timer settings
                    checkBoxEnableTimer.Checked = (bool?)profileData?.TimerEnabled ?? false;
                    numericUpDownHours.Value = (decimal)(profileData?.Hours ?? 0);
                    numericUpDownMinutes.Value = (decimal)(profileData?.Minutes ?? 0);
                    numericUpDownSeconds.Value = (decimal)(profileData?.Seconds ?? 0);

                    // Clear and load ModsCheckedListBox with directories from modDir
                    ModsCheckedListBox.Items.Clear();

                    //Webhook Settings
                    webhookTextBox.Text = profileData?.WebhookURL ?? string.Empty;
                    EnableWebhookCheckbox.Checked = (bool?)profileData?.WebhookCheckbox ?? false;
                    StartWebhookCheckbox.Checked = (bool?)profileData?.StartWebhook ?? false;
                    StopWebhookCheckbox.Checked = (bool?)profileData?.StopWebhook ?? false;
                    RestartWebhookCheckbox.Checked = (bool?)profileData?.RestartWebhook ?? false;

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
                            }
                        }

                        // Now check the items in ModsCheckedListBox based on the mods in textBoxMods
                        string modsFromTextBox = textBoxMods.Text;
                        if (!string.IsNullOrWhiteSpace(modsFromTextBox))
                        {
                            // Split the mods string by semicolon (;) and trim the parts
                            string[] selectedMods = modsFromTextBox.Split(';').Select(mod => mod.Trim()).ToArray();

                            // Iterate over ModsCheckedListBox items and check those present in selectedMods
                            for (int i = 0; i < ModsCheckedListBox.Items.Count; i++)
                            {
                                string currentMod = ModsCheckedListBox.Items[i].ToString();
                                if (selectedMods.Contains(currentMod))
                                {
                                    ModsCheckedListBox.SetItemChecked(i, true);
                                }
                                UpdateButtonsState();
                                ToggleTimerFields();
                                ToggleTimerFields();
                                ToggleWebhookField();
                            }
                        }
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
            modDir.Clear();

            // Clear all entries from the CheckedListBox
            ModsCheckedListBox.Items.Clear();
            UpdateButtonsState();

            // Uncheck all items in the CheckedListBox
            for (int i = 0; i < ModsCheckedListBox.Items.Count; i++)
            {
                ModsCheckedListBox.SetItemChecked(i, false);
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
            StopWebhookCheckbox.Checked = false;
            RestartWebhookCheckbox.Checked = false;

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

                // Terminate each instance found
                foreach (var process in runningProcesses)
                {
                    process.Kill(); // Stop the process
                    process.WaitForExit(); // Optionally wait for the process to exit
                }

                // Stop the restart timer
                restartTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Disable the timer

                // Update the label to show that the timer has been stopped
                timeremaininglabel.Text = "Timer stopped.";
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
        private void loadFileToolStripMenu_Click(object sender, EventArgs e)
        {
            // Call the LoadSelectedProfile function to load the selected profile from comboBoxProfiles
            LoadSelectedProfile();
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

        private void buttonStart_Click(object sender, EventArgs e)
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
                string mods = textBoxMods.Text;

                // Construct the command line arguments
                List<string> arguments = new List<string>
        {
            $"-port={port}",
            $"-cpuCount={cpuCount}",
            $"-config={config}",
            $"-mod={mods}" // Always add the -mod parameter
        };

                // Add optional parameters based on checkbox states
                if (checkBoxDoLogs.Checked) arguments.Add("-doLogs");
                if (checkBoxAdminLog.Checked) arguments.Add("-adminlog");
                if (checkBoxNetLog.Checked) arguments.Add("-netlog");
                if (checkBox1.Checked) arguments.Add("-freezecheck");

                // Join the arguments into a single string
                string argumentString = string.Join(" ", arguments);

                // Start the process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = argumentString,
                    UseShellExecute = true // Use the shell to start the process
                };

                Process.Start(startInfo);

                // If the checkbox for enabling the timer is checked, start the restart loop
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

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
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
                string mods = textBoxMods.Text;

                // Construct the command line arguments
                List<string> arguments = new List<string>
        {
            $"-port={port}",
            $"-cpuCount={cpuCount}",
            $"-config={config}",
            $"-mod={mods}" // Always add the -mod parameter
        };

                // Add optional parameters based on checkbox states
                if (checkBoxDoLogs.Checked)
                {
                    arguments.Add("-doLogs");
                }

                if (checkBoxAdminLog.Checked)
                {
                    arguments.Add("-adminlog");
                }

                if (checkBoxNetLog.Checked)
                {
                    arguments.Add("-netlog");
                }

                if (checkBox1.Checked) // Assuming checkBox1 corresponds to -freezecheck
                {
                    arguments.Add("-freezecheck");
                }

                // Join the arguments into a single string
                string argumentString = string.Join(" ", arguments);

                // Start the process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = argumentString,
                    UseShellExecute = true // Use the shell to start the process
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, invalid path)
                MessageBox.Show($"Error starting the executable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get all instances of DayZServer_x64
                var runningProcesses = Process.GetProcessesByName("DayZServer_x64");

                // Check if any instances are found
                if (runningProcesses.Length == 0)
                {
                    MessageBox.Show("No instance of DayZServer_x64.exe is currently running.", "Process Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return; // Exit if no process is found
                }

                // Terminate each instance found
                foreach (var process in runningProcesses)
                {
                    process.Kill(); // This will stop the process
                    process.WaitForExit(); // Optionally wait for the process to exit
                }

                MessageBox.Show("DayZServer_x64.exe has been stopped.", "Process Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle exceptions that may occur during the process termination
                MessageBox.Show($"Error stopping the executable: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // Clear previous items
                    ModsCheckedListBox.Items.Clear();

                    // Get all directories in the selected path
                    string[] directories = Directory.GetDirectories(folderBrowserDialog.SelectedPath);

                    // Populate the CheckedListBox with folder names
                    foreach (string directory in directories)
                    {
                        // Extract the folder name
                        string folderName = Path.GetFileName(directory);

                        // Check if the folder name is the one to exclude
                        if (folderName != "!DO_NOT_CHANGE_FILES_IN_THESE_FOLDERS")
                        {
                            // Add to CheckedListBox if it's not the excluded folder
                            ModsCheckedListBox.Items.Add(folderName);
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
                    process.Kill(); // Stop the process
                    process.WaitForExit(); // Wait for the process to terminate
                }

                // Wait 5 seconds before restarting
                await Task.Delay(5000);

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
        private void TimerTick(object state)
        {
            // Calculate the remaining time for the countdown
            TimeSpan remainingTime = endTime - DateTime.Now;

            // If time is up, stop the timer and restart the server
            if (remainingTime.TotalSeconds <= 0)
            {
                restartTimer?.Change(Timeout.Infinite, Timeout.Infinite); // Stop the timer
                Invoke(new Action(() => timeremaininglabel.Text = "Restarting...")); // Update the UI

                // Kill the process and restart the server
                KillAndRestartServer();
            }
            else
            {
                // Update the remaining time in the label
                Invoke(new Action(() =>
                {
                    timeremaininglabel.Text = $"Time remaining: {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
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
            StopWebhookCheckbox.Enabled = isWebhookEnabled;
            RestartWebhookCheckbox.Enabled = isWebhookEnabled;
        }

        private void label20_Click(object sender, EventArgs e)
        {

        }
    }
}