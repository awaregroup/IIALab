using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.AI.MachineLearning;
using Windows.Foundation;
using Windows.Media;

using EdgeModuleSamples.Common;
using static EdgeModuleSamples.Common.AsyncHelper;
using static Helpers.BlockTimerHelper;

namespace SampleModule
{
    class ImageInference
    {
        private static AppOptions Options;
        private static ModuleClient ioTHubModuleClient;
        private static CancellationTokenSource cts = null;

        static async Task<int> Main(string[] args)
        {
            try
            {
                //
                // Parse options
                //

                Options = new AppOptions();
                Options.Parse(args);

                if (Options.ShowList) { }
                if (Options.Exit) return -1;
                if (string.IsNullOrEmpty(Options.FileName))
                    throw new ApplicationException("Please use --filename to specify which file to use");


                //
                // Init module client
                //

                if (Options.UseEdge)
                {
                    Log.WriteLine($"{AppOptions.AppName} module starting.");
                    await BlockTimer("Initializing Azure IoT Edge", async () => await InitEdge());
                }

                cts = new CancellationTokenSource();
                AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
                Console.CancelKeyPress += (sender, cpe) => cts.Cancel();

                //
                // Load model
                //

                MLModel model = null;
                await BlockTimer($"Loading modelfile '{Options.ModelPath}' on the {(Options.UseGpu ? "GPU" : "CPU")}",
                    async () => {
                        var d = Directory.GetCurrentDirectory();
                        var path = d + "\\" + Options.ModelPath;
                        
                        var modelFile = StorageFile.GetFileFromApplicationUriAsync(new Uri(d)).GetResults();
                        model = await MLModel.CreateFromStreamAsync(modelFile as IRandomAccessStreamReference);
                    });

                //
                // Open file
                //

                
                //
                // Main loop
                //
                do
                {
                    //
                    // Evaluate model
                    //

                    var inputShape = new long[2] { 1, 4 };
                    var inputFeatures = new float[4] { 100, 100, 100, 100 };

                    MLModelVariable result = null;
                    var evalticks = await BlockTimer("Running the model",
                        async () =>
                        {
                            var prediction = await model.EvaluateAsync(new MLModelVariable()
                            {
                                Variable = TensorFloat.CreateFromArray(inputShape, inputFeatures)
                            });
                        });


                    //
                    // Print results
                    //

                    var message = new MessageBody 
                    {
                        result = result.Variable.GetAsVectorView().First()
                    };
                    message.metrics.evaltimeinms = evalticks;
                    var json = JsonConvert.SerializeObject(message);
                    Log.WriteLineRaw($"Recognized {json}");

                    //
                    // Send results to Edge
                    //

                    if (Options.UseEdge)
                    { 
                        var eventMessage = new Message(Encoding.UTF8.GetBytes(json));
                        await ioTHubModuleClient.SendEventAsync("resultsOutput", eventMessage); 

                        // Let's not totally spam Edge :)
                        await Task.Delay(500);
                    }
                }
                while (Options.RunForever && ! cts.Token.IsCancellationRequested);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                return -1;
            }

            return 0;
        }

        
        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        private static async Task InitEdge()
        {
            // Open a connection to the Edge runtime
            ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(TransportType.Amqp);

            Log.WriteLineVerbose("CreateFromEnvironmentAsync OK");

            await ioTHubModuleClient.OpenAsync();

            Log.WriteLineVerbose("OpenAsync OK");

            Log.WriteLine($"IoT Hub module client initialized.");
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}