using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AwareThings.WinIoTCoreServices.Core.Notifications;
using AwareThings.WinIoTCoreServices.Core.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AwareThings.WinIoTCoreServices
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        private MainPageViewModel vm = null;

        public Dashboard()
        {
            this.InitializeComponent();
            if (this.DataContext != null)
                if (this.DataContext is MainPageViewModel)
                    vm = (MainPageViewModel)this.DataContext;





            this.Loaded += async delegate (object sender, RoutedEventArgs args)
            {
                Messenger.Default.Register<CloseFlyoutsNotification>(this, OnCloseFlyoutsNotification);
                if (vm != null)
                {
                    await vm.ViewLoaded();
                }
            };

            this.Unloaded += async delegate (object sender, RoutedEventArgs args)
            {
                Messenger.Default.Unregister<CloseFlyoutsNotification>(this);
                if (vm != null)
                {
                    await vm.ViewUnloaded();
                }
            };

        }

        public void OnCloseFlyoutsNotification(CloseFlyoutsNotification msg)
        {
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    flSettings.Hide();
                });

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    //flOther.Hide();
                });
            }
            catch (Exception e)
            {

            }
        }

        private void CloseSettingsFlyout_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                flSettings.Hide();
            }
            catch (Exception exception)
            {

            }
        }

        private void CloseOtherFlyout_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
               // flOther.Hide();
            }
            catch (Exception exception)
            {

            }
        }
    }
}
