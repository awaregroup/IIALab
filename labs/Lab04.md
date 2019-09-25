# Lab 04 - Introduction to Azure IoT Edge

This lab introduces Azure IoT Edge with Windows 10 IoT Core.

## 1.0 - Set up your Surface Laptop Device

### 1.1 - Cloud setup

1. Make a note of the Surface Laptop device name printed on the device. For example, IOTEDGE02 
1. Open a browser and navigate to the [Azure Portal (https://portal.azure.com)](https://portal.azure.com). Log in with the lab credentials provided.
1. Click **Resource groups** on the left-hand menu, select the **winiot** resource group in the list and choose the IoT Hub created in [Lab 2](./Lab02.md#11---deploy-azure-iot-hub)
![](./media/2_azure5.png)
1. Click **IoT Edge** on the IoT Hub side menu and click **Add an IoT Edge device** at the top. **Note: that this is a slightly different menu than the one used earlier in the lab**
![IoT Hub Portal](./media/4_SelectIoTEdge.png)
1. Enter the Surface Laptop name (from earlier) as the device id and click **Save** to create the device
1. Refresh the list and open the device properties
1. Copy **Connection string (primary key)** to the clipboard
![IoT Edge Device Information](./media/4_CopyConnectionStringIoTEdge.png)


### 1.2 - IoT Device setup using Azure CLI
1. On your Surface Laptop, open powershell as Administrator
2. Install the Azure IoT Edge runtime on the device by running the following command and wait for the device to reboot:
```powershell
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Deploy-IoTEdge
```
3. Re-open the remote PowerShell session as Administrator 
4. Configure the Azure IoT Edge runtime with the following command:
```powershell
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Initialize-IoTEdge
```
5. Enter the Device Connection string from the previous step: 
6. To validate the Azure IoT Edge runtime installation, use the command:
```powershell
iotedge check
``` 

## 2.0 - Deploy Simulated Temperature Sensor

### 2.1 - Module deployment using Azure CLI

1. Open PowerShell as Administrator:
1. Run the following commands replacing [devicename] and [hub name] with their respective fields:

```powershell
az extension add --name azure-cli-iot-ext

az login
az iot edge set-modules --device-id [device name] --hub-name [hub name] --content "C:\Labs\Content\src\IoTLabs.IoTEdge\deployment.example.win-x64.json"
```

### 2.2 - Monitor Device-to-Cloud messages
1. Enter the following command to monitor Device-to-Cloud (D2C) messages being published to the IoT Hub:

```powershell
az iot hub monitor-events -n [hub name] -d [device id]
```
## 3.0 - Configure Azure Stream Analytics Edge Job
### 3.1 - Navigate to your Azure Stream Analytics Edge Job
1. In the [Azure Portal (https://portal.azure.com)](https://portal.azure.com) open the **winiot** resource group
1. Open the **Stream Analytics job** resource
![Stream Analytics Job](/media/lab04/asa-overview.jpg)

### 3.2 - Adding Inputs
1. Under the **Job topology** heading in the left hand menu, select **Inputs**
1. Select **Add stream input**, then select **Edge Hub**
1. Set the **Input Alias** as **temperature** and leave the rest of the settings as default.
1. Click **Save**

### 3.3 - Adding Outputs
1. Under the **Job topology** heading in the left hand menu, select **Outputs**
1. Select **Add**, then select **Edge Hub**
1. Set the **Output Alias** as **alert** and leave the rest of the settings as default.
1. Click **Save**

### 3.4 - Adding Query
1. Under the **Job topology** heading in the left hand menu, select **Query**
1. Replace the current query with the following:
```sql
SELECT  
    'reset' AS command 
INTO 
   alert 
FROM 
   temperature TIMESTAMP BY timeCreated 
GROUP BY TumblingWindow(second,30) 
HAVING Avg(machine.temperature) > 70
```
1. Click **Save query**

### 3.5 - Adding Storage Account
1. Under the **Configure** heading in the left hand menu, select **Storage account settings**
1. Click **Add storage account**
1. Leave the toggle set to **Select storage account from your subscriptions** and 
1. Set **Container** to **Create new** and enter a name in the box.
1. Click **Save**

## 4.0 - Configure IoT Edge to use Azure Stream Analytics Edge Job
### 4.1 - Module deployment using Azure Portal
1. In the [Azure Portal (https://portal.azure.com)](https://portal.azure.com) open the **winiot** resource group
1. Open the **IoT Hub** resource, navigate to **IoT Edge** and then select the device created in [step 1.1](#11---cloud-setup)
![IoT Edge Devices](/media/lab04/iot-edge-devices.jpg)
1. Click **Set modules**
![Set Modules](/media/lab04/set-modules.jpg)
1. Under the **Deployment Modules** heading click **+ Add** and choose **Azure Stream Analytics Module**
![Adding ASA Module](/media/lab04/add-asa-module.jpg)
1. Set the Subscription as **MSIoTLabs-IIA** and Edge Job as **msiotlabs-iia-[user]-asa**, then click **Save**
*Note: You may have to click on the **Edge job** dropdown for the save button to show.*
1. When the module has loaded, select **Configure** and take note of the **Name** field. You will be using this module name in the next step.
1. Click **Save**, then **Next**

### 4.2 - Selecting the routes
1. Replace the current JSON with the following, substituting [module name] with the module name found in the previous step:

```javascript
{
    "routes": {
        "telemetryToCloud": "FROM /messages/modules/SimulatedTemperatureSensor/* INTO $upstream",
        "alertsToCloud": "FROM /messages/modules/[module name]/* INTO $upstream",
        "alertsToReset": "FROM /messages/modules/[module name]/* INTO BrokeredEndpoint(\"/modules/SimulatedTemperatureSensor/inputs/control\")",
        "telemetryToAsa": "FROM /messages/modules/SimulatedTemperatureSensor/* INTO BrokeredEndpoint(\"/modules/[module name]/inputs/temperature\")"
    }
}
```
2. Select **Next**, then **Submit**

### 4.3 - Verify deployment on the IoT device
The module deployment is instant, however changes to the device can take around 5-7 minutes to take effect. Let's check that our device has loaded our Azure Stream Analytics module from the last step.

1. Open Powershell as Administrator
1. Inspect the currently running modules using the following command:
```powershell
iotedge list
```
![List Modules](/media/lab04/list-modules.jpg)
3. You can also see a list of logs for your module using the following command:
```powershell
iotedge logs [module name]
```
