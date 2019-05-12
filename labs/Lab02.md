# Lab 02 - Azure IoT Hub

## Pre-requisites
### Hardware
* Arrow Dragonboard 410c
* Grove LED
* Grove Mini PIR Sensor
* Grove Barometer Sensor

### Software
* Visual Studio 2019 Community Edition (or above)


## 1 - Deploying Azure IoT Hub

### 1.1 - Deploy Azure IoT Hub

1. Sign into the [Azure Portal (https://portal.azure.com)](https://portal.azure.com) with the supplied lab credentials
1. Click "Create a resource", search for "IoT Hub"
![](./media/2_azure1.png)
1. Click on "IoT Hub" and "Create"
![](./media/2_azure2.png)
1. Fill out the details by choosing a name and select the existing subscription and resource group
![](./media/2_azure3.png)
1. Click "Review + create" and finally "Create" to complete the provisioning
![](./media/2_azure4.png)

### 1.2 - Configure Azure IoT Hub

1. Click "Resource groups" on the left-hand menu, select the "winiot" resource group in the list and choose the IoT Hub previously created
![](./media/2_azure5.png)
1. Click "IoT devices" on the IoT Hub menu and click "Add"
![](./media/2_azure6.png)
1. Enter "dragonboard" as the Device ID and click "Save" to create the device
![](./media/2_azure7.png)

 Now, we will create a device. Switch to the "IoT Devices" pane and choose "Add"
1. Enter a name for this device and then click "Save"
1. Push Refresh then click on your newly created devices
1. Copy the Connection String (Primary Key) by clicking on the blue copy box. Save this as you will need it later.

## TODO : The rest of the lab

## 3 - Integration into TSI

## Azure Time Series Insights

Refer to this guide: [Add an IoT hub event source to your Time Series Insights environment](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-how-to-add-an-event-source-iothub)

1. Sign into the Azure Portal
1. Create a new "Time Series Insights environment" resource.
1. Enter a unique name for this resource, include the number in your user acocunt name at the end. For example : "msiotlabs-01"
1. Choose the "S1" pricing tier.
1. Choose "Next: Event Source"
1. For the event source, choose the existing Azure IoT Hub you configured above.
1. For IoT Hub access policy, choose "iothubowner". 
1. For "consumer group", click the "New" button then enter a unique name to use as the consumer group for events then *push "Add" before continuing*
1. Click "Review & Create" then create the resource.
