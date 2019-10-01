# Lab 06 - Turn your device into a locked down kiosk

## Option 1 - Using Shell Launcher
### Create your kiosk user 
1. Open PowerShell as Administrator and run the following commands to create a non admin local user:
```powershell
New-LocalUser "Kiosk"
```

### Enable Shell Launcher
1. Open PowerShell as Administrator and run the following commands to enable Shell Launcher:
```powershell
Enable-WindowsOptionalFeature -online -FeatureName Client-EmbeddedShellLauncher -all
```
2. Run the following command to lockdown your machine to a single program:
```powershell
#sets up shell launcher
$ShellLauncherClass = [wmiclass]"\\localhost\root\standardcimv2\embedded:WESL_UserSetting"
$ShellLauncherClass.SetDefaultShell("notepad.exe", 1)
$Admins_SID = "S-1-5-32-544"
$kiosk_SID = (New-Object System.Security.Principal.NTAccount("Kiosk")).Translate([System.Security.Principal.SecurityIdentifier]).value

#sets up apps for kiosk and admin users
$ShellLauncherClass.SetCustomShell($kiosk_SID, "notepad.exe", ($null), ($null), 1)
$ShellLauncherClass.SetCustomShell($Admins_SID, "powershell.exe")
$ShellLauncherClass.SetEnabled($TRUE)

#disables task manager
$RegKey ="HKLM:\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System"
New-ItemProperty -Path $RegKey -Name DisableTaskMgr -value 1 -Force 
```

### Disabling Shell Launcher
1. Log in with your admin user
2. Open PowerShell as Administrator and enter the following commands:
```powershell
Start-Process powershell -Verb runAs

#clears the custom shell settings
$CommonArgs = @{"namespace"="root\standardcimv2\embedded"}
Get-WMIObject -class WESL_UserSetting @CommonArgs |
foreach {
    $_.Delete() | Out-Null;
    Write-Host "Deleted $_.Id"
}

#disables shell launcher
$ShellLauncherClass = [wmiclass]"\\localhost\root\standardcimv2\embedded:WESL_UserSetting"
$ShellLauncherClass.SetEnabled($FALSE)

#enables task manager again
$RegKey ="HKLM:\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System"
New-ItemProperty -Path $RegKey -Name DisableTaskMgr -value 0 -Force 

If($ShellLauncherClass.IsEnabled().Enabled)
{
	Write-Warning("Shell Launcher is still Enabled...")
}
Else 
{
	Write-Host "Shell Launcher has been disabled..."
	Write-Host "Restarting Computer now..."
	Start-Sleep 3
	Restart-Computer -force
}
```

### Warning
If your shell application requires administrator rights and needs to be elevated, and User Account Control (UAC) is present on your device, you must disable UAC in order for Shell Launcher to launch the shell application.
Shell Launcher user rights
A custom shell is launched with the same level of user rights as the account that is signed in. This means that a user with administrator rights can perform any system action that requires administrator rights, including launching other applications with administrator rights, while a user without administrator rights cannot.

## Option 2 - Using Assigned Access Provisioning Package

### Create your kiosk user 
1. Open PowerShell as Administrator and run the following commands to create a non admin local user:
```powershell
New-LocalUser "Kiosk"
```

### Install the provisioning package 
1. Open PowerShell as Administrator and run the following command:
```powershell
Add-ProvisiongPackage "C:\Labs\Content\src\IoTLabs.AssignedAccess\lab06.ppkg"
```
2. Restart your computer
