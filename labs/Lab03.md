# Lab 03 - Configure your cloud solution platform

## 1 - Deploy and configure Azure IoT components

### 1.0 - Provision Azure Resources

The focal point of all Microsoft IoT solutions is the Azure IoT Hub service. IoT Hub provides secure messaging, device provisioning and device management capabilities.

Azure Resource Manager Templates (ARM Templates) can be deployed into Azure that include IoT Hub. 

1. Click the button below to create the Azure IoT components required for the next labs:<br/><br/>
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fawaregroup%2FIIALab%2Fmaster%2Fsrc%2FAzure.ARM%2Fiia-azuredeploy.json" target="_blank" rel="noopener noreferrer">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fawaregroup%2FIIALab%2Fmaster%2Fsrc%2FAzure.ARM%2Fiia-azuredeploy.json" target="_blank" rel="noopener noreferrer">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.png"/>
</a><br/><br/>
You can also visualize an ARM template to see the components that will be created.

2. Choose the Resource Group that matches your lab user number, click 'Accept' on the terms and 'Purchase' to begin the provisioning process.
![](./media/3_1.png)

3. Wait for the provisioning process to complete.

### 1.1 - Validate Resources

1. On the left hand icon menu, click 'Resource Groups'
![](./media/3_2.png)
2. Click on the resource group that corresponds to your username
![](./media/3_3.png)
3. Validate that you can see the following types of resources:<br/>
* Device Provisioning Service
* IoT Hub
* Storage Account
* Stream Analytics Job

These components represent the IoT platform your device connects to.


## 2 - Validate Resource Configuration

### 2.1 - IoT Hub

IoT Hub is the core of all IoT projects in Azure. Click on your IoT Hub resource and explore the different pages. 


![](./media/3_4.png)


|Component Name    |Notes|
|--------|-----|
|Access Policies|IoT Hub has a specific focus on security and this is one of the areas to configure access to the management of the IoT Hub. |
|IoT Devices|This device list allows you to see all the devices that are currently registered against the IoT hub and manage them. You can also check the metadata for each device including their Device Twin.|
|IoT Edge|This is an important component for the labs further on. This allows you to manage your IoT Edge devices in a similar fashion to the IoT Devices.|
|Message Routing|Core to IoT Hub is a messaging platform - the ability to send messages from the Device-to-Cloud and Cloud-to-Device. Message routes allow you to forward device messages to other Azure services. There is a route configured in this solution that allows the telemetry to flow through to Time Series Insights.

