# Lab 05 - Machine Learning at the Edge


For this lab, we will use the Azure Custom Vision service to train a machine learning model for image classification. We will use that model to create a .NET application to pull frames from a connected USB camera, use Windows ML to classify the image, then send the result to Azure IoT Hub. We will deploy that application to a Windows IoT Core device using Azure IoT Edge. Finally, we will visualize the results using Time Series Insights.

## Ready to go

When starting the lab, you should have these things open on your development machine:
1. These instructions
1. Visual Studio Code
1. [Custom Vision Portal](https://www.customvision.ai/) open in a browser tab, and log in with your Azure Subscription. Select the Directory associated with your Azure custom vision resource. 
1. [Time Series Insights explorer](https://insights.timeseries.azure.com/) in another browser tab, also logged in

# Part 1: Custom Vision

## Prepare your Custom Vision Environment
1. Log into the Azure Portal
1. Type "Custom Vision" into the search bar at the top, and click on the "Custom Vision" entry below "Marketplace"
1. Enter a name for this custom vision resource : eg lab05-yourname
1. Choose a location closest to you from the location drop down list
1. Choose S0 for both pricing tiers
1. Select the same resource group you have been using prior to this lab
1. Click create

## Step 1: Train a model

1. Plug the USB camera into your development PC.
1. Using the Camera app on your development PC, take at least 5 pictures each of your objects. Store these pictures on your computer. Organize all the photos for each object into a folder named for this object. It will make them easier to upload.
1. Log into the [Custom Vision Portal](https://www.customvision.ai/)
1. Choose the Directory associated with your Azure account
1. Create a New Project. Be sure to choose a "compact" domain and *Classification* not Object Detection
1. Upload them to your custom vision project. I recommend to upload one object at a time, so it's easy to apply a tag to all your images. Each time you upload all the images for a given object, specify the tag at that time.
1. Select "Train" to train the model
1. Select "Quick Test" to test the model.
. Using the camera app on your PC, take one more picture of one of the objects
1. Upload the picture you took, verify that it was predicted correctly.
1. Export the model. From the "Performance" tab, select the "Export" command.
1. Choose "ONNX", then "ONNX1.2" version.
1. After downloading, rename the file "CustomVision.onnx"

## Step 2: Package the model into a C# .NET Application

### Get the code

If you are running this lab in an environment where this has already been set up, the initial sample will already be present in the directory "C:\Labs\Content\src\IoTLabs.CustomVision". Otherwise, refer to the code distributed with this documentation.

### Get your model file

Copy the CustomVision.onnx model file from your downloads directory into the lab directory "C:\Labs\Content\src\IoTLabs.CustomVision", overwrite the existing onnx file.

### Build & Test the sample

```
dotnet restore -r win-x64
dotnet publish -c Release -o ./release -r win-x64
```

Point the camera at one of your objects, still connected to your development PC.

Run the sample locally to classify the object. This will test that the app is running correctly locally. We specify "Lifecam" for this model of camera. Here we can see that a "Mug" has been recognized.

```
dotnet run --model=CustomVision.onnx --device=LifeCam
4/24/2019 4:09:04 PM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/24/2019 4:09:04 PM: ...OK 594 ticks
4/24/2019 4:09:05 PM: Running the model...
4/24/2019 4:09:05 PM: ...OK 47 ticks
4/24/2019 4:09:05 PM: Recognized {"results":[{"label":"Mug","confidence":1.0}],"metrics":{"evaltimeinms":47,"cycletimeinms":0}}
```

## Step 3: Run the code on our IoT Core Device

### Connect to our IoT Core device

We will need a way to copy files to our device and a Windows PowerShell window from our development PC connected to that device.

First, we will map the Q: drive to our device so we can access files. 

You'll need the Device IP Address. To get the IP Address open the "IoT Dashboard" from the desktop of your surface and select "My Devices".
 
The name of your device is written on the underside of the case in white.
Right click on your device and select "Copy IPv4 Address".

```
net use q: \\[ENTER YOUR DEVICE IP ADDRESS HERE]\c$ p@ssw0rd /USER:Administrator
The command completed successfully.
```

### Copy published files to target device

Next copy the 'publish' folder over to our device

```
robocopy .\release\ q:\data\modules\customvision
```

### Test the sample on the target device

Following the same approach as above, we will run the app on the target device to ensure we have the correct camera there, and it's working on that device.

1. Connect the camera to the IoT Core device.
1. In the Windows PowerShell window to the IoT Core device. (Use the IoT Core Dashboard to connect to the remote powershell)
1. Change to the "C:\data\modules\customvision" directory
1. Do a test run as we did previously:

```
[192.168.X.X]: PS C:\Data\Users\Administrator\Documents> cd C:\data\modules\customvision

[192.168.X.X]: PS C:\data\modules\customvision> .\WindowsAiEdgeLabCV.exe --model=CustomVision.onnx --device=LifeCam
4/27/2019 8:31:31 AM: Loading modelfile 'CustomVision.onnx' on the CPU...
4/27/2019 8:31:32 AM: ...OK 1516 ticks
4/27/2019 8:31:36 AM: Running the model...
4/27/2019 8:31:38 AM: ...OK 1953 ticks
4/27/2019 8:31:42 AM: Recognized {"results":[{"label":"Mug","confidence":1.0}],"metrics":{"evaltimeinms":1953,"cycletimeinms":0}}
```

# Step 4 : Deploy to IoT Edge
Using the skills you learned in [Lab 04](./labs/Lab04.md) from Step 3.2 onwards, redeploy this solution to the Up Squared using IoT Edge.


# Part 2 - XGboost for Sensor Data

## Introduction to XGboost

XGBoost is a machine learning framework for fitting classification and regression tasks on tabular data. 

This model works by iteratively building decision trees that correct for one anothers error and collectively model interdependencies in the data in a way that a singlur tree cannot.

The training process demonstrated today solves a regression problem, but this can be adapted to forecasting, prediction, and classification wth minimal changes.

## Step 1 - Training a model

In VSCode navigate to src\IoTLabs.MachineLearning and open the XGBoost_to_ONNX.py file, this will open a notebook style script.

You can run cells of this notebook by pressing *Shift+Enter*, the output of each cell will appear in the right hand side of the code window.

An explanation of each step of the script is as follows:

1. Data importing with pandas
	* We downloaded this data from the [UCI Dataset Depository](http://archive.ics.uci.edu/ml/datasets/Combined%20Cycle%20Power%20Plant)
	* We import our dataset and ignore the column headers, normally we would keep the headers but in our case this will cause issues when exporting the model
	* We also specify the lack of an index as there are no line numbers in this file and Pandas will add them for us.
1. Data pre-processing 
	* Separation of target value from features means taking the last column of the data and making it a separate dataframe.
	* In order to verify our model accuracy we split our data into one set with 80% of the data for training, and another set with 20% of the data for testing
	* We also export the Testing data to a CSV to validate the deployment of our model to the device later on.
1. Specify model and parameters
	* We define our model as an XGBRegressor model from the [XGBoost SKLearn API](https://xgboost.readthedocs.io/en/latest/python/python_api.html#module-xgboost.sklearn), this ensures our model is compatible with our export tool.
	* There are many tunable parameters for this model type, experiment with the settings you choose and try to improve the accuracy over the results of the default settings (we are printing out the size and frequency of errors, so lower is better).
1. Tune model
	* By calling model.fit() with our data we execute the training process and generate our model, the performance of the model on the testing data is printed in the cell below so that we can monitor the performance.
1. Export model to ONNX file
	* We first convert our XGBoost model into the ONNX format to allow the model to run in WinML.
	* After checking that the conversion is successful we save the model and are now ready to deploy it to the device.

*Go through the entire script before proceeding.*

## Step 2 - Use Converted Model in an Application

### 2.1 - Add model to UWP app
1. Once we have exported the model it will already be in the right file to build the app.
1. We build and deploy the app with the same method used in the previous part of this Lab.

```
cd "C:\Labs\Content\src\IoTLabs.MachineLearning"
dotnet restore -r win-x64
dotnet publish -c Release -o ./release -r win-x64
```

We run the app locally on our test data with the following command. 
* To stop the execution push "Ctrl + C" *
```
C:\Labs\src\IoTLabs.MachineLearning>dotnet run --model=model.onnx --csv="test_data.csv"
Loading model from: 'model.onnx', Exists: 'True'
14-May-19 10:48:16 AM: Loading modelfile 'model.onnx' on the CPU...
14-May-19 10:48:17 AM: ...OK 812 ticks
Loaded 9568 row(s)
System.Collections.Generic.List`1[WindowsAiEdgeLabTabular.DataRow]
14-May-19 10:48:17 AM: Running the model...
14-May-19 10:48:17 AM: ...OK 125 ticks
14-May-19 10:48:18 AM: Recognized {"result":481.429626,"metrics":{"evaltimeinms":125,"cycletimeinms":0}}
```

## Step 3: Build and push a container

### 3.0 - Containerize the sample app 
The following steps assume that you have created a Azure Container Registry in the previous lab.

1.  Publish the executables into a folder named release by running the following command:

```
dotnet publish -c Release -o ./release -r win-x64
```

2. Then enter the name of your container image (note: the number value after the colon denotes the version, increment this every time you make a change)
```powershell
#SAMPLE: aiedgelabcr (this is the same container registry created in lab 04)
$registryName = "[azure-container-registry-name]"
$version = "1.0"
$imageName = "tabularmodel"

$containerTag = "$registryName.azurecr.io/$($imageName):$version-x64-iotcore"
docker build . -t $containerTag
```


### 3.1 - Push container to ACR

1. Run the following commands to login and upload the container into Azure:

```powershell
az acr login --name $registryName
docker push $containerTag
```

## 4.0 - Deploy IoT Edge Modules

### 4.1 - Author a deployment.json file

Now that we have a container image with our inferencing logic stored in our container registry, it's time to create an Azure IoT Edge deployment to our device.

1. Go to "C:\Labs\Content\src\IoTLabs.IoTEdge"
1. Edit the "deployment.template.lab05.win-x64.json" file
1. Search for any variables starting with ACR and replace those values with the correct values for your container repository. The ACR_IMAGE must exactly match what you pushed, e.g. aiedgelabcr.azurecr.io/tabularmodel:1.0-x64-iotcore

**Hint: you can type $containerTag to get the full container string from PowerShell.**


### 4.2 - Deploy the IoT Edge deployment.json file. 

Just like the example deployment, use the following syntax to update the expected module state on the device. IoT Edge will pick up on this configuration change and deploy the container to the device.

```
az iot edge set-modules --device-id [device name] --hub-name [hub name] --content "C:\Labs\Content\src\IoTLabs.IoTEdge\deployment.template.lab05.win-x64.json"
```

Run the following command to get information about the modules deployed to your IoT Hub.
```
az iot hub module-identity list --device-id [device name] --hub-name [hub name]
```
### Verify the deployment on device

Wait a few minutes for the deployment to go through. On the target device you can inspect the running modules. Success looks like this:

```
[192.168.1.102]: PS C:\data\modules\customvision> iotedge list
NAME             STATUS           DESCRIPTION      CONFIG
customvision     running          Up 32 seconds    aiedgelabcr.azurecr.io/tabularmodel:1.0-x64-iotcore
edgeAgent        running          Up 2 minutes     mcr.microsoft.com/azureiotedge-agent:1.0
edgeHub          running          Up 1 second      mcr.microsoft.com/azureiotedge-hub:1.0
```

Once the modules are up, you can inspect that the "tabularmodel" module is operating correctly:

```
[192.168.1.102]: PS C:\data\modules\customvision> iotedge logs tabularmodel
```

Finally, back on the development machine, we can monitor device to cloud (D2C) messages from VS Code to ensure the messages are going up.

1. In VS Code, open the "Azure IoT Hub Devices" pane. 
1. Locate the device there named "ai-edge-lab-device". 
1. Right-click on that device, then select "Start monitoring D2C message".
1. Look for inferencing results in the output window.

Once you see this, you can be certain the inferencing is happening on the target device and flowing up to the Azure IoT Hub.
