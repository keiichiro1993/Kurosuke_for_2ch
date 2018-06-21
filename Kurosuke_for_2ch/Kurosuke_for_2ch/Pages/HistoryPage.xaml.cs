using Kurosuke_for_2ch.Utils;
using Kurosuke_for_2ch.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class HistoryPage : Page
    {
        public HistoryPageViewModel viewModel { get; set; }
        public HistoryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var item = MainFlip.SelectedItem as IHistoryPageFlipModel;
                item.Refresh();
            }
            else
            {
                viewModel = new HistoryPageViewModel(MainFlip);
                MainGrid.DataContext = viewModel;
            }
        }

        private void ToCreatePostPageButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreatePostPage), ((HistoryPagePostsFlipModel)MainFlip.SelectedValue).thread);
        }

        private void ScrollToTheEnd(object sender, RoutedEventArgs e)
        {
            var container = MainFlip.ContainerFromIndex(MainFlip.SelectedIndex);
            var scrollViewer = FindChildrenObjectByType<ScrollViewer>(container);
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
        }

        public static T FindChildrenObjectByType<T>(DependencyObject root) where T : class
        {
            var MyQueue = new Queue<DependencyObject>();
            MyQueue.Enqueue(root);
            while (MyQueue.Count > 0)
            {
                DependencyObject current = MyQueue.Dequeue();
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    MyQueue.Enqueue(child);
                }
            }
            return null;
        }

        private async void RefreshPostList(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarButton;
            button.IsEnabled = false;
            var posts = MainFlip.SelectedItem as HistoryPagePostsFlipModel;
            await posts.Refresh();
            button.IsEnabled = true;
        }

        private void DeleteThreadFile(object sender, RoutedEventArgs e)
        {
            var flip = MainFlip.SelectedItem as HistoryPagePostsFlipModel;
            var thread = flip.thread;
            var store = new StoreToFiles();
            store.DeleteThread(thread);

            viewModel = new HistoryPageViewModel(MainFlip);
            MainGrid.DataContext = viewModel;
        }

        private async void RefreshAllThreads(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarButton;
            button.IsEnabled = false;
            if (MainFlip.Items.Count > 0)
            {
                var flip = viewModel.FlipCollection[0] as HistoryThreadsFlipModel;
                await flip.Refresh();
                viewModel = new HistoryPageViewModel(MainFlip);
                MainGrid.DataContext = viewModel;
            }
            button.IsEnabled = true;
        }
    }
}
