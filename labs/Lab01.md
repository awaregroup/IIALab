# Lab 01 - Getting started with Windows IoT

This lab covers setting up Windows 10 IoT Core on a SolidRun HummingBoard Edge board and deploying apps using Visual Studio.

## Pre-requisites
### Hardware
* SolidRun HummingBoard Edge
* ?

### Software
* Visual Studio 2019 Community Edition (or above)

## 1 - Installing Windows 10 IoT Core with IoT Dashboard

Devices running Windows 10 IoT Core can be installed and configured using the IoT Dashboard. This tool makes it simple to get started and also provision Azure connectivity.

### 1.1 - Installing IoT Dashboard

1. Open a browser window to: [http://go.microsoft.com/fwlink/?LinkID=708576](http://go.microsoft.com/fwlink/?LinkID=708576)
1. Launch the IoT Dashboard and verify the application starts correctly

![IoT Dashboard](./media/1_iotdashboard.png)

### 1.1 - Installing Windows 10 IoT Core

1. Download the HummingBoard Edge i.MX6 Quad IoT Core FFU from [https://images.solid-build.xyz/IMX6/Windows%2010%20IoT/HB_Edge_Quad.ffu](https://images.solid-build.xyz/IMX6/Windows%2010%20IoT/HB_Edge_Quad.ffu)
1. Insert Micro SD card into host PC 
1. Open IoT Dashboard and click 'Setup a new device'
1. Change the device type to 'NXP [i.MX6/i.MX7/i.MX8]' and set the OS Build to 'Custom'
1. Browse to the FFU file downloaded earlier
1. Accept the license agreement and click 'Install'

![IoT Dashboard](./media/1_iotdashboard2.png)


### 1.2 - Validating your install

1. Once the HummingBoard has completed installing, a line entry will show in the IoT Dashboard as above
2. Right click on your device and select 'Device Portal' - **Note: if your device doesn't show, in the list, read the IP Address from the display and enter that in your browser on port 8080. For example: http://192.168.88.200:8080**
3. In your browser enter the default username and password:

|Name    |Value|
|--------|-----|
|Username|Administrator|
|Password|p@ssw0rd|

![Device Portal](./media/1_deviceportal1.png)

4. Enter a name in the 'Change your device name' text box and click 'Save'. Click yes to reboot to display the new name 



## 2 - Deploying apps to HummingBoard

### 2.1 - Hello world

1. Open up the first lab project found in [C:\Labs\content\src\IoTLabs.HummingBoard\IoTLabs.HummingBoard.sln](file:///C:\Labs\content\src\IoTLabs.HummingBoard.sln) 
2. Update the target system architecture to say 'ARM' as shown in the image below
![](./media/1_vs3.png)
3. Change target system from 'Local' to 'Remote Machine' and enter the IP address of your device. This can be found on the screen of your device
![](./media/1_vs2.png)
4. Run the project to test it on your HummingBoard. You should see an interface, however no data should be showing - **Note: the first deployment can take a few minutes**


## 3 - Publishing your app

1. In Visual Studio, in the Solution Explorer tab on the right, click on MainPage.xaml 
2. Click ```Project > Store > Create App Packages...```
![](./media/1_createapppackages.png)
3. Choose "I want to create packages for sideloading" and uncheck "Enable automatic updates"
![](./media/1_createapppackages4.png)
4. Select "Never" on "Generate app bundle" and select "ARM" as the Architecture 
![](./media/1_createapppackages2.png)
5. After package creation, click on the link to verify the .appx files have been created
![](./media/1_createapppackages5.png)


Once you've confirmed the appx file has been created, you can move onto the next lab: [2 - Integrating Windows IoT with Azure](./Lab02.md)

