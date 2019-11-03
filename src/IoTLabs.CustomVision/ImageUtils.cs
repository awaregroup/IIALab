using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using EdgeModuleSamples.Common;
using SkiaSharp;

namespace WindowsAiEdgeLabCV
{
    public class ImageUtils
    {
        public async static Task<byte[]> EncodeSoftwareBitmapToJpeg(SoftwareBitmap softwareBitmap)
        {
            try
            {
                using (var stream = new InMemoryRandomAccessStream())
                {
                    // Create an encoder with the desired format
                    BitmapEncoder encoder = await AsyncHelper.AsAsync(BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream));

                    // Set the software bitmap
                    encoder.SetSoftwareBitmap(softwareBitmap);

                    // Set additional encoding parameters, if needed
                    encoder.BitmapTransform.ScaledWidth = (uint)softwareBitmap.PixelWidth;
                    encoder.BitmapTransform.ScaledHeight = (uint)softwareBitmap.PixelHeight;
                    encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.None;
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                    encoder.IsThumbnailGenerated = false;

                    try
                    {
                        await AsyncHelper.AsAsync(encoder.FlushAsync());
                    }
                    catch (Exception)
                    {
                    }

                    if (encoder.IsThumbnailGenerated == false)
                    {
                        await AsyncHelper.AsAsync(encoder.FlushAsync());
                    }

                    var reader = new DataReader(stream.GetInputStreamAt(0));
                    var bytes = new byte[stream.Size];
                    await AsyncHelper.AsAsync(reader.LoadAsync((uint)stream.Size));
                    reader.ReadBytes(bytes);

                    return bytes;
                }
            }
            catch (Exception)
            {
                
            }

            return new byte[] { };
        }

        public static async Task<InMemoryRandomAccessStream> ConvertToIRandomAccessStream(SoftwareBitmap sb)
        {
            InMemoryRandomAccessStream imageStream = new InMemoryRandomAccessStream();
            var encoder = await AsyncHelper.AsAsync(BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, imageStream));
            encoder.SetSoftwareBitmap(sb);
            await AsyncHelper.AsAsync(encoder.FlushAsync());
            imageStream.Seek(0L);
            return imageStream;
        }

        public static async Task<byte[]> ConvertInputStreamToBytes(IRandomAccessStream stream)
        {
            try
            {
                var reader = new DataReader(stream.GetInputStreamAt(0));
                var bytes = new byte[stream.Size];
                await AsyncHelper.AsAsync(reader.LoadAsync((uint)stream.Size));
                reader.ReadBytes(bytes);
                return bytes;
            }
            catch (Exception)
            {

            }
            return new byte[] { };
        }

        public static async Task<byte[]> GetConvertedImage(SoftwareBitmap sb)
        {
            return (await AnnotateImage(sb,"","",0,0));
        }

        public static async Task<byte[]> AnnotateImage(SoftwareBitmap sb, string headerText="", string footerText = "", int headerSize = 40, int footerSize = 40, int maxWidth=1280)
        {
            try
            {
                using (var t = await ConvertToIRandomAccessStream(sb))
                {
                    SKPaint pntBlack = new SKPaint() { Color = new SKColor(0, 0, 0) };
                    SKPaint pntWhite = new SKPaint() { Color = new SKColor(255, 255, 255), TextSize = 24, TextAlign = SKTextAlign.Left };
                    SKPaint pntWhiteFooter = new SKPaint() { Color = new SKColor(255, 255, 255), TextSize = 22, TextAlign = SKTextAlign.Left };

                    byte[] data = await ConvertInputStreamToBytes(t);
                    float w = (float) sb.PixelWidth;
                    float h = (float) sb.PixelHeight;

                    float origW = (float)sb.PixelWidth;
                    float origH = (float)sb.PixelHeight;

                    int newWidth = sb.PixelWidth;
                    int newHeight = sb.PixelHeight;

                    if (w > maxWidth)
                    {
                        float aspect = w / h;
                        w = (float) maxWidth;
                        h = w / aspect;
                        newWidth = (int) Math.Ceiling(w);
                        newHeight = (int)Math.Ceiling(h);
                    }

                    SKImageInfo info = new SKImageInfo(newWidth, newHeight + headerSize + footerSize);
                    using (SKSurface sf = SKSurface.Create(info))
                    using (SKCanvas cnv = sf.Canvas)
                    {
                        using (SKBitmap skb = SKBitmap.Decode(new SKMemoryStream(data)))
                        {
                            cnv.Clear(pntBlack.Color);
                            cnv.DrawBitmap(skb, new SKRect(0,0,origW, origH), new SKRect(0, (float) headerSize, w, h + (float)headerSize));
                            
                            if (headerText!="")
                                cnv.DrawText(headerText, 5,3 + pntWhite.TextSize, pntWhite);
                            if (footerText != "")
                                cnv.DrawText(footerText, 5, 3 + h + headerSize + pntWhiteFooter.TextSize, pntWhiteFooter);
                            using (SKData enc = sf.Snapshot().Encode(SKEncodedImageFormat.Jpeg, 90))
                            {
                                return enc.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return new byte[] {};
        }



    }
}
