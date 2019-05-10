# Lab 02 - Creating a custom FFU

## Pre-requisites
### Hardware
* Arrow Dragonboard 410c
* Grove LED
* Grove Button

### Software
* Visual Studio 2019 Community Edition (or above)
* Dragonboard 410c - BSP Package https://developer.qualcomm.com/hardware/dragonboard-410c/software
* Windows Assessment and Deployment KiT (Windows ADK)
* Windows 10 IoT Core packages
* IoT Core ADK Add-Ons
* IoT Core PowerShell environment
* Dragonboard update tool

## 1 - Build your image

### 1.0 - Create your workspace
On your desktop open up the folder named iot-adkaddonkit then open the IoTCorePShell script. 

```
New-IoTWorkspace C:\MyWorkspace lab02 arm
Import-IoTOEMPackage *
```

### 1.1 - Install Board Support Package (BSP)

```
Import-IoTBSP 410c db410cetcetc.zip
Add-IoTProduct ProductA 410c
```

### 1.3 - Add Universal Windows App

```
Add-IoTAppxPackage "C:\DefaultApp\IoTCoreDefaultApp_1.2.0.0_ARM_Debug_Test\IoTCoreDefaultApp_1.2.0.0_ARM_Debug_Test.appx" fga Appx.MyUWPApp
New-IoTCabPackage Appx.MyUWPApp
Add-IoTProductFeature ProductA Test APPX_MYUWPAPP -OEM
```

### 1.3 - Compile FFU image

```
New-IoTCabPackage All
New-IoTFFUImage ProductX Test
```

## 2 - Install your image

Use the Windows 10 IoT Core Dashboard to flash the image in the same method as was done in Lab01 using the IoT Dashboard