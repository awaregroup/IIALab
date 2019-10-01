using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using AwareThings.WinIoTCoreServices.Core.Common;
using AwareThings.WinIoTCoreServices.Core.Configuration;
using AwareThings.WinIoTCoreServices.Core.Interfaces;
using AwareThings.WinIoTCoreServices.Core.Services;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;

namespace AwareThings.WinIoTCoreServices.Controls
{
    public class SkinStorageServiceLocal : ISkinStorageService
    {
        internal static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        public const string BaseSkinsFolderName = "Skins";



        private LiveConfigurationService _liveConfigurationService;

        public SkinStorageServiceLocal(LiveConfigurationService liveConfigurationService)
        {
            _liveConfigurationService = liveConfigurationService;
        }

        public async Task<Dictionary<string, DownloadedSkin>> GetDownloadedSkinsAsync()
        {
            Dictionary<string, DownloadedSkin> results = new Dictionary<string, DownloadedSkin>();
            try
            {
                var m = await GetLocalSkins();
                if (m != null)
                    foreach (string ky in m.Keys)
                    {
                        results.Add(ky, _liveConfigurationService.GetDownloadedSkin(m[ky]));
                    }
            }
            catch (Exception e)
            {
            }
            return results;
        }


        public async Task<LiveConfiguration> GetDownloadedSkinAsync(string key)
        {
            try
            {
                var m = await GetLocalSkins(key);
                if (m != null)
                    if (m.Count > 0)
                    {
                        return m.Values.FirstOrDefault();
                    }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public async Task<bool> RemoveDownloadedSkinAsync(string key)
        {
            try
            {
                var skinFolder = await GetSkinFolder(key);
                await skinFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public async Task<bool> ImportDownloadedSkinAsync(string key, LiveConfiguration config)
        {
            try
            {
                var skinFolder = await GetSkinFolder(key);
                string jsonData = JsonConvert.SerializeObject(config);
                await File.WriteAllTextAsync(Path.Combine(skinFolder.Path, "configuration.json"), jsonData);

                if (config.Assets != null)
                    foreach (var asset in config.Assets)
                    {
                        var data = await HttpHelperUtils.AsyncGetUrlBytes(asset.SourceUrl);
                        if (data != null)
                        {
                            File.WriteAllBytes(Path.Combine(skinFolder.Path, asset.Key), data);
                        }
                    }

                await Task.Delay(TimeSpan.FromSeconds(1.0));

                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }

        public async Task<LiveConfiguration> ImportDefaultSkin(string sourcePath)
        {

            try
            {
                ResourceLoaderHelper2 ResourceLoaderHelper2 = new ResourceLoaderHelper2();
                string data = ResourceLoaderHelper2.LoadTextFileFromResource(sourcePath + ".Configuration.json");
                LiveConfiguration config = JsonConvert.DeserializeObject<LiveConfiguration>(data);
                if (config != null)
                    if ((config.Id ?? "") != "")
                    {
                        var skinFolder = await GetSkinFolder(config.Id);
                        await skinFolder.WriteTextToFileAsync(data, "Configuration.json", CreationCollisionOption.ReplaceExisting);
                        if (config.Assets != null)
                            foreach (var n in config.Assets)
                            {
                                if ((n.SourceUrl ?? "").StartsWith("~/"))
                                {
                                    try
                                    {
                                        string fl = (n.SourceUrl ?? "").Substring(2);
                                        byte[] binaryBytes = ResourceLoaderHelper2.LoadBinaryFileFromResource(sourcePath + "." + fl);
                                        if (binaryBytes != null)
                                        {
                                            await skinFolder.WriteBytesToFileAsync(binaryBytes, n.Key, CreationCollisionOption.ReplaceExisting);
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }

                        return config;
                    }

                //var skinFolder = await GetSkinFolder(key);
                //string jsonData = JsonConvert.SerializeObject(config);
                //await File.WriteAllTextAsync(Path.Combine(skinFolder.Path, "configuration.json"), jsonData);

                //if (config.Assets != null)
                //    foreach (var asset in config.Assets)
                //    {
                //        var data = await HttpHelperUtils.AsyncGetUrlBytes(asset.SourceUrl);
                //        if (data != null)
                //        {
                //            File.WriteAllBytes(Path.Combine(skinFolder.Path, asset.Key), data);
                //        }
                //    }

                //await Task.Delay(TimeSpan.FromSeconds(1.0));
            }
            catch (Exception e)
            {
            }
            return null;
        }

        private async Task<Dictionary<string, LiveConfiguration>> GetLocalSkins(string filterKey = "*")
        {
            Dictionary<string, LiveConfiguration> results = new Dictionary<string, LiveConfiguration>();
            try
            {
                var baseFolder = await GetSkinsStorage();
                var folders = await baseFolder.GetFoldersAsync();

                if (folders != null)
                {
                    foreach (var f in folders)
                    {
                        LiveConfiguration cfg = null;
                        string key = f.Name;

                        if ((filterKey == "*") || (filterKey == key))
                        {

                            var files = await f.GetFilesAsync();
                            foreach (var fl in files)
                            {
                                if (fl.Name.ToLower() == "configuration.json")
                                {
                                    cfg = _liveConfigurationService.LoadFromPath(fl.Path);
                                    if (cfg != null)
                                    {
                                        if (!results.ContainsKey(key))
                                        {
                                            results.Add(key, cfg);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return results;
        }


        private async Task<StorageFolder> GetSkinsStorage()
        {
            try
            {
                StorageFolder fldr =
                    await LocalFolder.CreateFolderAsync(BaseSkinsFolderName, CreationCollisionOption.OpenIfExists);
                return fldr;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        private async Task<StorageFolder> GetSkinFolder(string skinId)
        {
            try
            {
                StorageFolder fldr =
                    await (await GetSkinsStorage()).CreateFolderAsync(skinId, CreationCollisionOption.OpenIfExists);
                return fldr;
            }
            catch (Exception e)
            {
            }
            return null;
        }
    }

    public class ResourceLoaderHelper2
    {
        public string LoadTextFileFromResource(string srcPath)
        {

            string result = "";
            try
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream(srcPath);

                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }
            return result;
        }

        public byte[] LoadBinaryFileFromResource(string srcPath)
        {

            byte[] result = null;
            try
            {
                var assembly = this.GetType().GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream(srcPath);
                result = ReadToEnd(stream);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

            }
            return result;
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

    }
}
