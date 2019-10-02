using EdgeModuleSamples.Common;
using Microsoft.Azure.Devices.Client;
using Mono.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Windows.AI.MachineLearning;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Profile;
using WindowsAiEdgeLabCV;
using static EdgeModuleSamples.Common.AsyncHelper;
using static Helpers.BlockTimerHelper;

namespace SampleModule
{
    class ImageInference
    {
        private static AppOptions Options;
        private static ModuleClient ioTHubModuleClient;
        private static CancellationTokenSource cts = null;

        private static object _objLock = new object();
        private static string _lastPayload = "{}";
        private static string _lastStatsPayload = "{}";
        private static byte[] _lastAnnotatedImage = null;
        private static OverallStats _stats = new OverallStats();

        static void SetLatestStatsPayload(string lastStatsPayload)
        {
            try
            {
                lock (_objLock)
                {
                    _lastStatsPayload = lastStatsPayload;
                }
            }
            catch (Exception e)
            {
            }
        }

        static void SetLatestFrameData(string lastPayload, string lastStatsPayload, byte[] lastAnnotatedImage)
        {
            try
            {
                lock (_objLock)
                {
                    _lastPayload = lastPayload;
                    _lastStatsPayload = lastStatsPayload;
                    _lastAnnotatedImage = lastAnnotatedImage;
                }
            }
            catch (Exception e)
            {
            }
        }

