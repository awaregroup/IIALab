# Lab 03 - Configure your cloud solution platform

## 1 - Deploy and configure Azure IoT components

### 1.0 - Create your workspace

The focal point of all Microsoft IoT solutions is the Azure IoT Hub service. IoT Hub provides secure messaging, device provisioning and device management capabilities.

Azure Resource Manager Templates (ARM Templates) can be deployed into Azure that include IoT Hub. Click the button below to create the Azure IoT components required for the next labs:

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fawaregroup%2FIIALab%2Fmaster%2Fsrc%2FAzure.ARM%2Fiia-azuredeploy.json" target="_blank" rel="noopener">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fawaregroup%2FIIALab%2Fmaster%2Fsrc%2FAzure.ARM%2Fiia-azuredeploy.json" target="_blank" rel="noopener">
<img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.png"/>
</a>

You can also visualize an ARM template to see the components that will be created.

### 1.2 Install Example OEM Packages
Most required packages will automatically be imported to the workspace, but we can include sample packages by running the following command
```Import-IoTOEMPackage *```

### 1.3 - Install Board Support Package (BSP)

Next include the board support package (BSP) provided by the component/silicon vendor containing drivers compatable with Windows IoT Core.

Run the following PowerShell commands in the console from the previous step to install the BSP for the Dragonboard 410c.

```
$bspName = "QCDB410C"
Import-IoTBSP -BSPName $bspName -Source "C:\labs\tools\Dragonboard\db410c_bsp.zip"
```

### 1.4 - Create product

A product is a specific configuration of a device based upon a BSP that contains what custom applications and customisations are intended to be deployed to a range of devices.

```
$productName = "Lab03Product"
Add-IoTProduct -ProductName $productName -BSPName $bspName 
```
You will be asked for further SMBIOS information such as Manufacturer name, Product Family, SKU, BaseboardManufacturer and BaseboardProduct, enter the example values as below.

- System OEM Name: ContosoElectronics
- System Family Name: SensorArray
- System SKU Number: BLINK-V1
- Baseboard Manufacturer Qualcomm
- Baseboard Product: Dragonboard 410c

### 1.5 - Add Universal Windows App

1. Change the path to the full path of the appx file you generated in lab01 (including the file name), then run these powershell scripts to bundle the application with your image.

**Note: Do not change the $configName or the $appName. The following scripts assume the application name is fixed and the configuration environment is set to be the test environment.**

```
$appName = "Appx.DragonboardTest"
$configName = "Test"

Add-IoTAppxPackage -AppxFile "[full-path-to-appx-file-from-lab01]" -StartupType "fga" -OutputName $appName
New-IoTCabPackage -PkgFile $appName
Add-IoTProductFeature -Product $productName -Config $configName -FeatureID "APPX_DRAGONBOARDTEST" -OEM
```

### 1.6 - Compile FFU image

By running the previous commands we have created packages that describe what content should be included in this product. Next we process these packages into cabinet files for inclusion in the final image.
```
New-IoTCabPackage -PkgFile "All"
```

With all the cab packages compiled the FFU image can be created by running the following command, this can take up to ten minutes to complete.

```
New-IoTFFUImage -Product $productName -Config $configName
```


## 2 - Install custom image

### 2.2 - Installing custom image

1. Connect Dragonboard to host PC with a Micro-USB cable
1. Hold down the 'Volume Up (+)' button while plugging in the power adapter into the Dragonboard
1. Open IoT Dashboard and click 'Setup a new device'
1. Change the device type to 'Qualcomm \[Dragonboard 410c\]' and set the OS Build to 'Custom'
1. Browse to the custom FFU file generated in the previous steps
1. Accept the license agreement and click 'Install'

![IoT Dashboard](./media/2_iotdashboard.png)

### 2.3 - Validating your install

1. Once the Dragonboard has completed installing, a line entry will show in the IoT Dashboard as above
2. Right click on your device and select 'Device Portal'
3. In your browser enter the default username and password:

|Name    |Value|
|--------|-----|
|Username|Administrator|
|Password|p@ssw0rd|

![Device Portal](./media/1_deviceportal1.png)

4. Enter a name in the 'Change your device name' text box and click 'Save'. Your device should reboot and display the new name 

5. Check the sensors and the bundled application works as expected.
