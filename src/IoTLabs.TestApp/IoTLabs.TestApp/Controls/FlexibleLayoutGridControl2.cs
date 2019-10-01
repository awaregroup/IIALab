using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using AwareThings.WinIoTCoreServices.Core.Common;
using AwareThings.WinIoTCoreServices.Core.Controls;
using AwareThings.WinIoTCoreServices.Core.DisplayItems;
using AwareThings.WinIoTCoreServices.Core.ViewModels;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace AwareThings.WinIoTCoreServices.Controls
{
    public class FlexibleLayoutGridControl2 : Grid
    {
        //private Grid _hostGrid;
        //private Grid _headerGrid;

        private Grid _hostGrid;
        //private StackPanel _SelectorGrid;

        public FlexibleLayoutGridControl2() : base()
        {
            _hostGrid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            //_SelectorGrid = new StackPanel() { Width=60, VerticalAlignment = VerticalAlignment.Stretch, Background = new SolidColorBrush(Colors.Red), Orientation = Orientation.Vertical};

            _hostGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            _hostGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            _hostGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //this.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            //this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            this.Children.Add(_hostGrid);
            //this.Children.Add(_SelectorGrid);

            Grid.SetColumn(_hostGrid, 1);
            //Grid.SetColumn(_SelectorGrid, 0);
        }

        public static readonly DependencyProperty PanelTemplateSelectorProperty = DependencyProperty.Register(
            "PanelTemplateSelector", typeof(DataTemplateSelector), typeof(FlexibleLayoutGridControl2), new PropertyMetadata(default(DataTemplateSelector)));

        public DataTemplateSelector PanelTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(PanelTemplateSelectorProperty); }
            set { SetValue(PanelTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(
            "HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(FlexibleLayoutGridControl2), new PropertyMetadata(default(DataTemplateSelector)));

        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
            set { SetValue(HeaderTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty SourceLayoutProperty = DependencyProperty.Register(
            "SourceLayout", typeof(BindableLayout), typeof(FlexibleLayoutGridControl2), new PropertyMetadata(default(BindableLayout), SourceLayoutPropertyChangedCallback));

        public BindableLayout SourceLayout
        {
            get { return (BindableLayout)GetValue(SourceLayoutProperty); }
            set { SetValue(SourceLayoutProperty, value); }
        }

        private static void InjectMergedDictionary(ResourceDictionary parentResourceDictionary, string rdXaml)
        {
            try
            {
                if (rdXaml != "")
                {
                    ResourceDictionary rd = (ResourceDictionary)XamlReader.Load(rdXaml);
                    if (rd != null)
                        parentResourceDictionary.MergedDictionaries.Add(rd);
                }
            }
            catch (Exception e)
            {

            }
        }



        //private static ResourceDictionary GetStyleDictionary(string styleUri, string rdThemeXaml, string rdTemplateXaml)
        //{
        //    ResourceDictionary rd = new ResourceDictionary();
        //    var rdStyles = new ResourceDictionary() { Source = new Uri("ms-appx:///AwareThings.WinIoTCoreServices.Core/Theme/Default/SkinStyles.xaml", UriKind.RelativeOrAbsolute) };

        //    try
        //    {
        //        InjectMergedDictionary(rdStyles, rdThemeXaml);
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    try
        //    {
        //        if (rdTemplateXaml != "")
        //        {
        //            var rdn = (ResourceDictionary)XamlReader.Load(rdTemplateXaml);
        //            if (rdn != null)
        //            {
        //                rdn.MergedDictionaries.Add(rdStyles);
        //                rd = rdn;
        //            }
        //            else
        //            {
        //                rd = rdStyles;
        //            }
        //        }
        //        else
        //        {
        //            rd = rdStyles;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //    }

        //    return rd;
        //}


        public static string BuildPanelStylingXaml(List<string> panelStylingFlags)
        {
            StringBuilder sbXaml = new StringBuilder();
            try
            {
                if (panelStylingFlags == null) return "";
                if (panelStylingFlags.Count == 0) return "";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");


                List<string> cleanStylingFlags = panelStylingFlags.Select(o => (o.Trim().ToLower())).ToList();

                bool hasElements = false;

                if (cleanStylingFlags.Any(o => o.StartsWith("border") || o.StartsWith("rounded")))
                {
                    string cornerRadius = "";
                    string borderThickness = "";

                    if (cleanStylingFlags.Contains("rounded") || cleanStylingFlags.Contains("roundedmediumlarge") || cleanStylingFlags.Contains("rounded")) cornerRadius = "6,6,6,6";
                    else if (cleanStylingFlags.Contains("roundednone")) cornerRadius = "0,0,0,0";
                    else if (cleanStylingFlags.Contains("roundedbottom")) cornerRadius = "0,0,6,6";
                    else if (cleanStylingFlags.Contains("roundedtop")) cornerRadius = "6,6,0,0";
                    else if (cleanStylingFlags.Contains("roundedbottomright")) cornerRadius = "0,0,6,0";
                    else if (cleanStylingFlags.Contains("roundedbottomleft")) cornerRadius = "0,0,0,6";
                    else if (cleanStylingFlags.Contains("roundedtopright")) cornerRadius = "0,6,0,0";
                    else if (cleanStylingFlags.Contains("roundedtopleft")) cornerRadius = "0,6,0,0";
                    else if (cleanStylingFlags.Contains("roundedcorners")) cornerRadius = "6,0,6,0";
                    else if (cleanStylingFlags.Contains("roundedcornersreverse")) cornerRadius = "0,6,0,6";

                    if (cleanStylingFlags.Contains("roundedmedium")) cornerRadius = cornerRadius.Replace("6", "9");
                    else if (cleanStylingFlags.Contains("roundedlarge")) cornerRadius = cornerRadius.Replace("6", "12");

                    if (cleanStylingFlags.Contains("border") || cleanStylingFlags.Contains("bordermedium") || cleanStylingFlags.Contains("borderlarge")) borderThickness = "2";
                    else if (cleanStylingFlags.Contains("bordertop")) borderThickness = "0,2,0,0";
                    else if (cleanStylingFlags.Contains("borderbottom")) borderThickness = "0,0,0,2";
                    else if (cleanStylingFlags.Contains("bottomright")) borderThickness = "0,0,2,0";
                    else if (cleanStylingFlags.Contains("bottomleft")) borderThickness = "2,0,0,0";
                    else if (cleanStylingFlags.Contains("bordertopbottom")) borderThickness = "0,2,0,2";
                    else if (cleanStylingFlags.Contains("bottomleftright")) borderThickness = "2,0,2,0";

                    if (cleanStylingFlags.Contains("bordermedium")) borderThickness = borderThickness.Replace("2", "4");
                    else if (cleanStylingFlags.Contains("borderlarge")) borderThickness = borderThickness.Replace("2", "6");

                    //xDoc.DocumentElement.PrependChild();

                    if ((cornerRadius != "") || (borderThickness != ""))
                    {
                        hasElements = true;
                        sb.AppendLine("\t<Style x:Key=\"SkinPanelOuterHostStyle\" TargetType=\"Border\"  BasedOn=\"{StaticResource SkinPanelOuterHostBaseStyle}\">");
                        sb.AppendLine($"\t\t<Setter Property=\"Background\" Value=\"{{StaticResource SkinPanelBackgroundBrush}}\"/>");
                        if (cornerRadius != "")
                            sb.AppendLine($"\t\t<Setter Property=\"CornerRadius\" Value=\"{cornerRadius}\" />");
                        if (borderThickness != "")
                        {
                            sb.AppendLine("\t\t<Setter Property=\"BorderBrush\" Value=\"{StaticResource SkinPanelOutlineBrush}\"/>");
                            sb.AppendLine($"\t\t<Setter Property=\"BorderThickness\" Value=\"{borderThickness}\" />");
                        }
                        else
                        {
                            sb.AppendLine($"\t\t<Setter Property=\"BorderThickness\" Value=\"0\" />");
                        }
                        sb.AppendLine("\t</Style>");
                    }
                }

                if (cleanStylingFlags.Any(o => o.StartsWith("header") || o.StartsWith("text")))
                {
                    bool hasHeaderOverride = false;
                    if (cleanStylingFlags.Contains("headerbackground"))
                    {
                        hasHeaderOverride = true;
                        hasElements = true;
                        sb.AppendLine("\t<Style x:Key=\"PanelHeaderBorderStyle\" TargetType=\"Border\"  BasedOn=\"{StaticResource PanelHeaderBorderBaseStyle}\">");
                        sb.AppendLine("\t\t<Setter Property=\"Background\" Value=\"{StaticResource SkinPanelHeaderBackgroundBrush}\"/>");
                        sb.AppendLine("\t</Style>");
                    }

                    string textModifications = "";

                    if (cleanStylingFlags.Contains("textright"))
                        textModifications = textModifications + " HorizontalAlignment=\"Right\"";
                    else if (cleanStylingFlags.Contains("textcenter"))
                        textModifications = textModifications + " HorizontalAlignment=\"Center\"";
                    else
                        textModifications = textModifications + " HorizontalAlignment=\"Left\"";

                    if (cleanStylingFlags.Contains("textmedium"))
                        textModifications = textModifications + " FontSize=\"20\"";
                    else if (cleanStylingFlags.Contains("textlarge"))
                        textModifications = textModifications + " FontSize=\"24\"";

                    if (cleanStylingFlags.Contains("textnormal"))
                        textModifications = textModifications + " FontWeight=\"Normal\"";
                    else if (cleanStylingFlags.Contains("textlight"))
                        textModifications = textModifications + " FontWeight=\"SemiLight\"";

                    if (hasHeaderOverride || textModifications != "")
                    {
                        hasElements = true;
                        sb.AppendLine("\t<DataTemplate x:Key=\"PanelTemplateHeader\">");
                        sb.AppendLine("\t\t<Border Style=\"{StaticResource PanelHeaderBorderStyle}\">");
                        sb.AppendLine("\t\t\t<TextBlock Style=\"{StaticResource PanelHeaderTextBlockStyle}\" Text=\"{Binding}\" Margin=\"0,8\" " + textModifications + " />");
                        sb.AppendLine("\t\t</Border>");
                        sb.AppendLine("\t</DataTemplate>");
                    }

                }

                sb.AppendLine("</ResourceDictionary>");

                if (hasElements)
                    return sb.ToString();
            }
            catch (Exception e)
            {

            }
            return "";
        }

        public static ElementTheme UpdateColorThemeResources(string rdTemplateXaml)
        {
            ElementTheme theme = ElementTheme.Dark;
            Dictionary<string, Color> colors = new Dictionary<string, Color>();
            try
            {

                var rdn = (ResourceDictionary)XamlReader.Load(rdTemplateXaml);
                if (rdn != null)
                {
                    if (rdn.Keys != null)
                    {
                        foreach (var ky in rdn.Keys)
                        {
                            if (rdn[ky] is SolidColorBrush)
                            {
                                colors.Add((string)ky, ((SolidColorBrush)rdn[ky]).Color);

                            }
                            else if (ky.ToString() == "SkinTheme")
                            {
                                if (rdn[ky] is ElementTheme)
                                {
                                    theme = (ElementTheme)rdn[ky];
                                }


                            }
                        }
                    }
                }

                ResourceDictionary rd = Application.Current.Resources.MergedDictionaries[0].MergedDictionaries[0].MergedDictionaries[0];

                foreach (string ky in colors.Keys)
                {
                    if (rd.ContainsKey(ky))
                        if (rd[ky] is SolidColorBrush)
                        {
                            (rd[ky] as SolidColorBrush).Color = colors[ky];
                        }
                }


                try
                {
                    rd["SkinTheme"] = theme;
                }
                catch (Exception e)
                {
                    string p = e.Message;
                }


            }
            catch (Exception e)
            {

            }
            return theme;
        }


        private static void SourceLayoutPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var ctl = (FlexibleLayoutGridControl2)dependencyObject;
                BindableLayout bindableLayout = (BindableLayout)dependencyPropertyChangedEventArgs.NewValue;
                ctl._hostGrid.Children.Clear();
                if (bindableLayout == null) return;

                //TODO: Replace this with call to get a constant for the .xaml rd path.
                ctl.Resources.MergedDictionaries.Clear();

                string rdThemeXaml = XamlHelper2.GetXamlFromResourcePath(bindableLayout.CurrentSkin.SkinOverrideXamlUri);
                string rdTemplateXaml = XamlHelper2.GetXamlFromResourcePath(bindableLayout.CurrentSkin.TemplateOverrideXamlUri);

                bool hasPanelStylingFlags = false;
                bool hasOverrideTheme = (rdTemplateXaml ?? "") != "";

                if (((rdTemplateXaml ?? "") == "") && (bindableLayout.CurrentSkin.PanelStylingFlags != null))
                    if (bindableLayout.CurrentSkin.PanelStylingFlags.Count > 0)
                    {
                        hasPanelStylingFlags = true;
                    }

                bindableLayout.Theme = UpdateColorThemeResources(rdThemeXaml);

                if (hasPanelStylingFlags)
                {
                    rdTemplateXaml = BuildPanelStylingXaml(bindableLayout.CurrentSkin.PanelStylingFlags);
                }

                // ResourceDictionary rdStyles = GetStyleDictionary("ms-appx:///AwareThings.WinIoTCoreServices.Core/Theme/Default/SkinStyles.xaml", rdThemeXaml, rdTemplateXaml);
                //rd2.MergedDictionaries.Add(rdStyles);

                try
                {
                    ctl.Resources.Clear();
                    ctl.Resources.MergedDictionaries.Clear();

                    if (rdTemplateXaml != "")
                    {
                        var template = XamlHelper2.ParseXamlResourceDictionary(rdTemplateXaml);
                        if (template != null)
                        {
                            foreach (var ky in template.Keys)
                                ctl.Resources.Add((string)ky, template[(string)ky]);
                            //ctl.Resources.MergedDictionaries.Add(template);
                        }
                    }

                    //ctl.Resources.MergedDictionaries.Add(rd2);

                    var sdp = ViewModelLocator.Current.DisplayPanelFactoryService;
                    if (sdp != null)
                        if (sdp.ProvidorTemplateResourceFiles != null)
                            if (sdp.ProvidorTemplateResourceFiles.Count > 0)
                            {
                                foreach (var templateUri in sdp.ProvidorTemplateResourceFiles)
                                {
                                    try
                                    {
                                        var rdNew = new ResourceDictionary() { Source = templateUri };
                                        ctl.Resources.MergedDictionaries.Add(rdNew);
                                    }
                                    catch (Exception errTemplate)
                                    {
                                        string msg = errTemplate.Message;
                                    }

                                }
                            }
                }
                catch (Exception e)
                {

                }

                ctl.PanelTemplateSelector = XamlHelper2.ResolveDataTemplateSelectorFromResources("SkinTemplateSelector");
                ctl.HeaderTemplateSelector = XamlHelper2.ResolveDataTemplateSelectorFromResources("HeaderTemplateSelector");

                try
                {
                    foreach (var skn in bindableLayout.SkinPanels)
                        try
                        {
                            if (skn.HasOverrideTemplate)
                                skn.InitializeOverrideItemTemplate(ctl.Resources);
                        }
                        catch (Exception e)
                        {
                            string v = e.Message;
                        }
                }
                catch (Exception e)
                {

                }

                try
                {
                    ctl.Background = XamlHelper2.ResolveBrushFromResources("SkinBackgroundBrush");// ResolveBrushFromResources(rdStyles, (rdTemplateXaml!=""), (rdThemeXaml != ""), "SkinBackgroundBrush");
                }
                catch (Exception e)
                {

                }

                if (bindableLayout.CurrentSkin != null)
                {
                    ContentPresenter cp = new ContentPresenter()
                    {
                        Content = bindableLayout.CurrentSkin,
                        ContentTemplateSelector = ctl.HeaderTemplateSelector,
                        //ContentTemplate = .SelectTemplate(item),
                        Visibility = bindableLayout.CurrentSkin.ShowPageHeader ? Visibility.Visible : Visibility.Collapsed,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Stretch
                    };

                    if (bindableLayout.CurrentSkin.ShowPageHeader)
                    {
                        Grid.SetRow(cp, bindableLayout.CurrentSkin.ShowPageHeaderTop ? 0 : 2);
                        ctl._hostGrid.Children.Add(cp);
                    }
                }

                if (bindableLayout.CurrentSkin.Layouts > 0)
                {
                    if (bindableLayout.CurrentSkin.Layouts == 1)
                    {
                        var grd = FlexibleLayoutGridControl2.GenerateLayoutGrid(bindableLayout, ctl.PanelTemplateSelector, 0, false);
                        grd.Margin = bindableLayout.CurrentSkin.IsProvisioningSkin ? new Thickness(0) : new Thickness(14);
                        Grid.SetRow(grd, 1);
                        ctl._hostGrid.Children.Add(grd);
                    }
                    else if (bindableLayout.CurrentSkin.CurrentLayoutMode == LayoutMode.Tabs)
                    {
                        //ctl._hostGrid.Padding = new Thickness(28,14, 28, 14);
                        TabView tv = new TabView() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Background = ctl.Background, Margin = new Thickness(28, 14, 28, 14) };
                        Grid.SetRow(tv, 1);
                        ctl._hostGrid.Children.Add(tv);

                        tv.Resources.Add("TabViewItemHeaderBackground", XamlHelper2.ResolveBrushFromResources("SkinPanelBackgroundBrush"));// ResolveBrushFromResources(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinPanelBackgroundBrush"));
                        tv.Resources.Add("TabViewItemHeaderForeground", XamlHelper2.ResolveBrushFromResources("SkinForegroundBrush")); //ResolveBrushFromResources(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinForegroundBrush"));
                        tv.Resources.Add("TabViewItemHeaderBackgroundSelected", XamlHelper2.ResolveBrushFromResources("SkinForegroundBrush")); //ResolveBrushFromResources(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinForegroundBrush"));
                        tv.Resources.Add("TabViewItemHeaderForegroundSelected", XamlHelper2.ResolveBrushFromResources("SkinPanelBackgroundBrush")); //ResolveBrushFromResources(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinPanelBackgroundBrush"));
                        tv.Resources.Add("TabViewSelectionIndicatorForeground", XamlHelper2.ResolveBrushFromResources("SkinPrimaryAccentBrush")); //ResolveBrushFromResources(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinPrimaryAccentBrush"));

                        for (int i = 0; i < bindableLayout.CurrentSkin.Layouts; i++)
                        {

                            TabViewItem tvi = new TabViewItem()
                            { Header = $"Layout {(i + 1)}", Icon = new SymbolIcon(Symbol.Document) };

                            try
                            {
                                if (bindableLayout.CurrentSkin.LayoutTabs != null)
                                {
                                    var p = bindableLayout.CurrentSkin.LayoutTabs.FirstOrDefault(o => o.Index == i);
                                    if (p != null)
                                    {
                                        tvi.Header = p.Title;
                                        tvi.Icon = p.FontIcon;
                                    }
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            tvi.Content = FlexibleLayoutGridControl2.GenerateLayoutGrid(bindableLayout, ctl.PanelTemplateSelector, i, false);
                            (tvi.Content as FrameworkElement).Margin = new Thickness(-10, 0, -10, 0);
                            tv.Items.Add(tvi);
                        }
                    }
                    else if (bindableLayout.CurrentSkin.CurrentLayoutMode == LayoutMode.Blades)
                    {

                        var brForeground = XamlHelper2.ResolveBrushFromResources("SkinForegroundBrush");//(rdStyles, (rdTemplateXaml != ""), (rdThemeXaml != ""), "SkinForegroundBrush");
                        var brAlt = new SolidColorBrush(Color.FromArgb(10, 255, 255, 255));

                        if (brForeground != null)
                            if (brForeground is SolidColorBrush)
                                if ((((brForeground as SolidColorBrush).Color.R) + ((brForeground as SolidColorBrush).Color.R) + ((brForeground as SolidColorBrush).Color.R)) < 200)
                                {
                                    brAlt = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
                                }

                        //ctl._hostGrid.Padding = new Thickness(0);
                        ScrollViewer scr = new ScrollViewer() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, HorizontalScrollMode = ScrollMode.Enabled, VerticalScrollMode = ScrollMode.Disabled, VerticalContentAlignment = VerticalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
                        Grid.SetRow(scr, 1);
                        ctl._hostGrid.Children.Add(scr);

                        StackPanel stck = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment = HorizontalAlignment.Left };
                        scr.Content = stck;
                        //scr.SizeChanged += delegate(object sender, SizeChangedEventArgs args)
                        //{
                        //    try
                        //    {
                        //        if (scr.Content != null)
                        //            if (scr.Content is StackPanel)
                        //                if ((scr.Content as StackPanel).Children!=null)
                        //                    foreach (var c)
                        //                {

                        //                }

                        //    }
                        //    catch (Exception e)
                        //    {

                        //    }
                        //};

                        for (int i = 0; i < bindableLayout.CurrentSkin.Layouts; i++)
                        {

                            Grid grd = new Grid()
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Background = (((i % 2) == 1) ?
                                    brAlt
                                    : null),
                                Padding = new Thickness(14)
                            };



                            grd.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                            grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                            var flx = FlexibleLayoutGridControl2.GenerateLayoutGrid(bindableLayout, ctl.PanelTemplateSelector, i, true);
                            grd.Children.Add(flx);
                            Grid.SetRow(flx, 1);

                            scr.SizeChanged += delegate (object sender, SizeChangedEventArgs args)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    try
                                    {
                                        //if (((flx as FixedAspectRatioGridControl).Mode == FixedRatioMode.AdjustWidth) && (args.NewSize.Height != args.PreviousSize.Height))
                                        //    (flx as FixedAspectRatioGridControl).UpdateSize();
                                        //else if (((flx as FixedAspectRatioGridControl).Mode == FixedRatioMode.AdjustHeight) && (args.NewSize.Width != args.PreviousSize.Width))
                                        //    (flx as FixedAspectRatioGridControl).UpdateSize();
                                    }
                                    catch (Exception e)
                                    {
                                    }

                                });
                            };

                            var item = bindableLayout.CurrentSkin.LayoutTabs.FirstOrDefault(o => o.Index == i);

                            if (item != null)
                            {
                                StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal, Height = 32, Margin = new Thickness(10, 0, 0, 0) };
                                item.FontIcon.Foreground = brForeground;
                                sp.Children.Add(item.FontIcon);
                                sp.Children.Add(new TextBlock() { Text = item.Title, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 0, 0, 0), Foreground = brForeground });
                                grd.Children.Add(sp);
                                Grid.SetRow(sp, 0);
                            }
                            stck.Children.Add(grd);


                            ////bindableLayout.CurrentSkin.LayoutTabs.FirstOrDefault(o => o.Index == i);

                            //TabViewItem tvi = new TabViewItem()
                            //    { Header = $"Layout {(i + 1)}", Icon = new SymbolIcon(Symbol.Document), Background = new SolidColorBrush(Colors.Black) };

                            //try
                            //{
                            //    if (bindableLayout.CurrentSkin.LayoutTabs != null)
                            //    {
                            //        var p = bindableLayout.CurrentSkin.LayoutTabs.FirstOrDefault(o => o.Index == i);
                            //        if (p != null)
                            //        {
                            //            tvi.Header = p.Title;
                            //            tvi.Icon = p.FontIcon;
                            //        }
                            //    }
                            //}
                            //catch (Exception e)
                            //{

                            //}
                            //tvi.Content = FlexibleLayoutGridControl2.GenerateLayoutGrid(bindableLayout, ctl.PanelTemplateSelector, i);
                            //(tvi.Content as FrameworkElement).Margin = new Thickness(-10, 0, -10, 0);
                            //tv.Items.Add(tvi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }


        private static Grid GenerateLayoutGrid(BindableLayout bindableLayout, DataTemplateSelector panelTemplateSelector, int LayoutIndex, bool useFixedAspectRatioGrid)
        {

            //Resolve Styles
            ResourceDictionary rdStyleOverrides = null;
            string xamlStyling = BuildPanelStylingXaml(bindableLayout.CurrentSkin.PanelStylingFlags);
            if (xamlStyling != null)
                rdStyleOverrides = XamlHelper2.ParseXamlResourceDictionary(xamlStyling);

            Style stSkinPanelOuterHostStyle = XamlHelper2.ResolveStyleFromResourceDictionary(rdStyleOverrides, "SkinPanelOuterHostStyle");
            Style stSkinPanelHostStyle = XamlHelper2.ResolveStyleFromResourceDictionary(rdStyleOverrides, "SkinPanelHostStyle");
            DataTemplate dtPanelTemplateHeader = XamlHelper2.ResolveStylingDataTemplateFromResourceDictionary(rdStyleOverrides, "PanelTemplateHeader"); ;


            //Style stSkinPanelOuterHostStyle = BuildSkinPanelOuterHostStyle(bindableLayout.CurrentSkin.PanelStylingFlags);

            Grid grd = null;
            if (useFixedAspectRatioGrid)
            {
                double div = Math.Max((bindableLayout.CurrentSkin.VerticalResolution - bindableLayout.CurrentSkin.TitleHeight), 10);
                double ratio = (bindableLayout.CurrentSkin.HorizontalResolution / div);
                ratio = Math.Clamp(ratio, 0.05, 4);
                grd = new FixedAspectRatioGridControl() { Mode = FixedRatioMode.AdjustWidth, AspectRatio = ratio };
            }
            else
            {
                grd = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            }

            //Grid.SetRow(grd, 1);

            try
            {

                for (int z = 0; z < bindableLayout.CurrentSkin.LayoutColumns; z++)
                {
                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
                for (int z = 0; z < bindableLayout.CurrentSkin.LayoutRows; z++)
                {
                    grd.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }

                
                foreach (var item in bindableLayout.SkinPanels)
                {

                    if ((item.IsVisible) && (LayoutIndex == item.LayoutIndex))
                    {

                        bool hasStylingFlags = false;
                        Style stItemSkinPanelOuterHostStyle = null;
                        Style stItemSkinPanelHostStyle = null;
                        DataTemplate dtItemPanelTemplateHeader = null;

                        if (item.PanelStylingFlags != null)
                            if (item.PanelStylingFlags.Count > 0)
                            {
                                ResourceDictionary rdItemStyleOverrides = null;
                                string xamlItemStyling = BuildPanelStylingXaml(item.PanelStylingFlags);
                                if (xamlItemStyling != null)
                                {
                                    rdItemStyleOverrides = XamlHelper2.ParseXamlResourceDictionary(xamlItemStyling);
                                    if (rdItemStyleOverrides != null)
                                    {
                                        hasStylingFlags = true;
                                        stItemSkinPanelOuterHostStyle = XamlHelper2.ResolveStyleFromResourceDictionary(rdItemStyleOverrides, "SkinPanelOuterHostStyle");
                                        stItemSkinPanelHostStyle = XamlHelper2.ResolveStyleFromResourceDictionary(rdItemStyleOverrides, "SkinPanelHostStyle");
                                        dtItemPanelTemplateHeader = XamlHelper2.ResolveStylingDataTemplateFromResourceDictionary(rdItemStyleOverrides, "PanelTemplateHeader");
                                    }
                                }
                            }

                        if (hasStylingFlags)
                        {
                            item.SkinPanelOuterHostStyle = stItemSkinPanelOuterHostStyle ?? stSkinPanelOuterHostStyle;
                            item.SkinPanelHostStyle = stItemSkinPanelHostStyle ?? stSkinPanelHostStyle;
                            item.ItemHeaderDataTemplate = dtItemPanelTemplateHeader ?? dtPanelTemplateHeader;
                        }
                        else
                        {
                            item.SkinPanelOuterHostStyle = stSkinPanelOuterHostStyle;
                            item.SkinPanelHostStyle = stSkinPanelHostStyle;
                            item.ItemHeaderDataTemplate = dtPanelTemplateHeader;
                        }

                        item.RequestedTheme = bindableLayout.Theme;

                        ContentPresenter cp = new ContentPresenter()
                        {
                            Content = item,
                            ContentTemplate = panelTemplateSelector.SelectTemplate(item),
                            Visibility = item.IsVisible ? Visibility.Visible : Visibility.Collapsed,
                            HorizontalContentAlignment = HorizontalAlignment.Stretch,
                            VerticalContentAlignment = VerticalAlignment.Stretch
                        };

                        Grid.SetRow(cp, item.Row);
                        Grid.SetColumn(cp, item.Column);
                        Grid.SetRowSpan(cp, item.RowSpan);
                        Grid.SetColumnSpan(cp, item.ColumnSpan);

                        grd.Children.Add(cp);
                        //grd.UpdateLayout();
                    }
                }

            }
            catch (Exception e)
            {

            }
            return grd;
        }



    }
}