        static byte[] GetLatestAnnotatedImage()
        {
            byte[] result = new byte[] { };
            try
            {
                
                lock (_objLock)
                {
                    if (_lastStatsPayload!=null)
                    { 
                        result = _lastAnnotatedImage;
                    }
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        static string GetLatestPayload()
        {
            string result = "";
            try
            {

                lock (_objLock)
                {
                    result = _lastPayload;
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        static string GetLatestStatsPayload()
        {
            string result = "";
            try
            {

                lock (_objLock)
                {
                    result = _lastStatsPayload;
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }


        static async Task<int> Main(string[] args)
        {
            try
            {
                //
                // Parse options
                //

                Options = new AppOptions();

                Options.Parse(args);

                if (Options.ShowList)
                {
                    var devices = await FrameSource.GetSourceNamesAsync();

                    Log.WriteLine("Available cameras:");
                    
                    foreach(var device in devices)
                        Log.WriteLine(device);
                }

                if (Options.Exit)
                    return -1;

                if (string.IsNullOrEmpty(Options.DeviceId))
                    throw new ApplicationException("Please use --device to specify which camera to use");

                try
                {
                    string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                    ulong v = ulong.Parse(sv);
                    ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                    ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                    ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                    ulong v4 = (v & 0x000000000000FFFFL);
                    var systemVersion = $"{v1}.{v2}.{v3}.{v4}";

                    _stats.CurrentVideoDeviceId = Options.DeviceId;
                    _stats.Platform = $"{AnalyticsInfo.VersionInfo.DeviceFamily} - {System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") ?? "Unknown"} - {systemVersion}";
                }
                catch (Exception e)
                {
                }
                

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

                ScoringModel model = null;
                await BlockTimer($"Loading modelfile '{Options.ModelPath}' on the {(Options.UseGpu ? "GPU" : "CPU")}",
                    async () => {
                        var d = Directory.GetCurrentDirectory();
                        var path = d + "\\" + Options.ModelPath;
                        StorageFile modelFile = await AsAsync(StorageFile.GetFileFromPathAsync(path));
                        model = await ScoringModel.CreateFromStreamAsync(modelFile,Options.UseGpu);
                    });

                _stats.OnnxModelLoaded = true;
                _stats.CurrentOnnxModel = Options.ModelPath;
                _stats.IsGpu = Options.UseGpu;

                // WebServer Code


                HttpServer httpsv = null;
                bool HttpServerStarted = false;

                if (Options.RunForever)
                {
                    try
                    {
                        Log.WriteLine($"Start HTTP Server on port : " + Options.WebServerPort.ToString());
                        httpsv = new HttpServer(Options.WebServerPort);
                        httpsv.Start();
                        httpsv.OnGet += HttpsvOnOnGet;

                        HttpServerStarted = true;
                        Log.WriteLine($"- HTTP Server Started.");
                        Log.WriteLine($"");

                    }
                    catch (Exception e)
                    {
                        HttpServerStarted = false;
                        Log.WriteLine($"Exiting - Websockets Server Failed to start : " + e.Message);
                    }
                }


                //
                // Open camera
                //

                using (var frameSource = new FrameSource())
                {
                    await frameSource.StartAsync(Options.DeviceId,Options.UseGpu);

                    _stats.DeviceInitialized = true;
                    SetLatestStatsPayload(JsonConvert.SerializeObject(_stats));

                    //
                    // Main loop
                    //
                    do
                    {
                        Log.WriteLineVerbose("Getting frame...");
                        using (var frame = await frameSource.GetFrameAsync())
                        {
                            var inputImage = frame.VideoMediaFrame.GetVideoFrame();
                            ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(inputImage);

                            _stats.TotalFrames = _stats.TotalFrames + 1;

                            //
                            // Evaluate model
                            //

                            ScoringOutput outcome = null;
                            var evalticks = await BlockTimer("Running the model",
                                async () =>
                                {
                                    var input = new ScoringInput() { data = imageTensor };
                                    outcome = await model.EvaluateAsync(input);
                                });

                            //
                            // Print results
                            //

                            _stats.TotalEvaluations = _stats.TotalEvaluations + 1;
                            
                            var message = ResultsToMessage(outcome);
                            message.metrics.evaltimeinms = evalticks;
                            var json = JsonConvert.SerializeObject(message);
                            Log.WriteLineRaw($"Inferenced: {json}");

                            //
                            // Send results to Edge
                            //

                            if (Options.UseWebServer)
                            {
                                try
                                {
                                    ResultPayload payload = new ResultPayload() {Result = message};
                                    string summaryText = "-";
                                    if ((message.results != null)&&(message.metrics!=null))
                                    {
                                        long totalEvaluations = _stats.TotalEvaluationSuccessCount;
                                        double confidence = 0.0;
                                        string lastLabel = "";

                                        _stats.TotalEvaluationSuccessCount = _stats.TotalEvaluationSuccessCount + 1;
                                        if (message.results.Length > 0)
                                        {
                                            summaryText = $"Matched : {message.results[0].label} - Confidence ={message.results[0].confidence.ToString("P")} - Eval Time {message.metrics.evaltimeinms} ms";
                                            confidence = message.results[0].confidence;
                                            lastLabel = message.results[0].label;
                                        }
                                        else
                                        {
                                            summaryText = $"No Match - Eval Time {message.metrics.evaltimeinms} ms";
                                        }

                                        //update eval ms stats
                                        try
                                        {
                                            if (totalEvaluations > 0)
                                            {
                                                _stats.AverageEvaluationMs = ((_stats.AverageEvaluationMs * ((double)totalEvaluations) + ((double)message.metrics.evaltimeinms))/((double)totalEvaluations+1));
                                            }
                                            else
                                            {
                                                _stats.AverageEvaluationMs = (double)message.metrics.evaltimeinms;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            
                                        }

                                        //update match totals
                                        try
                                        {
                                            if (lastLabel != "")
                                            {
                                                if (totalEvaluations > 0)
                                                {
                                                    _stats.AverageConfidence = ((_stats.AverageConfidence * ((double)totalEvaluations) + ((double)confidence)) / ((double)totalEvaluations + 1));
                                                }
                                                else
                                                {
                                                    _stats.AverageConfidence = (double)confidence;
                                                }

                                                lock (_objLock)
                                                {
                                                    LabelSummary ls = _stats.MatchSummary.FirstOrDefault(o => o.Label == lastLabel);
                                                    if (ls == null)
                                                    {
                                                        ls = new LabelSummary(){Label=lastLabel, TotalMatches = 1};
                                                        _stats.MatchSummary.Add(ls);
                                                    }
                                                    else
                                                    {
                                                        ls.TotalMatches = ls.TotalMatches + 1;
                                                    }
                                                }

                                            }
                                        }
                                        catch (Exception e)
                                        {

                                        }


                                    }
                                    else if (message.metrics == null)
                                    {
                                        _stats.TotalEvaluationFailCount = _stats.TotalEvaluationFailCount + 1;
                                        summaryText = $"Evaluation Failed";
                                    }
                                    else
                                    {
                                        _stats.TotalEvaluationSuccessCount = _stats.TotalEvaluationSuccessCount + 1;
                                        summaryText = $"No Match - Eval Time {message.metrics.evaltimeinms} ms";
                                    }

                                    payload.Statistics = _stats;

                                    byte[] data = await ImageUtils.GetConvertedImage(inputImage.SoftwareBitmap);
                                    byte[] annotatedData = await ImageUtils.AnnotateImage(inputImage.SoftwareBitmap, $"Current Webcam : {Options.DeviceId??"-"}", summaryText);

                                    if (data != null)
                                    {
                                        payload.ImageSnapshot = Convert.ToBase64String(data);
                                        SetLatestFrameData(JsonConvert.SerializeObject(payload), JsonConvert.SerializeObject(_stats), annotatedData);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.WriteLineRaw($"Failed to create Result Payload : {e.Message}");
                                }
                            }

                            if (Options.UseEdge)
                            { 
                                var eventMessage = new Message(Encoding.UTF8.GetBytes(json));
                                await ioTHubModuleClient.SendEventAsync("resultsOutput", eventMessage); 

                                // Let's not totally spam Edge :)
                                await Task.Delay(500);
                            }
                        }
                    }
                    while (Options.RunForever && ! cts.Token.IsCancellationRequested);

                    await frameSource.StopAsync();
                }

                if (HttpServerStarted)
                {
                    try
                    {
                        Log.WriteLine($"- Stopping Web Server.");
                        httpsv.OnGet -= HttpsvOnOnGet;
                        httpsv.Stop();
                        httpsv = null;
                        Log.WriteLine($"- Web Server Stopped.");
                    }
                    catch (Exception e)
                    {
                        
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.WriteLineException(ex);
                return -1;
            }
        }

        private static void HttpsvOnOnGet(object sender, HttpRequestEventArgs e)
        {
            try
            {
                string t = e.Request.RawUrl;
                Log.WriteLine($"- Received Request : " + t);
                if (t == "/data")
                {
                    //get last full payload with image
                    e.Response.ContentType = "application/json";
                    e.Response.WriteContent(Encoding.UTF8.GetBytes(GetLatestPayload() ?? "{}"));
                }
                else if (t == "/stats")
                {
                    //get last full payload with image
                    e.Response.ContentType = "application/json";
                    e.Response.WriteContent(Encoding.UTF8.GetBytes(GetLatestStatsPayload()??"{}"));
                }
                else if (t == "/image")
                {
                    //get last annotated image request
                    e.Response.ContentType = "image/jpeg";
                    e.Response.WriteContent(GetLatestAnnotatedImage());
                }
            }
            catch (Exception exception)
            {
            }
        }

        private static MessageBody ResultsToMessage(ScoringOutput outcome)
        {
            var resultVector = outcome.classLabel.GetAsVectorView();
            var message = new MessageBody();
            message.results = new LabelResult[1];
            message.results[0] = new LabelResult() { label = resultVector.First(), confidence = 1.0 };

            return message;
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