# Lab 06 - Turn your device into a locked down kiosk

## Option 1 - Using Settings to set up Assigned Access

You can use **Settings** on the local device to quickly configure one or a few devices as a kiosk. 

### Build the UWP Application that we will be using for assigned access

1. Open up the lab project found in `C:\Labs\content\src\IoTLabs.TestApp\IoTLabs.TestApp.sln`
2. Ensure the Target Architecture is set to **x86** and push the green debug button labeled **Local Machine**
   ![](./media/lab06/x86.jpg)
3. You may be prompted to enable **Developer mode**. Select the **Developer mode** toggle when prompted
   ![](./media/lab06/enable-developer.jpg)
4. Ensure the application is running correctly then close out of the Application and Visual Studio

### Configure Assigned Access
1.  Go to **Start** > **Settings** > **Accounts** > **Other users**
2.  Select **Set up a kiosk > Assigned access**, and then select **Get started**
   ![](./media/lab06/assigned-access.jpg)
3.  When prompted set the Kiosk users name to **Kiosk**
   ![](./media/lab06/setting-up-kiosk.jpg)
4.  When prompted to select an Application, select **IoTLabs.TestApp**
   ![](./media/lab06/select-app.jpg)
5.  Select **Close**.
   ![](./media/lab06/kiosk-done.jpg)

### Remove Assigned Access
1. Push **Ctrl + Alt + Delete**
2. Switch user into your administrative account
3. Go to **Start** > **Settings** > **Accounts** > **Other users**
4. Select **Set up a kiosk**,
3. Select the **Kiosk** users tile and then select **Remove kiosk**

## Option 2 - Using a Provisioning Package to set up Assigned Access

You can use **Provisoning Packages** to quickly and consistently deploy settings to a fleet of devices. This can be done either during OOBE or after the device has been set up.

### Install

1. Go to `C:\Labs\Content\src\IoTLabs.AssignedAccess\`
2. Open the ppkg file `lab06.ppkg`, this is the provisoning package that holds all the settings and files required to deploy
3. When prompted click **Yes, add it**
![](./media/lab06/add-package.jpg)
4. Restart your computer
5. Your device should auto login as the locked down Kiosk user

### Removing the provisioning package 

1. Push **Ctrl + Alt + Delete**
2. Switch user into your administrative account
3. Go to **Start** > **Settings** > **Accounts** > **Access work or school** > **Add or remove a provisioning package**
4. Select the provisoning package and then select **Remove**
