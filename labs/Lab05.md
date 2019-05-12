# Lab 05 - Machine Learning at the Edge


For this lab, we will use the Azure Custom Vision service to train a machine learning model for image classification. We will use that model to create a .NET application to pull frames from a connected USB camera, use Windows ML to classify the image, then send the result to Azure IoT Hub. We will deploy that application to a Windows IoT Core device using Azure IoT Edge. Finally, we will visualize the results using Time Series Insights.


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

TODO: Instructions on using the source from the previous lab

## Get your model file

Copy the CustomVision.onnx model file from your downloads directory where you exported it into the lab directory.

## Build & Test the sample

TODO: You may need to delete the publish directory first as the ONNX file isn't always overridden, this is a repeat of what was in Lab04 but with a custom model.

```
PS C:\WindowsAiEdgeLabCV> dotnet publish -r win-x64
```

Point the camera at one of your objects, still connected to your development PC.

Run the sample locally to classify the object. This will test that the app is running correctly locally, you should see the end evaluation of of your custom vision model.

```
PS C:\WindowsAiEdgeLabCV> dotnet run --model=CustomVision.onnx --device=LifeCam
```

## Deploy the solution to IoT Edge

**TODO : Get users to go back to Lab04 and redeploy the solution. Of note you want to mention to tag the container with a /DIFFERENT/ TAG**

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
2. In the left menu, select All resources. Select your Time Series Insights environment.
3. Under Environment Topology, select Event Sources, and then select Add.
4. In the New event source pane, for Event source name, enter a name that's unique to this Time Series Insights environment. For example, enter event-stream.
5. For Source, select IoT Hub.
6. Select the "Use IoT Hub from Avaliable Subscriptions" dropdown
7. Add the dedicated Time Series Insights consumer group name that you added to your IoT hub.
8. Select Create.
9. After you create the event source, Time Series Insights automatically starts streaming data to your environment.
