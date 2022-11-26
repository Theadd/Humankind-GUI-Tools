# TROUBLESHOOTING

1. Open `BepInEx\config\BepInEx.cfg` in Notepad or any other plain text editor and set `Enabled = true` under the `[Logging.Console]` section and under the `[Logging.Disk]` section, both.

2. Also, open `BepInEx\config\DevTools.Humankind.GUITools.cfg` and set `QuietMode = false` and `WriteLogToDisk = true`.

3. Save and close both files.

4. Run Humankind and start/load a game or whatever you do to reproduce the error you want to report.

5. Close Humankind.

6. Send the `BepInEx\LogOutput.log` file to rbeltran8000@gmail.com
