using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AwareThings.IoTCoreServices.SensorTileSensors;
using AwareThings.WinIoTCoreServices;
using AwareThings.WinIoTCoreServices.Controls;
using AwareThings.WinIoTCoreServices.Core.Interfaces;
using AwareThings.WinIoTCoreServices.Core.Panels;
using AwareThings.WinIoTCoreServices.Core.Sensors;
using AwareThings.WinIoTCoreServices.Core.Services;
using AwareThings.WinIoTCoreServices.Core.ViewModels;
using AwareThings.WinIoTCoreServices.Core.Views;
using AwareThings.WinIoTCoreServices.ImageClassificationSensors;
using GalaSoft.MvvmLight.Ioc;
//using AwareThings.WinIoTCoreServices.ImageClassificationSensors;
using GalaSoft.MvvmLight.Threading;

namespace IoTLabs.TestApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }


        public void ScanInterfaces(Type myType)
        {
            try
            {
                Debug.WriteLine("Searching for Type : " + myType.FullName);

                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => myType.IsAssignableFrom(p));

                if (types != null)
                    foreach (var item in types)
                    {
                        if (!item.IsInterface)
                            Debug.WriteLine(" >> Found : " + item.FullName);
                    }

            }
            catch
            {
            }



        }


        public static ViewModelLocator Locator;


        public void InitializeFactories(ViewModelLocator locator)
        {
            try
            {
                //Setup some defaults for the Lab Version.
                XamlHelper2.BaseDefaultThemePath = "ms-appx:///";
                SimpleIoc.Default.Unregister<ISkinStorageService>();
                SimpleIoc.Default.Register<ISkinStorageService, AwareThings.WinIoTCoreServices.Controls.SkinStorageServiceLocal>();
                locator.DeviceConfigurationService.SetAutoStartIfTpmAvailable(false);

                //HOL Step 1: Code - Deploy the App with Default AwareThings Skin..
                locator.DeviceConfigurationService.SetDefaultSkin("AwareThings", "AwareThings.WinIoTCoreServices.Skins.AwareThings");

                //HOL Step 2: 
                // 1. Connect SensorTile.Box to Device
                // 2. Add Reference to SensorTile (AwareThings.IoTCoreServices.SensorTileSensors.dll in components folder).
                // 3. Add the HOL Skin directory to the App (create new folder HOL under \Skins). Add the skin files to the project + set them to 'Embedded Resource' / 'Copy always'
                // 4. Uncomment these lines to (a) Set Contoso skin with SensorTile Support,  (b) Add the new Sensor Providor required by the skin (for SensorTiles.Box)
                //locator.SensorFactoryService.AddProvidor(new SensorTileSensorProvidor());
                //locator.DeviceConfigurationService.SetDefaultSkin("HOL", "AwareThings.WinIoTCoreServices.Skins.HOL");


                //HOL Step 3: 
                // 1. Deploy the Generated TPMOverride.json file to the device (LocalState folder) with Connection settings pointing to Iot Central Device using SensorTile.Box DeviceTemplate
                // 3. Uncomment this line so that Azure IoT Services are autostarted if TPM information is available on device (either tpmoverride.json or if not available will attempt to use device tpm).
                //locator.DeviceConfigurationService.SetAutoStartIfTpmAvailable(true);

                //------------------------------------------------------------

                //Add Display Panel Providors
                locator.DisplayPanelFactoryService.AddProvidor(new DefaultDisplayPanelFactoryProvidor());
                locator.DisplayPanelFactoryService.AddProvidor(new ImageClassificationSkinPanelProvidor());
                
                //Add Default Sensor Providors + Image Classification Sensor Providor
                locator.SensorFactoryService.AddProvidor(new ImageClassificationSensorProvidor());
                locator.SensorFactoryService.AddProvidor(new DefaultSensorFactoryProvidor());
                
                //Add Additional Skin Settings UX Templates for Image Classifications.
                locator.DisplayPanelFactoryService.AddTemplateResourceUri(new Uri("ms-appx:///AwareThings.WinIoTCoreServices.ImageClassificationSensors/ImageClassificationDictionary.xaml", UriKind.RelativeOrAbsolute));

                //Add Additional Resource Dictionaries
                try
                {
                    foreach (var m in locator.DisplayPanelFactoryService.ProvidorStyleResourceFiles)
                        try
                        {
                            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = m });
                        }
                        catch (Exception e)
                        {

                        }
                }
                catch (Exception e)
                {

                }

            }
            catch (Exception ee)
            {
            }

            //Setup Provisioning Service

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Locator = (ViewModelLocator)this.Resources["Locator"];

            InitializeFactories(Locator);

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            DispatcherHelper.Initialize();

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(Dashboard), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
