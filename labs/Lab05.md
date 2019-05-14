# Lab 05 - Machine Learning at the Edge


For this lab, we will use the Azure Custom Vision service to train a machine learning model for image classification. We will use that model to create a .NET application to pull frames from a connected USB camera, use Windows ML to classify the image, then send the result to Azure IoT Hub. We will deploy that application to a Windows IoT Core device using Azure IoT Edge. Finally, we will visualize the results using Time Series Insights.

## Ready to go

When starting the lab, you should have these things open on your development machine:

1. These instructions
1. Visual Studio Code
1. [Custom Vision Portal](https://www.customvision.ai/) open in a browser tab, and logged in with your Azure Subscription. Select the Directory associated with your Azure custom vision resource. 
1. [Time Series Insights explorer](https://insights.timeseries.azure.com/) in another browser tab, also logged in

# Part 1: Custom Vision

## Step 1: Train a model

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

## Step 2: Package the model into a C# .NET Application

### Get the code

If you are running this lab in an environment where this has already been set up, the initial sample will already be present in the directory "C:\Labs\Content\src\IoTLabs.CustomVision". Otherwise, refer to the code distributed with this documentation.

### Get your model file

Copy the CustomVision.onnx model file from your downloads directory where you exported it into the lab directory.

### Build & Test the sample

```
dotnet restore -r win-x64
dotnet publish -c Release -o ./release -r win-x64
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

## Step 3: Run the code on our IoT Core Device

### Connect to our IoT Core device

We will need a way to copy files to our device and a Windows PowerShell window from our development PC connected to that device.

First, we will map the Q: drive to our device so we can access files. 

You'll need the Device IP Address. To get the IP Address open the "IoT Dashboard" from the desktop of your surface and select "My Devices".
 
The name of your device is written on the underside of the case in white.
Right click on your device and select "Copy IPv4 Address".

```
$DeviceIPAddress = [ENTER YOUR IP ADDRESS HERE]
net use q: \\$DeviceIPAddress\c$ p@ssw0rd /USER:Administrator
The command completed successfully.
```

### Copy published files to target device

Next copy the 'publish' folder over to our device

```
PS C:\WindowsAiEdgeLabCV> robocopy .\release\ q:\data\modules\customvision
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

# Part 2 - XGboost for Sensor Data

## Introduction to XGboost

XGBoost is a machine learning framework for fitting classification and regression tasks on tabular data. 

This model works by iteratively building decision trees that correct for one anothers error and collectively model interdependencies in the data in a way that a singlur tree cannot.

The training process demonstrated today solves a regression problem, but this can be adapted to forecasting, prediction, and classification wth minimal changes.

## Step 1 - Training a model

1. Open Jupyter Notebook
	* Open Visual Studio Code and install the python extension by selecting extensions in the left hand menu and typing python in the search
	* Navigate to the lab content and select the 'XGBoost_to_ONNX.py' file, this will open a notebook style code window
	* You can run a cell and jump to the next one by pressing 'Shift + Enter'
1. Data importing with pandas
	* We downloaded the Combined Cycle Power Plant data from the [UCI Dataset Depository](http://archive.ics.uci.edu/ml/datasets/Combined%20Cycle%20Power%20Plant)
	* We import our dataset and ignore the column headers, normally we would keep the headers for analysis, but in this case they will cause issues when exporting the model
	* We also specify the lack of an index as there are no line numbers in this file and Pandas will add them for us automatically.
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


## Step 2 - Integration into TSI

### 2.1 - Add the model to the C# app
1. Open the UWP app included in this the 'src' folder of this repo titled "IoTLabs.MachineLearning" 
1. Drag and drop the "model.onnx" and "test_data.csv" files we just exported in the previous step into main folder of the project
1. build the app by running:
	'''dotnet restore -r win-x64
	dotnet publish -r win-x64'''
1. If there are no build errors your app it now ready to run, test that it works locally by running:
	'''dotnet run --model=model.onnx --csv=test_data.csv'''

### 2.2 - Add a consumer group to your IoT hub
1. Applications use consumer groups to pull data from Azure IoT Hub. Provide a dedicated consumer group that's used only by this Time Series Insights environment to reliably read data from your IoT hub.
1. To add a new consumer group to your IoT hub:
1. In the Azure portal, find and open your IoT hub.
1. In the menu, under Settings, select Built-in Endpoints, and then select the Events endpoint.
1. Under Consumer groups, enter a unique name for the consumer group. Use this same name in your Time Series Insights environment when you create a new event source.
1. Select Save.

### 2.3 - Add a new event source
1. Sign in to the Azure portal.
2. In the left menu, select All resources. Select your Time Series Insights environment.
3. Under Environment Topology, select Event Sources, and then select Add.
4. In the New event source pane, for Event source name, enter a name that's unique to this Time Series Insights environment. For example, enter event-stream.
5. For Source, select IoT Hub.
6. Select a value for Import option:
	* If you already have an IoT hub in one of your subscriptions, select Use IoT Hub from available subscriptions. This option is the easiest approach.
	* If the IoT hub is external to your subscriptions, or if you want to choose advanced options, select Provide IoT Hub settings manually.
7. The following table describes the required properties for the Provide IoT Hub settings manually:


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

8. Add the dedicated Time Series Insights consumer group name that you added to your IoT hub.
9. Select Create.
10. After you create the event source, Time Series Insights automatically starts streaming data to your environment.
