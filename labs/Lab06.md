# Lab 06 - Turn your device into a locked down kiosk

## 1 - Deploy an app to use in Kiosk Mode
As we are deploying an app that has not been officially released on the Windows store, we need to turn on developer features. Once enabled, the IoTLabs UWP (Universal Windows Platform) app can be installed.

### 1.1 - Turn on developer features

1. Open the start menu and type **Developer Settings** and select the top option\
![](./media/lab06/developer-start.png)

2. Select the **Developer Mode** toggle\
![](./media/lab06/developer-mode-selected.png)

3. Click **Yes** and wait a few minutes for it to load the required packages\
![](./media/lab06/developer-mode.png)

### 1.2 - Install the application

1. Navigate to `C:\Labs\content\src\IoTLabs.AssignedAccess\`

2. Unzip **IoTLabs.TestApp_1.0.1.0_Debug_Test** and open the unzipped folder. 
**Hint:** Right-click on the .zip folder and ‘Extract All…’

3. Right click on the file named **Install.ps1** and click **Run with PowerShell**\
![](./media/lab06/install-app.png)

4. Look for the message ‘Success: Your app was successfully installed.’ When prompted, type **Y** and **Enter** to allow the app to finish installing.  This closes the PowerShell window.
![](./media/lab06/installed-app.png)


## Option 1 - Using Settings to set up Assigned Access

You can use **Settings** on the local device to quickly configure one or a few devices as a kiosk. 

### 1.1 - Configure Assigned Access

1.  Go to **Start** > **Settings** > **Accounts** > **Family & other users**  **Hint:** Ignore the warning (under ‘Your Info’) in red it says ‘You need to activate Windows before you can personalize your PC.’  Simply click ‘Family & other users’.
![](./media/lab06/assigned-access.jpg)

2.  Select **Set up a kiosk > Assigned access**, and then select **Get started**

3.  When prompted set the Kiosk user name to **Kiosk** and click **Next**\
![](./media/lab06/setting-up-kiosk.jpg)

4.  When prompted to select an Application, select **IoTLabs.TestApp**\
![](./media/lab06/select-app.jpg)

5.  Select **Close**\
![](./media/lab06/kiosk-done.jpg)

6.  Restart the device. 

7.  Your device should auto login as the locked down Kiosk user.  Wait for the kiosk to power up and take a look at the IoT Dashboard before you carry out the next steps.

### 1.2 - Remove Assigned Access

1. Push **Ctrl + Alt + Delete**

2. Switch user into your administrative account, this is likely to be 'LabUser'. Enter your password that you used previously for your user **lab.userXX@msiotlabs.com**

3. Go to **Start** > **Settings** > **Accounts** > **Familty & other users**

4. Select **Set up a kiosk**

5. Select the **Kiosk** users tile and then select **Remove kiosk.** The system will ask you to confirm that you really wish to remove the kiosk, click **Remove** again.

**Hint:** you may want to restart your device in preparation for the next step.

## Option 2 - Using a Provisioning Package to set up Assigned Access

You can use **Provisioning Packages** to quickly and consistently deploy settings to a fleet of devices. This can be done either during OOBE (Out-Of-Box Experience) or after the device has been set up.

### 2.1 - Install

1. Go to `C:\Labs\Content\src\IoTLabs.AssignedAccess\`
**Hint:** if you have not restarted your device from the previous step, you may need to use the magnifying glass beside the windows icon to search for ‘File Explorer’ to then find your C: drive.

2. Open the ppkg file `lab06.ppkg`, this is the provisioning package that holds all the settings and files required to deploy.

3. Allow the app to make changes to your device, click **Yes**. When prompted click **Yes, add it**\
![](./media/lab06/add-package.jpg)

4. It may look as if nothing has occurred, simply restart your computer.

5. Your device should auto login as the locked down Kiosk user. This should display the same IoT Dashboard that you saw with Option 1 above.

### 2.2 - Removing the provisioning package 

1. Push **Ctrl + Alt + Delete**

2. Switch user into your administrative account, this is likely to be 'LabUser'. Enter your password that you used previously for your user **lab.userXX@msiotlabs.com**

3. Go to **Start** > **Settings** > **Accounts** > **Access work or school** > **Add or remove a provisioning package**\
![](./media/lab06/add-remove-package.jpg)

4. Select the provisioning package and then select **Remove**  Allow the app to make changes to your device, click **Yes**.  You should notice the Package 'Lab06' has been removed.
![](./media/lab06/remove-package.jpg)
