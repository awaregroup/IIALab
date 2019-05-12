# Lab 03 - Azure Device Provisioning Service (DPS)

## Pre-requisites
### Hardware
* Arrow Dragonboard 410c
* Grove LED
* Grove Button

### Software
* Visual Studio 2019 Community Edition (or above)


## 1 - Introduction to Azure DPS

### 1.1 - Deploying Azure IoT Hub

### 1.2 - Deploying Azure DPS

### 1.3 - Deploying Azure Device Management (DM) Agent

### 1.4 - Introduction to Device Twins



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
