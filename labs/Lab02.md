# Lab 02 - Integrating Windows 10 IoT with Azure IoT Central

## Pre-requisites

### Software
* Web Browser

## 1 - Deploying Azure IoT Central (IOTC)

### 1.1 - Create IOTC application

1. Navigate to [apps.azureiotcentral.com](https://apps.azureiotcentral.com) and log in with supplied lab credentials
![](./media/2_iotc1.png)
2. Click "New Application", and choose the following settings:

|Name    |Value|
|--------|-----|
|Payment Plan|Pay-as-you-go|
|Application Template|Preview Application|
|Application Name|[choose a name]|

3. Click "Create" to provision your application


### 1.2 - Create Device Template

1. Navigate to Device Templates and click 'New'
![](./media/2_iotc2.png)

1. IoT Central can import existing Device Templates from the [Azure IoT Device Catalog](), however we are creating our own device. Click 'Custom'.
![](./media/2_iotc3.png)

1. Name your device and click 'Import Capability Model'
![](./media/2_iotc4.png)

1. Browse to the `src/IoT Central/` folder and upload the file named `ST SensorTile.Box.json`.
![](./media/2_iotc5.png)

1. Click 'Views' and click 'Generate Default Views'
![](./media/2_iotc7.png)

1. Click 'Publish' and confirm the process by clicking 'Publish' again.
![](./media/2_iotc6.png)

### 1.3 - Create Device Template

1. Click 'Devices', choose your newly created Device Template and click 'New'.
![](./media/2_iotc8.png)

1. Confirm that the 'Simulated' toggle is **unchecked**, then click 'Create'.

1. Click on your new device to see the device dashboard. There should be no data showing yet.
![](./media/2_iotc9.png)

1. Click the 'Connect' button and record the `Scope ID`, `Device ID` and `Primary Key`. These are the Azure IoT Hub Device Provisioning Service (DPS) details. You will need these to set up your device.
![](./media/2_iotc10.png)

### 1.4 - Monitor IOTC Application

1. Go to settings and generate API Token
1. Connect to IOTC event stream with iotc-explorer tool
1. See simulated events being pushed to IOTC

### 1.4 - Connect HummingBoard to IOTC

1. Add connection string
1. Re-deploy app
1. See telemetry in IOTC


## 4 - Republish appx files

1. Refer to Step 3 in Lab 1 to run through the steps in Visual Studio for publishing packages


With your new appx files you can move on to [Lab 3 - Creating a custom Windows IoT FFU](./Lab03.md)
