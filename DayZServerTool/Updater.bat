
@echo off
:: Kill the current process by PID
taskkill /PID 2620 /F

:: Wait until the process is completely killed
:waitloop
tasklist /FI "PID eq 2620" | find /I "2620"
if not errorlevel 1 (
    timeout /t 1 /nobreak
    goto waitloop
)

:: Wait an additional second to be sure the process is terminated
timeout /t 1 /nobreak

:: Copy new files from temp folder to the main app directory
xcopy /E /Y "C:\Users\13vas\AppData\Local\Temp\DayZServerToolUpdate\DayZServerTool-main\DayZServerTool\*" "D:\VScode repos\DaBoiJason_s DayZ Server Tool\DayZ Server Tool\bin\Debug\net8.0-windows\"

:: Clean up temp folder
rmdir /S /Q "C:\Users\13vas\AppData\Local\Temp\DayZServerToolUpdate"

:: Start the application again
start "" "DayZ Server Tool.exe"

:: Log restart status to a file for debugging
echo Application restarted at %date% %time% >> "D:\VScode repos\DaBoiJason_s DayZ Server Tool\DayZ Server Tool\bin\Debug\net8.0-windows\update_log.txt"
