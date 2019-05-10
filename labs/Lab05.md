# Lab 05 - Machine Learning at the Edge

### Software
* Visual Studio Code with Python Plugin
* Visual Studio with UWP Feature Enabled
* Anaconda
 

For this lab, we will use the Azure Custom Vision service to train a machine learning model for image classification. We will use that model to create a .NET application to pull frames from a connected USB camera, use Windows ML to classify the image, then send the result to Azure IoT Hub. We will deploy that application to a Windows IoT Core device using Azure IoT Edge. Finally, we will visualize the results using Time Series Insights.

## Ready to go

When starting the lab, you should have these things open on your development machine:

1. These instructions
1. VS Code open
1. [Custom Vision Portal](https://www.customvision.ai/) open in a browser tab, and logged in with your Azure Subscription. Select the Directory associated with your Azure custom vision resource. 
1. [Time Series Insights explorer](https://insights.timeseries.azure.com/) in another browser tab, also logged in
1. The following service and device information:

Item | Value
--- | ---
Azure Subscription Username	| 
Azure Subscription Password	| 
Container Registry Login Server	|
Container Registry Username	|
Container Registry Password	|
IoT Hub Name	|
IoT Hub Owner Connection String |
IoT Edge Device Name |	
IoT Edge Device Connection String |
Device Name |
Device IP Address |
Device Administrator Password |

# Step 1: Train the Model

1. Plug the USB camera into your development PC.
1. Using the Camera app on your development PC, take at least 5 pictures each of your objects. Store these pictures on your computer. Organize all the photos for each object into a folder named for this object. It will make them easier to upload.
1. Log into the [Custom Vision Portal](https://www.customvision.ai/)
1. Choose the Directory associated with your Azure account
1. Create a New Project. Be sure to choose a "compact" domain.
1. Upload them to your custom vision project. I recommend to upload one object at a time, so it's easy to apply a tag to all your images. Each time you upload all the images for a given object, specify the tag at that time.
1. Select "Train" to train the model
1. Select "Quick Test" to test the model.
. Using the camera app on your PC, take one more picture of one of the objects
1. Upload the picture you took, verify that it was predicted correctly.
1. Export the model. From the "Performance" tab, select the "Export" command.
1. Choose "ONNX", then "ONNX1.2" version.
1. After downloading, rename the file "CustomVision.onnx"

# Step 2: Package the model into a C# .NET Application

## Get the code

If you are running this lab in an environment where this has already been set up, the initial sample will already be present in the directory C:\WindowsAiEdgeLabCV. Otherwise, clone the code as follows:

1. Open a Windows Powershell Prompt.
1. Change to a directory you'll use for the lab code. For example, C:\
1. Clone the lab repo https://github.com/awaregroup/WindowsAiEdgeLabCV/

```
PS C:\> git clone https://github.com/awaregroup/WindowsAiEdgeLabCV/
Cloning into 'WindowsAiEdgeLabCV'...
remote: Azure Repos
remote: Found 78 objects to send. (51 ms)
Unpacking objects: 100% (78/78), done.
PS C:\> cd .\WindowsAiEdgeLabCV\
```

## Get your model file

Copy the CustomVision.onnx model file from your downloads directory where you exported it into the lab directory.

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

# Step 3: Build and push a container

## Connect to our IoT Core device

IoT Core container images must be built on an IoT Core device. 

We will need a way to copy files to our device and a Windows PowerShell window from our development PC connected to that device.

First, we will map the Q: drive to our device so we can access files. 

You'll need the Device IP Address. To get the IP Address open the "IoT Dashboard" from the desktop of your surface and select "My Devices".
 
The name of your device is written on the underside of the case in white.
Right click on your device and select "Copy IPv4 Address".

```
PS C:\WindowsAiEdgeLabCV> $DeviceIPAddress = {ENTER YOUR IP ADDRESS HERE}
PS C:\WindowsAiEdgeLabCV> net use q: \\$DeviceIPAddress\c$ p@ssw0rd /USER:Administrator
The command completed successfully.
```

Second, we'll connect a Windows PowerShell session to our target device. Open a new Windows PowerShell window, and connect to the device. When prompted enter the Device Administrator Password.

```
PS C:\WindowsAiEdgeLabCV> Enter-PSSession -ComputerName $DeviceIPAddress -Credential ~\Administrator
```

## Copy published files to target device

We will copy the 'publish' folder over to our device

```
PS C:\WindowsAiEdgeLabCV> robocopy .\bin\Debug\netcoreapp2.2\win-x64\publish\ q:\data\modules\customvision

-------------------------------------------------------------------------------
    ROBOCOPY     ::     Robust File Copy for Windows
-------------------------------------------------------------------------------

    Started : Wednesday, April 24, 2019 4:31:37 PM
    Source : D:\home\source\Repos\Windows-iotcore-samples\Samples\EdgeModules\SqueezeNetObjectDetection\cs\bin\Debug\netcoreapp2.2\win-x64\publish\
        Dest : q:\data\modules\customvision\

    Files : *.*

    Options : *.* /DCOPY:DA /COPY:DAT /R:1000000 /W:30

------------------------------------------------------------------------------
```

## Test the sample on the target device

Following the same approach as above, we will run the app on the target device to ensure we have the correct camera there, and it's working on that device.

1. Connect the camera to the IoT Core device.
1. In the Windows PowerShell window to the IoT Core device...
1. Change to the "C:\data\modules\customvision" directory
1. Do a test run as we did previously:

```
[192.168.1.102]: PS C:\Data\Users\Administrator\Documents> cd C:\data\modules\customvision

[192.168.1.102]: PS C:\data\modules\customvision> .\WindowsAiEdgeLabCV.exe --model=CustomVision.onnx --device=LifeCam
4/27/2019 8:31:31 AM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/27/2019 8:31:32 AM: ...OK 1516 ticks
4/27/2019 8:31:36 AM: Running the model...
4/27/2019 8:31:38 AM: ...OK 1953 ticks
4/27/2019 8:31:42 AM: Recognized {"results":[{"label":"Mug","confidence":1.0}],"metrics":{"evaltimeinms":1953,"cycletimeinms":0}}
```

## Containerize the sample app

Build the container on the device. To make things easier in this lab, we will set the value of $Container to the address where we will push our container. This will be in the container repository we set up earlier. Find the "Container Registry Login Server" address from the steps above. For example, I am using "aiedgelabcr.azurecr.io" 
to test the lab, so I will do as follows:

```
[192.168.1.102]: PS C:\data\modules\customvision> $Container = "aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore"
```

Still in the "C:\data\modules\customvision" directory, we will now build the container on the IoT Core device.
Note that if we were building for an IoT Enterprise device, we could just do this on our development machine.

```
[192.168.1.102]: PS C:\data\modules\customvision> docker build . -t $Container
Sending build context to Docker daemon  90.54MB

Step 1/5 : FROM mcr.microsoft.com/windows/iotcore:1809
 ---> b292a83fe7c1
Step 2/5 : ARG EXE_DIR=.
 ---> Using cache
 ---> cccdd52d4b4f
Step 3/5 : WORKDIR /app
 ---> Using cache
 ---> 3e071099a8a8
Step 4/5 : COPY $EXE_DIR/ ./
 ---> 951c8a6e96bc
Step 5/5 : CMD [ "WindowsAiEdgeLabCV.exe", "-mCustomVision.onnx", "-dLifeCam", "-ef" ]
 ---> Running in ae981c4d8819
Removing intermediate container ae981c4d8819
 ---> fee066f14f2c
Successfully built fee066f14f2c
Successfully tagged aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore
```

## Push the container

Now that we are sure the app is working correctly within the container, we will push it to our registry.

Before that, we will need to login to the container registry using the Container Registry Username and Container Registry Password obtained in previous steps.

```
PS C:\WindowsAiEdgeLabCV> docker login $ContainerRegistryLoginServer -u $ContainerRegistryUsername -p $ContainerRegistryPassword
Login Succeeded
```

Then we'll push the container into our registry.

```
[192.168.1.102]: PS C:\data\modules\customvision> docker push $Container
The push refers to repository [aiedgelabcr.azurecr.io/customvision]
c1933e4141d1: Preparing
ecdb3e0bf60d: Preparing
b7f45a54f179: Preparing
6bd44acbda1a: Preparing
13e7d127b442: Preparing
13e7d127b442: Skipped foreign layer
c1933e4141d1: Pushed
b7f45a54f179: Pushed
6bd44acbda1a: Pushed
ecdb3e0bf60d: Pushed
1.0-x64-iotcore: digest: sha256:7ba0ac77a29d504ce19ed2ccb2a2c67addb24533e4e3b66476ca018566b58086 size: 1465
```

# Step 4: Create an Azure IoT Edge deployment to the target device

## Author a deployment.json file

Now that we have a container with our inferencing logic safely up in our container registry, it's time to create an Azure IoT Edge deployment to our device.

We will do this back on the development PC.

Amongst the lab files, you will find a deployment json file named deployment.win-x64.json. Open this file in VS Code. We must fill in the details for the container image we just built above, along with our container registry credentials.

Search for "{ACR_*}" and replace those values with the correct values for your container repository.
The ACR_IMAGE must exactly match what you pushed, e.g. aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore

```
    "$edgeAgent": {
      "properties.desired": {
        "runtime": {
          "settings": {
            "registryCredentials": {
              "{ACR_NAME}": {
                "username": "{ACR_USER}",
                "password": "{ACR_PASSWORD}",
                "address": "{ACR_NAME}.azurecr.io"
              }
            }
          }
        }
...
        "modules": {
            "squeezenet": {
            "settings": {
              "image": "{ACR_IMAGE}",
              "createOptions": "{\"HostConfig\":{\"Devices\":[{\"CgroupPermissions\":\"\",\"PathInContainer\":\"\",\"PathOnHost\":\"class/E5323777-F976-4f5b-9B55-B94699C46E44\"},{\"CgroupPermissions\":\"\",\"PathInContainer\":\"\",\"PathOnHost\":\"class/5B45201D-F2F2-4F3B-85BB-30FF1F953599\"}],\"Isolation\":\"Process\"}}"
            }
          }
```

## Deploy edge modules to device

Refer to this guide: [Deploy Azure IoT Edge modules from Visual Studio Code](https://docs.microsoft.com/en-us/azure/iot-edge/how-to-deploy-modules-vscode)

1. In VS Code, open the "Azure IoT Hub Devices" pane. 
1. Locate the device there named according to the edge device name from when you created it in the hub. 
1. Right-click on that device, then select "Create deployment for single device".
1. Choose the deployment.json file you edited in the step above.
1. Press OK
1. Look for "deployment succeeded" in the output window.

```
[Edge] Start deployment to device [ai-edge-lab-device]
[Edge] Deployment succeeded.
```

## Verify the deployment on device

Wait a few minutes for the deployment to go through. On the target device you can inspect the running modules. Success looks like this:

```
[192.168.1.102]: PS C:\data\modules\customvision> iotedge list
NAME             STATUS           DESCRIPTION      CONFIG
customvision     running          Up 32 seconds    aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore
edgeAgent        running          Up 2 minutes     mcr.microsoft.com/azureiotedge-agent:1.0
edgeHub          running          Up 1 second      mcr.microsoft.com/azureiotedge-hub:1.0
```

Once the modules are up, you can inspect that the "customvision" module is operating correctly:

```
[192.168.1.102]: PS C:\data\modules\customvision> iotedge logs customvision
4/27/2019 9:04:59 AM: WindowsAiEdgeLabCV module starting.
4/27/2019 9:04:59 AM: Initializing Azure IoT Edge...
4/27/2019 9:06:11 AM: IoT Hub module client initialized.
4/27/2019 9:06:11 AM: ...OK 71516 ticks
4/27/2019 9:06:11 AM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/27/2019 9:06:15 AM: ...OK 4140 ticks
4/27/2019 9:06:25 AM: Running the model...
4/27/2019 9:06:27 AM: ...OK 1500 ticks
4/27/2019 9:06:27 AM: Recognized {"results":[{"label":"Mug","confidence":1.0}],"metrics":{"evaltimeinms":1500,"cycletimeinms":0}}
```

Finally, back on the development machine, we can monitor device to cloud (D2C) messages from VS Code to ensure the messages are going up.

1. In VS Code, open the "Azure IoT Hub Devices" pane. 
1. Locate the device there named "ai-edge-lab-device". 
1. Right-click on that device, then select "Start monitoring D2C message".
1. Look for inferencing results in the output window.

```
[IoTHubMonitor] [9:07:44 AM] Message received from [ai-edge-lab-device/customvision]:
{
  "results": [
    {
      "label": "Mug",
      "confidence": 1
    }
  ],
  "metrics": {
    "evaltimeinms": 1484,
    "cycletimeinms": 0
  }
}
```

Once you see this, you can be certain the inferencing is happening on the target device and flowing up to the Azure IoT Hub.


## 2 - XGboost

### 2.1 - Introduction to XGboost

XGBoost is a machine learning framework for fitting classification and regression tasks on tabular data. 

This model works by iteratively building decision trees that correct for one anothers error and collectively model interdependencies in the data in a way that a singlur tree cannot.

The training process demonstrated today solves a regression problem, but this can be adapted to forecasting, prediction, and classification wth minimal changes.

### 2.2 - Training a model

1. Data importing with pandas
1. Data pre-processing 
	- Separation of target value from features
	- Test/Train split
1. Specify model and parameters
1. Tune model
1. Export model to ONNX file


## 3 - Integration into TSI

## 3.1 - Add a consumer group to your IoT hub
1. Applications use consumer groups to pull data from Azure IoT Hub. Provide a dedicated consumer group that's used only by this Time Series Insights environment to reliably read data from your IoT hub.
1. To add a new consumer group to your IoT hub:
1. In the Azure portal, find and open your IoT hub.
1. In the menu, under Settings, select Built-in Endpoints, and then select the Events endpoint.
1. Under Consumer groups, enter a unique name for the consumer group. Use this same name in your Time Series Insights environment when you create a new event source.
1. Select Save.

## 3.2 - Add a new event source
1. Sign in to the Azure portal.
1. In the left menu, select All resources. Select your Time Series Insights environment.
1. Under Environment Topology, select Event Sources, and then select Add.
1. In the New event source pane, for Event source name, enter a name that's unique to this Time Series Insights environment. For example, enter event-stream.
1. For Source, select IoT Hub.
1. Select a value for Import option:
	* If you already have an IoT hub in one of your subscriptions, select Use IoT Hub from available subscriptions. This option is the easiest approach.
	* If the IoT hub is external to your subscriptions, or if you want to choose advanced options, select Provide IoT Hub settings manually.
1. The following table describes the required properties for the Provide IoT Hub settings manually:
Property | Description
--- | ---
Subscription ID | the subscription in which the IoT hub was created.
Resource group | The resource group name in which the IoT hub was created.
IoT hub name | the name of the IoT hub.
IoT hub policy name | the shared access policy. You can find the shared access policy on the IoT hub settings tab. Each shared access policy has a name, permissions that you set, and access keys. The shared access policy for your event source must have service connect permissions.
IoT hub policy key | The key is prepopulated.
IoT hub consumer group | The consumer group that reads events from the IoT hub. We highly recommend that you use a dedicated consumer group for your event source.
Event serialization format | Currently, JSON is the only available serialization format. The event messages must be in this format or no data can be read.
Timestamp property name | To determine this value, you need to understand the message format of the message data that's sent to the IoT hub. This value is the name of the specific event property in the message data that you want to use as the event timestamp. The value is case-sensitive. If left blank, the event enqueue time in the event source is used as the event timestamp.
1. Add the dedicated Time Series Insights consumer group name that you added to your IoT hub.
1. Select Create.
1. After you create the event source, Time Series Insights automatically starts streaming data to your environment.
