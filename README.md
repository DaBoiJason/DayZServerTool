# DayZ Server Tool

The **DayZ Server Tool** is a Windows application designed to facilitate the configuration and management of DayZ server instances. With an intuitive user interface, users can easily set various server parameters, manage mods, and save their configurations in JSON format. The tool also provides logging functionality to help with server diagnostics and monitoring and full RCON access.

## Features

- **User-Friendly Interface**: Simple UI for easy server management.
- **Profile Management**: Save and load server settings using JSON profiles.
- **Executable Management**: Browse and select the DayZ server executable.
- **Server Configuration**: Easily set parameters such as port, CPU count, and configuration file.
- **Mod Management**: Add mods to launch parameters with a simple interface.
- **Extra Mod Management**: Adds Mods and Keys with a simple interface and also has progress bars to check on the progress of the procedure.
- **Logging Options**: Enable various logging options to capture server logs.
- **Process Management**: Start and stop the DayZ server executable from the UI (Including resource monitor on the Server tab).
- **Discord Integration**: Links to Discord server for community support and updates.
- **Rcon Integration**: Allows for full RCON support on the server (playerlist coming soon).


## Installation

1. **Download the latest release**: You can download the pre-built application from the [Releases](https://github.com/DaBoiJason/DayZServerTool/releases/) page.

2. **Extract the ZIP file**: Unzip the downloaded file to your desired location on your computer.

3. **Run the application**: Navigate to the extracted folder and run the `DayZServerTool.exe` executable.

## Usage

1. **Launch the application**: Run the generated executable.

2. **Select the DayZ server executable**: Click the "Browse" button to select the `DayZServer_x64.exe` file.

3. **Configure your server settings**: 
   - Enter the desired port.
   - Select the number of CPU cores to use.
   - Specify the configuration file.
   - Add any mods in the designated field.

4. **Enable logging options**: Check the boxes for the desired logging options.

5. **Enable Extra Parameters (Not recommended unless you know what you are doing)**: Allow for more launch params in the server executable (be cautious with the use of it).

6. **Restart Timer**: Enable restart timer (later on I will implement for mpmission XLM managment).

7. **Discord Webhook Integration**: Allowing for discord messages for start/restart (fully customisable).

8. **Full Rcon Integration**: Rcon implementation into the tool for full managment of the server inside of the tool.

9. **Save your profile**: Name your profile and click the save button to store the settings in a JSON file.

10. **Start/Stop the server**: Use the respective buttons to start or stop the DayZ server process.

11. **Refresh profiles**: You can refresh the list of profiles to load any new configurations you have saved.

## Screenshot

Here is a Preview Gif of the DayZ Server Tool Menue (Wait 10 secs to 1 minute to load):

![DayZ Server Tool Screenshot](https://raw.githubusercontent.com/DaBoiJason/DayZServerTool/refs/heads/main/Assets/Menue.gif)


## Contributing

Contributions are welcome! If you have suggestions for improvements or encounter any issues, please open an issue or submit a pull request.

## License

This project is licensed under the Apache-2.0 License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Special thanks to the DayZ community for their ongoing support and feedback.
- Special thanks to Ryan Torzynski for the [Battleye Rcon library](https://github.com/BytexDigital/BytexDigital.BattlEye.Rcon) that is utilised in this project

## Contact

For support, please reach out to me via Steam for now [Steam](https://steamcommunity.com/id/DaBoiJason/).
