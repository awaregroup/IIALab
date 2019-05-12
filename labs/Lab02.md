# Lab 03 - Azure IoT Hub

## Pre-requisites
### Hardware
* Arrow Dragonboard 410c
* Grove LED
* Grove Button

### Software
* Visual Studio 2019 Community Edition (or above)


### 1.1 - Deploying Azure IoT Hub

## Azure IoT Hub

1. Sign into the Azure Portal
1. Create a new "IoT Hub" resource by typing "IoT Hub" in the search bar at the top of the Azure Portal and choosing the entry under "Marketplace"
1. Choose your existing resource group and the West US region. The name should contain the number in your username. eg: "msiotlabs-user01"
1. Click "Review and Create"
1. Once created, open the newly created IoT Hub in the resource group you selected.
1. Now, we will create a device. Switch to the "IoT Devices" pane and choose "Add"
1. Enter a name for this device and then click "Save"
1. Push Refresh then click on your newly created devices
1. Copy the Connection String (Primary Key) by clicking on the blue copy box. Save this as you will need it later.

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
