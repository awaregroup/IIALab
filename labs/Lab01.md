# Lab 01 - Getting started with Windows 10 IoT

This lab covers setting up Windows 10 IoT Core on a SolidRun HummingBoard Edge board and deploying apps using Visual Studio.

## Pre-requisites
### Hardware
* SolidRun HummingBoard Edge
* STM SensorTile.Box
* USB HDMI Capture Card 

### Software
* Visual Studio 2019 Community Edition (or above) 

## 1 - Installing Windows 10 IoT Core with IoT Dashboard

Devices running Windows 10 IoT Core can be installed and configured using the IoT Dashboard. This tool makes it simple to get started and also provision Azure connectivity.

### 1.1 - Installing IoT Dashboard

1. **If you already have Windows 10 IoT Core Dashboard on your desktop, you can skip this step and move on to 'Installing Windows 10 IoT Core'**
1. Open a browser window to: [http://go.microsoft.com/fwlink/?LinkID=708576](http://go.microsoft.com/fwlink/?LinkID=708576)  This will install a file to setup the data and tools required for the labs.  
1. Double click on the setup.exe file which will launch the IoT Dashboard and verify the application starts correctly (by seeing the screen appear as shown below).

![IoT Dashboard](./media/1_iotdashboardinitial.png)

### 1.2 - Installing Windows 10 IoT Core

1. Ensure the Micro SD card is in the card reader and **not in the HummingBoard**, then plug the card reader into your PC. 
![MicroSD](./media/5_microsd.jpg)
1. Dismiss any messages to format the drive. Close those windows.  
1. Open IoT Dashboard and click **Setup a new device**
1. Using the drop-down list, change the device type to **NXP [i.MX6/i.MX7/i.MX8]** and set the OS Build to **Custom**.
1. Browse to the FFU file found in `C:/Labs`
1. Choose a name for your device like 'Lab-UserXX', where the XX referes to user number assigned to you. 
1. Add a New Administrator password, and confirm. We recommend "p@ssw0rd" as it is used later in the labs. 
1. Accept the license agreement and click **Install**.
1. Windows Explorer may prompt you to format the drive, this message can safely be ignored, press **Continue**.  Allow the software to make changes to your device, if asked.
1. It may take a few minutes for the Micro SD card to provision.

![IoT Dashboard](./media/1_iotdashboard2.png)

### 1.3 - Plugging in your hardware
1. Plug the HDMI adapter into your HummingBoard and the USB end into the USB Hub attached to your PC. 
1. Plug the Micro USB cable into your SensorTile and the other end into the HummingBoard (not your PC).
1. Plug in the ethernet cable to your HummingBoard.
1. Remove the Micro SD Card from the card reader.
1. Insert the Micro SD Card into your HummingBoard.
1. Insert the Power Cable into your HummingBoard. 
1. On your Lab PC, open the Camera application to view the output of your HummingBoard as it boots. 
1. Select the camera cycle button, found in the top right corner of the Camera App, and select the capture card.
1. The HummingBoard will begin its initial setup. Please note that the setup does not require any user input; it will automatically step through.

**Hint:** If you get the order wrong, unplug and plug back in the Power Cable into the HummingBoard for the display to capture.

### 1.4 - Validating your install

1. Once the HummingBoard has completed booting, a line entry will show in the IoT Dashboard.
![Device in IoT Dashboard](./media/lab01/1_validatinginstall.png)
2. Right click on your device and select **Open in Device Portal** 
![Open Device Portal](./media/lab01/1_opendeviceportal.png)


**Note:** if your device doesn't show, in the list, read the IP Address from the display and enter that in your browser on port 8080. For example: http://192.168.88.200:8080
![Open Device Portal](./media/lab01/1_IoTCoreIPAddress.png)

3. In your browser enter the default username and password, that we just set:

|Name    |Value|
|--------|-----|
|Username|Administrator|
|Password|p@ssw0rd|


4. Open the Processes Menu, Select Run command
![Device Portal](./media/1_deviceportal1.png)
5. Type **"devcon status USB\VID_0483\*"** and hit enter to see if the device can see the connected SensorTile
6. You should see Name: USB Serial Device with status of Running
![SensorTile Connected](./media/1_SensorTileConnected.png)
7. Another way to see what is on the screen of the IoT device is to use the inbuilt **Screenshot** command on the Windows Device Portal. 
8. Select **Device Settings** on the Windows Device Portal, in the bottom right press the **Screenshot** button. Try it twice if it shows the broken image icon the first time.
![SensorTile Connected](./media/lab01/1_screenshot.png)   

You should now have a working IoT device with Windows 10 IoT Core installed, along with screen output being shown through the HDMI capture card to the Windows Camera app acting as a second screen, and confirmed that the sensor device is connected as a virtual com port. 


## 2 - Deploying apps to HummingBoard

### 2.1 - Hello world

1. Open up the first lab project found in [C:\Labs\content\src\IoTLabs.TestApp\IoTLabs.TestApp.sln](file:///C:\Labs\content\src\IoTLabs.TestApp\IoTLabs.TestApp.sln)  
2. If prompted to log in with VisualStudio, simply close the window as no sign in is required.
3. Update the target system architecture to say 'ARM' as shown in the image below
![](./media/1_vs3.png)
4. Change target system from 'Device' to 'Remote Machine' and enter the IP address of your HummingBoard device. This can be found on the camera screen showing your HummingBoard device.
![](./media/1_vs2.png)
5. Run the project back in VisualStudio (by pressing the green arrow beside the Remote Machine) to test it on your HummingBoard. If no sensor data is displayed, try changing the USB port the SensorTile is connected through to another port on the HummingBoard.

**Note:** the first deployment can take a few minutes.  You will know it has finished as the camera screen of your HummingBoard will show an interesting dashboard.


## 3 - Publishing your app _(optional)_

1. In Visual Studio, in the Solution Explorer tab on the right, click on Dashboard.xaml 
2. Click ```Project > Publish > Create App Packages...```
![](./media/1_createapppackages.png)v
3. Choose to distribute the application by **Sideloading** and uncheck "Enable automatic updates".  Click **Next** and leave the defaults for the 'Select signing method', click **Next** again.
![](./media/1_createapppackages4.png)
4. Select "Never" on "Generate app bundle" and select "ARM" as the Architecture 
![](./media/1_createapppackages2.png)
5. After package creation, click on the link to verify the .appx files have been created
![](./media/1_createapppackages5.png)


Once you've confirmed the appx file has been created, you can move onto the next lab: [2 - Integrating Windows IoT with Azure](./Lab02.md)

