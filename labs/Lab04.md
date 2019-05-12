# Lab 04 - Introduction to Azure IoT Edge

## 1 - Install Windows 10 IoT Core on UP2

1. Boot UP2 using USB provided in the lab
2. Type the following to install a pre-prepared FFU designed for the UP2:

```batch
d:\
.\eMMCInstaller.cmd
```
3. Reboot device by typing ```wpeutil reboot``` and verify Windows starts up 

## 2 - Deploying containers

### 2.1 - Deploying Azure Device Provisioning Service

#### 1.1 - Getting unique device identifiers

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

### Deploy the mock temperature sensor monitor deployment

TODO : Introduction to deployment.json, expected state for IoT Edge, as well as the integrations with the IoT Hub. Location of the example deployment.

```
az login
az iot edge set-modules --device-id [device id] --hub-name [hub name] --content deployment.example.json
```

### 4.3 - Monitor Device to Cloud messages

TODO : Use Azure CLI
1. In Visual Studio Code, open the 'Azure IoT Hub Devices' pane  
1. Right-click your device and 'Start monitoring D2C messages'
1. You should see the mock temperature sensor data start to flow into the IoT Hub.


### Build and deploy a custom model that detects faces (from the ONNX model zoo) using existing software over IoT Edge

#### Grab the C# and such for any vision model we provide and run it on the PC

We can use our own model, download the C# code and run it, build the docker container 

#### Set up and deploy Azure Container Registry : TODO Flesh this out

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

Run the following command to get information about the modules deployed to your IoT Hub.
```
az iot hub module-identity list --device-id $deviceId --hub-name $iotHub
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
