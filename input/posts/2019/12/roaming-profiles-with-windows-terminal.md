---
Title: Roaming profiles with Windows Terminal
Slug: roaming-profiles-with-windows-terminal
Date: 2019-12-02
RedirectFrom: 2019/12/roaming-profiles-with-windows-terminal/index.html
Tags:
- Windows 10
- Windows Terminal
---

If you have more than one computer that you use, you might have noticed
that it requires some work to keep your Windows Terminal profile up to date.

I've always been a fan of keeping configuration files and similar in 
version control but sadly Windows Terminal doesn't support merging 
external configurations with the main one. I've 
[opened an issue](https://github.com/microsoft/terminal/issues/2933) 
in Windows Terminals GitHub repository suggesting this, but until that has
been implemented you can work around this by creating a symlink to 
the configuration file in a Git repository or similar.

Below you see a small PowerShell script which will do this for you.

```powershell
###############
# Install.ps1 #
###############

# Make sure Windows Terminal have been installed.
$TerminalPath = Join-Path $Env:LOCALAPPDATA "Packages/Microsoft.WindowsTerminal_8wekyb3d8bbwe";
if(!(Test-Path $TerminalPath)) {
    Throw "Windows Terminal have not been installed."
}

# Create symlink to Windows Terminal settings.
$TerminalProfileSource = Join-Path $PWD "profiles.json"
$TerminalProfileDestination = Join-Path $TerminalPath "LocalState/profiles.json";
if(Test-Path $TerminalProfileDestination) {
    Remove-Item -Path $TerminalProfileDestination;
}
Write-Host "Creating symlink to Windows terminal settings..."
New-Item -Path $TerminalProfileDestination -ItemType SymbolicLink -Value $TerminalProfileSource | Out-Null;
```

Place this PowerShell file in the same directory as your `profiles.json` and
run it as administrator and you now have a roaming Windows Terminal profile. 
However, there's a downside with this approach. Any changes to the file 
won't be picked up by Windows Terminal automatically, so you will need to 
restart the terminal for it to  reload the configuration.

I've created a repository for my own configurations which include configuration files
for PowerShell as well as [Starship](https://starship.rs/), which you can find at 
[https://github.com/patriksvensson/machine](https://github.com/patriksvensson/machine).
Feel free to fork or copy it if you want a starting point for your own configuration files.