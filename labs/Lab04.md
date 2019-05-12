# Lab 04

## Deploying Azure IoT Platform and connecting your device

Windows 10 IoT Core is a version of Windows 10 that is optimized for smaller devices with or without a display that run on both ARM and x86/x64 devices

Windows 10 IoT Core
Built for small, secured smart devices, Windows 10 IoT Core embraces a rich UWP app experience

### Set up UP2 by flashing the FFU

TODO: Explain how this is like FFU for the dragonboard, pre-prepared development FFU with Container support enabled. Name the UP2 with the name on the UP2 device (check IP address for confirmation)

#### Boot off the USB drive and run the Installer Script

TODO: Explain how to do this, the UP2's default to booting off the usb drive. Ensure they reset correctly.

IoT Edge is in GA on Windows and installs as a windows feature (todo: get the right name) - install and set up and check that DPS has associated correctly. Also view config.yaml (for debugging)

1. Go to the BIOS setup of the device. Select Windows as the Operating system and set the device to boot from your USB drive. When the system reboots, you will see the WinPE command prompt. Switch the WinPE prompt  to the USB Drive. This is usually C: or D: but you may need to try other driver letters.
1. Run the eMMC Installer script, which will install the Windows 10 IoT Core image to the device's eMMC memory. When it completes, press any key and run wpeutil reboot. The system should boot into Windows 10 IoT Core, start the configuration process, and load the default application.

## Deploying containers

TODO : Fix up this whole segment

Deploying and updating code is often challenging, especially when working with devices that may exibit different behavour than the machines that the code development occurs on. Azure IoT Edge simplifies this by orchistrating distribution of container images. These images define the operating system, any set up and configuration and the executable files to be run. As the entire execution environment is shipped with the software solution itself environment configuration issues are much less likely to occur. 

Within a container you can control how isolated the network, storage, or other underlying subsystems are from other running processes on the same machne. Containers are also immutable and stateless by nature in that when they are removed, any changes to its state that are not stored in persistent storage disappear.

Azure IoT Edge is a cloud based container orchistration service that allows secure integration with IoT Hubs, enabling containers (defined as modules) to send messages to the IoT Hub without the need to hard code connection strings within the container definition or the image itself. 


### Set up DPS by grabbing the needful things from the Device Portal

This is like the other method of DPS except it associates only IoT Edge and the containers within it.

### 1.1 - Getting unique device identifiers

**TODO :**

``` 
limpet -azuredps -enrollmentinfo
```

### 1.2 - Deploying Azure DPS

1. Create a new "IoT Hub Device Provisioning Service" in the Azure Portal
1. Open the newly created DPS Service 
1. TODO : Get Scope ID
1. Switch to the "Manage enrolments" pane
1. Click "Add individual enrolment"
1. Select "TPM" as the mechanism,
1. Enter the endorsement key from the previous step
1. Enter the registration id gathered from the previous step
1. Enter the device id (this is the name of the device that will be created in IoT Hub)
1. Select the IoT Edge toggle to "True"
1. Click "Link a new IoT Hub" and select your existing IoT Hub with the "iothubowner" policy
1. Click "Save"

### Set up IoT Edge

1. Remotely connect to powershell on the Device TODO: Instructions for remotely connecting
1. Type in the following to the console and wait for the operation to complete, the machine will restart during this process.

```
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Deploy-IoTEdge
```

1. After the IoT Edge agent has been installed we can connect it up to our IoT Hub by enrolling it with DPS. You will need to enter your scope id and your registration id that you retrieved in step 1.2.

```
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Initialize-IoTEdge -Dps
```

1. You can check that IoT Edge has installed correctly by typing "iotedge check" and viewing the output.

### Deploy the default Temperature sensor monitor deployment

TODO : Introduction to deployment.json, expected state for IoT Edge, as well as the integrations with the IoT Hub. Location of the example deployment.

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


## Build & Test the sample

```
PS C:\WindowsAiEdgeLabCV> dotnet restore -r win-x64
PS C:\WindowsAiEdgeLabCV> dotnet publish -r win-x64
Microsoft (R) Build Engine version 16.0.225-preview+g5ebeba52a1 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 43.29 ms for C:\WindowsAiEdgeLabCV\WindowsAiEdgeLabCV.csproj.
  WindowsAiEdgeLabCV -> C:\WindowsAiEdgeLabCV\bin\Debug\netcoreapp2.2\WindowsAiEdgeLabCV.dll
  WindowsAiEdgeLabCV -> C:\WindowsAiEdgeLabCV\bin\Debug\netcoreapp2.2\publish\
```

Point the camera at one of your objects, still connected to your development PC.

Run the sample locally to classify the object. This will test that the app is running correctly locally. We specify "Lifecam" for this model of camera. Here we can see that a "Mug" has been recognized.

```
PS C:\WindowsAiEdgeLabCV> dotnet run --model=CustomVision.onnx --device=LifeCam
4/24/2019 4:09:04 PM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/24/2019 4:09:04 PM: ...OK 594 ticks
4/24/2019 4:09:05 PM: Running the model...
4/24/2019 4:09:05 PM: ...OK 47 ticks
4/24/2019 4:09:05 PM: Recognized {"results":[{"label":"Mug","confidence":1.0}],"metrics":{"evaltimeinms":47,"cycletimeinms":0}}
```

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

## 4.1 - Deploy edge modules to device 

## Author a deployment.json file

Now that we have a container with our inferencing logic safely up in our container registry, it's time to create an Azure IoT Edge deployment to our device.

We will do this back on the development PC.

TODO: Specify where in the lab files
Amongst the lab files, you will find a deployment json file named deployment.win-x64.json. Open this file in VS Code. We must fill in the details for the container image we just built above, along with our container registry credentials.

Search for "{ACR_*}" and replace those values with the correct values for your container repository.
The ACR_IMAGE must exactly match what you pushed, e.g. aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore


## Deploy the IoT Edge deployment.json file. 

Just like the example deployment, use the following syntax to update the expected module state on the device. IoT Edge will pick up on this configuration change and deploy the container to the device.

```
az iot edge set-modules --device-id [device id] --hub-name [hub name] --content deployment.json
```


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