using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace AwareThings.WinIoTCoreServices.Controls
{
    public static class XamlHelper2
    {

        public static string BaseDefaultThemePath = "ms-appx:///AwareThings.WinIoTCoreServices.Core/";

        internal static ResourceDictionary CoreTemplatesResourceDictionary =
            Application.Current.Resources.MergedDictionaries[0].MergedDictionaries[0];

        internal static ResourceDictionary CoreThemeResourceDictionary =
            Application.Current.Resources.MergedDictionaries[0].MergedDictionaries[0].MergedDictionaries[0];

        public static Brush ResolveBrushFromResources(string key)
        {
            try
            {
                if (CoreThemeResourceDictionary.ContainsKey(key))
                    if (CoreThemeResourceDictionary[key] is Brush)
                        return (CoreThemeResourceDictionary[key] as Brush);

                if (CoreTemplatesResourceDictionary.ContainsKey(key))
                    if (CoreTemplatesResourceDictionary[key] is Brush)
                        return (CoreTemplatesResourceDictionary[key] as Brush);

            }
            catch (Exception e)
            {

            }

            return null;
        }

        public static DataTemplateSelector ResolveDataTemplateSelectorFromResources(string key)
        {
            try
            {

                if (CoreTemplatesResourceDictionary.ContainsKey(key))
                    if (CoreTemplatesResourceDictionary[key] is DataTemplateSelector)
                        return (CoreTemplatesResourceDictionary[key] as DataTemplateSelector);

                if (CoreThemeResourceDictionary.ContainsKey(key))
                    if (CoreThemeResourceDictionary[key] is DataTemplateSelector)
                        return (CoreThemeResourceDictionary[key] as DataTemplateSelector);

            }
            catch (Exception e)
            {

            }

            return null;
        }

        public static ResourceDictionary ParseXamlResourceDictionary(string xaml)
        {
            try
            {
                if (xaml != "")
                {
                    var rdn = (ResourceDictionary)XamlReader.Load(xaml);
                    if (rdn != null)
                        return rdn;
                }
            }
            catch (Exception e)
            {
            }

            return null;
        }

        public static Style ResolveStyleFromResourceDictionary(ResourceDictionary rd, string key)
        {
            Windows.UI.Xaml.Style st = null;
            try
            {

                if (rd != null)
                {
                    if (rd.ContainsKey(key))
                        if (rd[key] is Style)
                            return ((Style)rd[key]);
                }

                if (CoreThemeResourceDictionary.ContainsKey(key))
                    if (CoreThemeResourceDictionary[key] is Style)
                        return ((Style)CoreThemeResourceDictionary[key]);

            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static DataTemplate ResolveStylingDataTemplateFromResourceDictionary(ResourceDictionary rd, string key)
        {
            Windows.UI.Xaml.Style st = null;
            try
            {

                if (rd != null)
                {
                    if (rd.ContainsKey(key))
                        if (rd[key] is DataTemplate)
                            return ((DataTemplate)rd[key]);
                }

                if (CoreThemeResourceDictionary.ContainsKey(key))
                    if (CoreThemeResourceDictionary[key] is DataTemplate)
                        return ((DataTemplate)CoreThemeResourceDictionary[key]);

            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static string GetXamlFromResourcePath(string path)
        {

            if ((path ?? "").Trim() == "") return "";

            if (path.StartsWith("#"))
            {
                try
                {
                    string file = path.Replace("#", "");
                    var sampleDataFile = StorageFile
                        .GetFileFromApplicationUriAsync(new Uri(
                            $"{BaseDefaultThemePath}Theme/Default/PresetThemes/{file}.xaml"))
                        .AsTask().Result;
                    var sampleData = (FileIO.ReadTextAsync(sampleDataFile).AsTask().Result ?? "");
                    return sampleData;
                }
                catch (Exception e)
                {
                }
            }
            else if (path.StartsWith("@"))
            {
                try
                {
                    string file = path.Replace("@", "");
                    var sampleDataFile = StorageFile
                        .GetFileFromApplicationUriAsync(new Uri(
                            $"{BaseDefaultThemePath}Theme/Default/PresetStyles/{file}.xaml"))
                        .AsTask().Result;
                    var sampleData = (FileIO.ReadTextAsync(sampleDataFile).AsTask().Result ?? "");
                    return sampleData;
                }
                catch (Exception e)
                {
                }
            }



            try
            {
                string k = File.ReadAllText(path);
                if ((k ?? "") != "")
                {
                    return k;
                }
            }
            catch (Exception e)
            {

            }

            return "";
        }


    }
}
