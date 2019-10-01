# Lab 06 - Turn your device into a locked down kiosk

## Option 1 - Using Shell Launcher
### Create your kiosk user 
1. Open PowerShell as Administrator and run the following commands to create a non-admin local user:
```powershell
New-LocalUser "Kiosk"
Add-LocalGroupMember -Group "Users" -Member "Kiosk"
```

### Enable Shell Launcher
1. Open PowerShell as Administrator and run the following commands to enable Shell Launcher:
```powershell
Enable-WindowsOptionalFeature -online -FeatureName Client-EmbeddedShellLauncher -all
```
2. Run the following command to lock down your machine to a single program:
```powershell
#sets up shell launcher
$ShellLauncherClass = [wmiclass]"\\localhost\root\standardcimv2\embedded:WESL_UserSetting"

#gets the SIDs
$Admins_SID = "S-1-5-32-544"
$kiosk_SID = (New-Object System.Security.Principal.NTAccount("Kiosk")).Translate([System.Security.Principal.SecurityIdentifier]).value

#sets up apps for kiosk and admin users
#TODO: replace notepad.exe with a path to the new app
$ShellLauncherClass.SetCustomShell($kiosk_SID, "notepad.exe", ($null), ($null), 1)
$ShellLauncherClass.SetCustomShell($Admins_SID, "powershell.exe")
$ShellLauncherClass.SetEnabled($TRUE)

#disables task manager
$RegKey ="HKLM:\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System"
New-ItemProperty -Path $RegKey -Name DisableTaskMgr -value 1 -Force 
```
3. Restart your computer for the changes to take effect

### Disabling Shell Launcher
1. Log in with your Admin user, a PowerShell session should automatically open.
2. Run the following command to open an Administrator PowerShell session:
```powershell
#opens powershell as administrator
Start-Process powershell -Verb runAs
```
3. In the new Admin PowerShell session run the following commands to disable the Shell Launcher:
```powershell
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

if($ShellLauncherClass.IsEnabled().Enabled)
{
	Write-Warning("Shell Launcher is still Enabled...")
}
else 
{
	Write-Host "Shell Launcher has been disabled..."
	Write-Host "Restarting Computer in 3 seconds..."
	Start-Sleep 3
	Restart-Computer -force
}
```

### Warning
If your shell application requires administrator rights and needs to be elevated, and User Account Control (UAC) is present on your device, you must disable UAC in order for Shell Launcher to launch the shell application.
A custom shell is launched with the same level of user rights as the account that is signed in. This means that a user with administrator rights can perform any system action that requires administrator rights, including launching other applications with administrator rights, while a user without administrator rights cannot.

## Option 2 - Using Assigned Access Provisioning Package

### Create your kiosk user 
1. Open PowerShell as Administrator and run the following commands to create a non-admin local user:
```powershell
New-LocalUser "Kiosk"
Add-LocalGroupMember -Group "Users" -Member "Kiosk"
```

### Install the provisioning package 
1. Open PowerShell as Administrator and run the following command:
```powershell
#installs the provisioning package from a local path
Add-ProvisioningPackage "C:\Labs\Content\src\IoTLabs.AssignedAccess\lab06.ppkg" -force
```
2. When prompted click **Yes, add it**
2. Restart your computer
3. Your device should auto login as the locked down Kiosk user

### Removing the provisioning package 
1. Go to the lock screen by pressing **ctrl + alt + delete**
2. Log in with your Admin account
2. Open PowerShell as Administrator and run the following command:
```powershell
#gets the packageID from the installed lab_06 package
$packageId = (Get-ProvisioningPackage | Where-Object {$_.packageName -eq 'lab_06' }).PackageID.Guid

#removes the package
if($packageId)
{
	Write-Host "Removing Lab_06 provisioning package..."
	Remove-ProvisioningPackage $packageId
}
else
{
	Write-Host "Provisioning package was already removed..."
}
```