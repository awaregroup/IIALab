using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using IoTLabs.Dragonboard.App.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTLabs.Dragonboard.App.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel vm = null;

        public MainPage()
        {
            this.InitializeComponent();
            vm = new MainViewModel();
            this.DataContext = vm;

            this.Loaded += delegate(object sender, RoutedEventArgs args)
            {
                vm.LoadSensors();
            };

            this.Unloaded += delegate (object sender, RoutedEventArgs args)
            {
                vm.UnloadSensors();
            };
        }
    }
}
