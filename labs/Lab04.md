# Lab 04 - Introduction to Azure IoT Edge

This lab introduces Azure IoT Edge with Windows 10 IoT Core.

## 1.0 - Set up your UP Squared Device

### 1.1 - Cloud setup

1. Attach Ethernet, HDMI, Keyboard/Mouse into the UP Squared device
1. Make a note of the UP Squared device name written on the case starting with A. For example, A19 
1. Open a browser and navigate to the [Azure Portal https://portal.azure.com](https://portal.azure.com). Use the lab credentials provided
1. Click "Resource groups" on the left-hand menu, select the "winiot" resource group in the list and choose the IoT Hub created in [Lab 2](./Lab02.md#11---deploy-azure-iot-hub)
![](./media/2_azure5.png)
1. Click "IoT Edge" on the IoT Hub side menu and click "Add an IoT Edge device" at the top. **Note: that this is a slightly different menu than the one used earlier in the lab**
![IoT Hub Portal](./media/4_SelectIoTEdge.png)
1. Enter the UP Squared name (from step 2) as the device id and click "Save" to create the device
1. Refresh the list and open the device properties
1. Copy "Connection string (primary key)" to the clipboard
![IoT Edge Device Information](./media/4_CopyConnectionStringIoTEdge.png)


### 1.1 - Device setup

1. Open ```IoT Dashboard```, right click on your device starting with A (for example, A19) and click "Launch PowerShell". When prompted, enter "p@ssw0rd" as the password
![IoT Dashboard](./media/4_SelectPowershellDevice.png)
2. Install the Azure IoT Edge runtime on the device by running the following command and wait for the device to reboot:

```
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Deploy-IoTEdge
```

3. Re-connect the remote PowerShell session 
4. Configure the Azure IoT Edge runtime with the following command:

```
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Initialize-IoTEdge
```

5. Enter ```iotedge check``` to validate the Azure IoT Edge runtime installation. ```Get-IoTEdgeLog``` can also be used to debug issues


## 2.0 - Deploy Simulated Temperature Sensor

### 2.1 - Module deployment

1. Close the remote PowerShell and run the following commands on the laptop PowerShell:

```
az extension add --name azure-cli-iot-ext

az login
az iot edge set-modules --device-id [device name] --hub-name [hub name] --content "C:\Labs\Content\src\IoTLabs.IoTEdge\deployment.template.lab04.win-x64.json"
```

### 2.2 - Monitor Device to Cloud messages

1. Enter the following command to monitor Device-to-Cloud (D2C) messages being published to the IoT Hub:

```
az iot hub monitor-events -n [hub name] -d [device id]
```
 
## 3.0 - Custom container deployment

### 3.1 - Deploy Azure Container Registry (ACR)

For more information read: [Quickstart: Create a private container registry using the Azure portal](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal).

1. Sign into the Azure Portal
1. Create a new "Container Registry" resource
1. Once created, switch to the "Access Keys" pane
1. Enable the "Admin User"
1. Make note of the Login Server, username, and password. You'll need these later


### 3.2 - Build and test code

1. Open PowerShell and type: 

```
cd "C:\Labs\Content\src\IoTLabs.CustomVision"
dotnet restore -r win-x64
```

2. Connect the USB webcam to your laptop and point the camera at the UP Squared
4. Run the following command to start the machine learning application. The console should return a string identifying the UP Squared: 

```
dotnet run --model=CustomVision.onnx --device=LifeCam
```

Output should match the following:

```
4/24/2019 4:09:04 PM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/24/2019 4:09:04 PM: ...OK 594 ticks
4/24/2019 4:09:05 PM: Running the model...
4/24/2019 4:09:05 PM: ...OK 47 ticks
4/24/2019 4:09:05 PM: Recognized {"results":[{"label":"up2","confidence":1.0}],"metrics":{"evaltimeinms":47,"cycletimeinms":0}}
```

### 3.3 - Containerize the sample app 

1.  Publish the executables into a folder named release by running the following command:

```
dotnet publish -c Release -o ./release -r win-x64
```

2. Then enter the name of your container (note: the number value after the colon denotes the version, increment this every time you make a change)
```powershell
#SAMPLE: aiedgelabcr
$registryName = "[azure-container-registry-name]"
$version = "1.0"
$imageName = "customvision"

$containerTag = "$registryName.azurecr.io/$($imageName):$version-x64-iotcore"
docker build . -t $containerTag
```


### 3.4 - Push containter to ACR

1. Run the following commands to login and upload the container into Azure:

```powershell
az acr login --name $registryName
docker push $containerTag
```

## 4.0 - Deploy IoT Edge Modules

### 4.1 - Author a deployment.json file

Now that we have a container image with our inferencing logic stored in our container registry, it's time to create an Azure IoT Edge deployment to our device.

1. Go to "C:\Labs\Content\src\IoTLabs.IoTEdge"
1. Edit the "deployment.template.lab04.win-x64.json" file
1. Search for any variables starting with ACR and replace those values with the correct values for your container repository. The ACR_IMAGE must exactly match what you pushed, e.g. aiedgelabcr.azurecr.io/customvision:1.0-x64-iotcore

**Hint: you can type $containerTag to get the full container string from PowerShell.**


### 4.2 - Deploy the IoT Edge deployment.json file. 

Just like the example deployment, use the following syntax to update the expected module state on the device. IoT Edge will pick up on this configuration change and deploy the container to the device.

```
az iot edge set-modules --device-id [device name] --hub-name [hub name] --content "C:\Labs\Content\src\IoTLabs.IoTEdge\deployment.template.lab04.win-x64.json"
```

Run the following command to get information about the modules deployed to your IoT Hub.
```
az iot hub module-identity list --device-id [device name] --hub-name [hub name]
```

### 4.3 - Verify the deployment on IoT device

The module deployment is instant, however changes to the device can take around 5-7 minutes to take effect. On the **target device** you can inspect the running modules with the following command in the remote PowerShell terminal:

```powershell
iotedge list
```

Once the modules have deployed to your device, you can inspect that the module is operating correctly:

```powershell
iotedge logs [container-name]
```


### 4.4 - Monitor Device to Cloud messages

1. Switch back to your development machine
1. Enter the following command in powershell to monitor Device-to-Cloud (D2C) messages being published to the IoT Hub, you should see the events change as you move the camera to face the up2 device.

```
az iot hub monitor-events -n [hub name] -d [device id]
```
