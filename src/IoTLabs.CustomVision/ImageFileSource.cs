using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using EdgeModuleSamples.Common;

namespace SampleModule
{
    class ImageFileSource 
    {

        private Queue<string> _availableImages = new Queue<string>();
        
        public async Task<SoftwareBitmap> GetNextImageAsync(string path, CancellationToken token)
        {
            try
            {
                //
                // Load the SoftwareBitmap in the Queue
                //
                SoftwareBitmap sbmp = null;
                do
                {
                    //Check for new files
                    ScanUpdateQueue(path);

                    //Get next SoftwareBitmap if an item exists on the queue.
                    if (_availableImages.Count > 0)
                    {
                        string item = _availableImages.Dequeue();
                        _availableImages.Enqueue(item);
                        Log.WriteLine($"Loading Image : {item}");
                        try
                        {
                            sbmp = await LoadSoftwareBitmap(item);
                        }
                        catch (Exception e2)
                        {
                            Log.WriteLine($"Error Loading Image : {item} - {e2.Message}");
                        }
                    }

                    if ((sbmp == null)&&(!token.IsCancellationRequested))
                    {

                        //wait 2 second between each directory scan if no files found.
                        Log.WriteLine($"No Image Files found in {path} - waiting for 2 seconds");
                        await Task.Delay(2000, token);
                    }
                } while ((sbmp == null) && (!token.IsCancellationRequested));

                return sbmp;
            }
            catch (Exception e)
            {
                throw new Exception("Error Obtaining Software Bitmap from Files : " + e.Message);
            }
            return null;
        }

        public void ScanUpdateQueue(string path)
        {
            List<string> currentFiles = new List<string>();
            try
            {
                
                foreach (var file in Directory.GetFiles(path, "*.jpg"))
                {
                    currentFiles.Add(file);
                }
            }
            catch (Exception e)
            {
            }

            List<string> toEnQueue = _availableImages.ToList().Where(o => currentFiles.Contains(o)).ToList();
            toEnQueue.AddRange(currentFiles.Where(o=> !_availableImages.Contains(o)));
            _availableImages.Clear();
            foreach (var file in toEnQueue)
            {
                _availableImages.Enqueue(file);
            }
        }

        public static async Task<SoftwareBitmap> LoadSoftwareBitmap(string path)
        {
            try
            {
                var decoder = await AsyncHelper.AsAsync(BitmapDecoder.CreateAsync(await ConvertBytesToInputStream(File.ReadAllBytes(path))));
                return (await AsyncHelper.AsAsync(decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore)));
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public static async Task<InMemoryRandomAccessStream> ConvertBytesToInputStream(byte[] data)
        {
            try
            {
                InMemoryRandomAccessStream strm = new InMemoryRandomAccessStream();

                var writer = new DataWriter(strm);
                {
                    writer.WriteBytes(data);
                    await AsyncHelper.AsAsync(writer.FlushAsync());
                    await AsyncHelper.AsAsync(writer.StoreAsync());
                }

                return strm;

            }
            catch (Exception e)
            {

            }

            return null;
        }

    }
}