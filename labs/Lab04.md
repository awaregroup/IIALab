# Lab 04

## Deploying Azure IoT Platform and connecting your device

Windows 10 IoT Core is a version of Windows 10 that is optimized for smaller devices with or without a display that run on both ARM and x86/x64 devices

Windows 10 IoT Core
Built for small, secured smart devices, Windows 10 IoT Core embraces a rich UWP app experience

### Set up UP2 by flashing the FFU

Explain how this is like FFU for the dragonboard, pre-prepared development FFU with Container support enabled.

#### Install WinPE tools onto a drive & Copy the eMMC Installer Script & FFU Across

Explain WinPE and how it fits into things & eMMC installer is a basic script that applies the WIM to the first internal HDD it finds.

1. Insert a USB drive into your machine.
1. Create a USB-bootable WinPE image:
1. Start the Deployment and Imaging Tools Environment (C:\Program Files (x86)\Windows Kits\10\Assessment and Deployment Kit\Deployment Tools) as an administrator.
1. Create a working copy of the Windows PE files. Specify either x86, amd64 or ARM: Copype amd64 C:\WINPE_amd64
1. Install Windows PE to the USB flash drive, specifying the WinPE drive letter below. More information can be found here. MMakeWinPEMedia /UFD C:\WinPE_amd64 P:
1. Download the Windows 10 IoT Core image by double-clicking on the downloaded ISO file and locating the mounted Virtual CD-drive.
1. This drive will contain an install file (.msi); double click it. This will create a new directory on your PC under C:\Program Files (x86)\Microsoft IoT\FFU\ in which you should see an image file, "flash.ffu".
1. Download, unzip and copy the eMMC Installer script to the USB device's root directory, along with the device's FFU.
1. Connect the USB drive, mouse, and keyboard to the USB hub. Attach the HDMI display to your device, the device to the USB hub, and the power cord to the device.


#### Boot off the USB drive and run the Installer Script

Explain how to do this, the UP2's default to booting off the usb drive. Ensure they reset correctly.

IoT Edge is in GA on Windows and installs as a windows feature (todo: get the right name) - install and set up and check that DPS has associated correctly. Also view config.yaml (for debugging)

1. Go to the BIOS setup of the device. Select Windows as the Operating system and set the device to boot from your uSB drive. When the system reboots, you will see the WinPE command prompt. Switch the WinPE prompt  to the USB Drive. This is usually C: or D: but you may need to try other driver letters.
1. Run the eMMC Installer script, which will install the Windows 10 IoT Core image to the device's eMMC memory. When it completes, press any key and run wpeutil reboot. The system should boot into Windows 10 IoT Core, start the configuration process, and load the default application.

## Deploying containers

A container is a runnable instance of an image. You can create, start, stop, move, or delete a container using the Docker API or CLI. You can connect a container to one or more networks, attach storage to it, or even create a new image based on its current state.

By default, a container is relatively well isolated from other containers and its host machine. You can control how isolated a container’s network, storage, or other underlying subsystems are from other containers or from the host machine.

A container is defined by its image as well as any configuration options you provide to it when you create or start it. When a container is removed, any changes to its state that are not stored in persistent storage disappear.

Why it is useful to them. 

### Set up DPS by grabbing the needful things from the Device Portal

This is like the other method of DPS except it associates only IoT Edge and the containers within it.

IoT Core Dashboard
1. Grab the Hardware Device Identity
1. Set up a Device ID in the portal with that Hardware Device Identity, keep this as it will be used later.


### Set up IoT Edge by installing it (Notice the cab file installs / we could get this integrated with DISM?)

1. Install Azure IoT Edge. Follow this guide: [Install the Azure IoT Edge runtime on Windows](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-install-iot-edge-windows)
1. After installing Azure IoT Edge, deploy the [Simulated Temperature Sensor](https://docs.microsoft.com/en-us/azure/iot-edge/quickstart). 
1. In VS Code, open the "Azure IoT Hub Devices" pane. 
1. Look for the Edge Device Name there. 
1. Expand "Modules". Notice three modules there, all green and connected.
1. Right-click on that device, then select "Start monitoring D2C message".
1. Look for simulated temperature sensor results in the output window.


What is IoT Edge, container delivery mechanism.

### Deploy the default Temperature sensor monitor deployment

Introduction to deployment.json, expected state for IoT Edge, as well as the integrations with the IoT Hub.

```
az login
az iot edge set-modules --device-id [device id] --hub-name [hub name] --content deployment.example.json
```

### Build and deploy a custom model that detects faces (from the ONNX model zoo) using existing software over IoT Edge

## 4.3 - Monitor Device to Cloud messages

1. In Visual Studio Code, open the 'Azure IoT Hub Devices' pane  
1. Right-click your device and 'Start monitoring D2C messages'
1. Test your model by holding up objects in front of the camera


#### Grab the C# and such for any vision model we provide and run it on the PC
We can use our own model, download the C# code and run it, build the docker container 

#### Set up and deploy Azure Container Registry

Refer to this guide: [Quickstart: Create a private container registry using the Azure portal](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal)

1. Sign into the Azure Portal
1. Create a new "Container Registry" resource
1. Once created, switch to the "Access Keys" pane.
1. Enable the "Admin User"
1. Make note of the Login Server, username, and password. You'll need these later.


## 3.5 - Containerize the sample app


```powershell
#SAMPLE: customvision:1.0-x64-iotcore
$container = "ENTER YOUR CONTAINER NAME HERE"
#SAMPLE: aiedgelabcr.azurecr.io/
$registryName = "ENTER YOUR REGISTRY NAME HERE"
docker build . -t "$registryName.azurecr.io/$container"
```

## 3.6 - Authenticate and push to Azure Container Registry

1. Authenticate to the Azure Container Registry

```powershell
az acr login --name $registryName
docker push $container
```

## 4.1 - Deploy edge modules to device INtroduction to deployment.json

Refer to this guide for more information: [Deploy Azure IoT Edge modules from Visual Studio Code](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-deploy-modules-vscode)

1. In Visual Studio Code, open the 'Azure IoT Hub Devices' pane by selecting the Explorer sidebar in the top left corner (Ctrl + Shift + E) and then clicking on the 'Azure IoT Hub Devices' at the bottom of the sidebar when it opens
1. If you see '-> Select IoT Hub', you will need to log in with your Azure Subscription, select the 'MS IoT Labs - Windows IoT' subscription and the IoT Hub
1. Right-click on your device (for example, 'A09') and click 'Create Deployment for Single Device'
1. Select ```C:\Users\Admin\Desktop\WindowsIoT\deployment.json```
1. Look for "deployment succeeded" in the output window.


## 4.2 - Verify the deployment on IoT device

The module deployment is instant, however changes to the device can take around 5-7 minutes to take effect. On the **target device** you can inspect the running modules with the following command in the remote PowerShell terminal:

```powershell
iotedge list
```

Once the modules have deployed to your device, you can inspect that the "customvision" module is operating correctly:

```powershell
iotedge logs customvision
```

On main PC, 
az iot hub module-identity list --device-id $deviceId --hub-name $iotHub