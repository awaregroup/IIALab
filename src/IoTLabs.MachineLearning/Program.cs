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
using WindowsAiEdgeLabTabular;

namespace SampleModule
{
    class TabularInference
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
                    throw new ApplicationException("Please use --file to specify which file to use");


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
                Console.WriteLine($"Loading model from: '{Options.ModelPath}', Exists: '{File.Exists(Options.ModelPath)}'");
                await BlockTimer($"Loading modelfile '{Options.ModelPath}' on the {(Options.UseGpu ? "GPU" : "CPU")}",
                    async () =>
                    {
                        var d = Directory.GetCurrentDirectory();
                        var path = d + "\\" + Options.ModelPath;

                        StorageFile modelFile = await AsAsync(StorageFile.GetFileFromPathAsync(path));
                        model = await MLModel.CreateFromStreamAsync(modelFile);
                    });


                do
                {
                    //
                    // Open file
                    //
                    var rows = new List<DataRow>();
                    try
                    {
                        using (var fs = new StreamReader(Options.FileName))
                        {
                            // I just need this one line to load the records from the file in my List<CsvLine>
                            rows = new CsvHelper.CsvReader(fs).GetRecords<DataRow>().ToList();
                            Console.WriteLine($"Loaded {rows.Count} row(s)");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Console.WriteLine(rows);


                    //
                    // Main loop
                    //

                    foreach (var row in rows)
                    {
                        //
                        // Evaluate model
                        //

                        var inputShape = new long[2] { 1, 4 };
                        var inputFeatures = new float[4] { row.Temperature, row.Pressure, row.Humidity, row.ExternalTemperature };

                        MLModelVariable result = null;
                        var evalticks = await BlockTimer("Running the model",
                            async () =>
                            {
                                result = await model.EvaluateAsync(new MLModelVariable()
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


                        Console.WriteLine("Waiting 1 second...");
                        Thread.Sleep(1000);
                    }
                }
                while (Options.RunForever && !cts.Token.IsCancellationRequested);
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