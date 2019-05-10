using IoTLabs.MachineLearning.ML;
using System;
using System.Linq;
using Windows.AI.MachineLearning;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace IoTLabs.MachineLearning
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //load and initialize model
                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/model.onnx"));
                var model = await MLModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);

                //make prediction
                var modelOutput = await model.EvaluateAsync(new MLModelInput()
                {
                    FloatInput = TensorFloat.CreateFromArray(new long[2] { 1, 4 }, new float[4] { float.Parse(TextBox1.Text), float.Parse(TextBox2.Text), float.Parse(TextBox3.Text), float.Parse(TextBox4.Text) })
                });

                //display results
                var noWifiDialog = new ContentDialog
                {
                    Title = "Machine Learning Module Result",
                    Content = $"{string.Join(",", modelOutput.Variable.GetAsVectorView().Select(f => f.ToString()))}",
                    CloseButtonText = "Close"
                };

                await noWifiDialog.ShowAsync();
            }
            catch (Exception)
            {
                ContentDialog noWifiDialog = new ContentDialog
                {
                    Title = "Something isn't working",
                    Content = "Did you enter numbers?",
                    CloseButtonText = "Try again"
                };

                try
                {
                    await noWifiDialog.ShowAsync();
                }
                catch { }
            }
        }
    }
}
