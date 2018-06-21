using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kurosuke_for_2ch.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class RootPage : Page
    {
        public RootPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch)
            {
                Splitter.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            MainContentFrame.Navigate(typeof(Pages.ItaListPage));
        }

        private void ToItaListPage(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame != null)
            {
                MainContentFrame.Navigate(typeof(Pages.ItaListPage));
            }
        }

        private void ToHistoryPage(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame != null)
            {
                MainContentFrame.Navigate(typeof(Pages.HistoryPage));
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch)
            {
                Splitter.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else
            {
                Splitter.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
        }
    }
}
