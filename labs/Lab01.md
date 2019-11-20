# Lab 01 - Getting started with Windows 10 IoT

This lab covers setting up Windows 10 IoT Core on a SolidRun HummingBoard Edge board and deploying apps using Visual Studio.

## Pre-requisites
### Hardware
* SolidRun HummingBoard Edge
* MicroSD Card and Adapter 
* STM SensorTile.Box
* Optional: USB HDMI Capture Card 

### Software
* Visual Studio 2019 Community Edition (or above) 

## 0 - Using GitHub lab documentation

It's a little diffucult to follow documentation on the same device that you are doing the labs on due to switching back and forward from browser tabs. Here are a couple tips that might help. 

### 1. Always open links in new Tab or Window
1. Right click on any links and select **open in new tab** or **open in new window** 

### 2. Scale content and have side by side windows 

1. Press CTRL and + or - (plus or minus buttons) to scale the window. 
1. Open Azure Portal or other non documentation sites in a new browser window and also scale them down too. 
1. 67% scale is about the lowest you can go on a 1920 x 1080 screen depending on screen size.  


## 1 - Installing Windows 10 IoT Core with IoT Dashboard

Devices running Windows 10 IoT Core can be installed and configured using the IoT Dashboard. This tool makes it simple to get started and also provision Azure connectivity.

### 1.1 - Installing IoT Dashboard

1. **If you already have Windows 10 IoT Core Dashboard on your desktop, you can skip this step and move on to 'Installing Windows 10 IoT Core'**
1. Open a browser window to: [http://go.microsoft.com/fwlink/?LinkID=708576](http://go.microsoft.com/fwlink/?LinkID=708576)  This will install a file to setup the data and tools required for the labs.  
1. Double click on the setup.exe file which will launch the IoT Dashboard and verify the application starts correctly (by seeing the screen appear as shown below).

![IoT Dashboard](./media/1_iotdashboardinitial.png)

### 1.2 - Installing Windows 10 IoT Core

1. Ensure your Hummingboard device power is unplugged. 
1. Take the Micro SD card and put it in the card reader. Ensure it's **not in the HummingBoard**, then plug the card reader into your PC. 
![MicroSD](./media/5_microsd.jpg)
1. Dismiss any messages to format the drive. Close all those windows that pop up.  
1. Open IoT Dashboard by typing **iot core** at the start menu
1. Click **Setup a new device**
1. Using the drop-down list, change the device type to **NXP [i.MX6/i.MX7/i.MX8]** and set the OS Build to **Custom**.
1. Browse to the FFU file found in `C:/Labs`
1. Choose a name for your device like 'Lab-UserXX', where the XX referes to user number assigned to you. 
1. Add a New Administrator password, and confirm. We recommend "p@ssw0rd" as it is used later in the labs. 
1. Accept the license agreement and click **Install**.
1. Windows Explorer may prompt you to format the drive, this message can safely be ignored, press **Continue**.  Allow the software to make changes to your device, if asked.
1. It may take a few minutes for the Micro SD card to provision.
1. Once complete, remove the Micro SD card from the adapter

![IoT Dashboard](./media/1_iotdashboard2.png)

### 1.3 - Plugging in your hardware

1. Plug the Micro USB cable into your SensorTile and the other end into the HummingBoard (not your PC).
1. Plug in the ethernet cable to your HummingBoard.
1. Insert the Micro SD Card into your HummingBoard.
1. Plug the HDMI adapter into your HummingBoard and the USB end into the USB Hub attached to your PC. 
![Hardware](./media/lab01/hardware_plugged_in.png)

### 1.4 - Booting your device
1. On your Lab PC, open the Camera application to view the output of your HummingBoard as it boots. 
1. Select the camera cycle button, found in the top right corner of the Camera App, and select the capture card.
1. Insert the Power Cable into your HummingBoard. 
1. The HummingBoard will begin its initial setup. 

**Note:** This initial boot process takes 3-5 minutes. It may reboot a couple times while it processes the initial setup. During boot you will see various screens that may take a minute before they proceed. If you see a Windows logo during this process thats a good sign. Once it reaches the Out of Box Experience (OOBE) you are ready to conitnue. The setup does not require any user input; it will automatically step through. 

**Note:** You can not interact with the IoT Core using your screen, mouse or keyboard. It is just a HDMI display being shown on your Lab PC. 

**Hint:** If you get the order wrong, unplug and plug back in the Power Cable into the HummingBoard for the display to capture.

### 1.5 - Validating your install

1. Once the HummingBoard has completed booting, a line entry will show in the IoT Dashboard. You can leave this window open to be notified when the device is ready. 
![Device in IoT Dashboard](./media/lab01/1_validatinginstall.png)
2. Right click on your device and select **Open in Device Portal** 
![Open Device Portal](./media/lab01/1_opendeviceportal.png)


**Note:** if your device doesn't show in the list, look at the HDMI output from the device to ensure it's fully booted, and read the IP Address from the display and enter that in your browser on port 8080. For example: http://192.168.1.40:8080
![Open Device Portal](./media/lab01/1_IoTCoreIPAddress.png)

3. In your browser enter the default username and password, that we just set:

|Name    |Value|
|--------|-----|
|Username|Administrator|
|Password|p@ssw0rd|


4. To test the SensorTile is connected. Make sure it's plugged into the Hummingboard, then open the Processes Menu, Select Run command
![Device Portal](./media/1_deviceportal1.png)
5. Type **"devcon status USB\VID_0483\*"** and hit enter to see if the device can see the connected SensorTile
6. You should see Name: USB Serial Device with status of Running
![SensorTile Connected](./media/1_SensorTileConnected.png)
7. Another way to see what is on the screen of the IoT device is to use the inbuilt **Screenshot** command on the Windows Device Portal. 
8. Select **Device Settings** on the Windows Device Portal, in the bottom right press the **Screenshot** button. Try it twice if it shows the broken image icon the first time.
![SensorTile Connected](./media/lab01/1_screenshot.png)   

You should now have a working IoT device with Windows 10 IoT Core installed, along with screen output being shown through the HDMI capture card to the Windows Camera app acting as a second screen, and confirmed that the sensor device is connected as a virtual com port. 


## 2 - Deploying apps to HummingBoard

### 2.1 - UWP IOT App

1. Open up the first lab project found in [C:\Labs\content\src\IoTLabs.TestApp\IoTLabs.TestApp.sln](file:///C:\Labs\content\src\IoTLabs.TestApp\IoTLabs.TestApp.sln)  
2. If prompted to log in with VisualStudio, simply close the window as no sign in is required.
3. Update the target system architecture to say 'ARM' as shown in the image below
![](./media/1_vs3.png)
4. Change target system from 'Device' to 'Remote Machine' and enter the IP address of your HummingBoard device. This can be found on the camera screen showing your HummingBoard device. It may already be listed in the **Auto Detected**
![](./media/1_vs2.png)
5. Run the project back in VisualStudio (by pressing the green arrow beside the Remote Machine) to test it on your HummingBoard. 

**Note:** the first deployment can take a few minutes.  You will know it has finished as the camera screen of your HummingBoard will show an interesting dashboard.

**Hint:** If no sensor data is displayed, try changing the USB port the SensorTile is connected through to another port on the HummingBoard. Stop the application in Visual Studio using the STOP button, then press the START button again. 


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

