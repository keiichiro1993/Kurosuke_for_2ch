using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using Windows.Web.Http;
using System.Threading.Tasks;
using Portable.Text;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.ViewModels;
using Kurosuke_for_2ch.Utils;
using Windows.UI.Popups;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kurosuke_for_2ch.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class ItaListPage : Page
    {
        public ItaListPage()
        {
            this.InitializeComponent();
        }

        public ItaListPageViewModel viewModel;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                viewModel = new ItaListPageViewModel(MainFlip);
                var categoryFlip = new CategoryFlipModel(MainFlip);
                viewModel.FlipList.Add(categoryFlip);

                var store = new StoreToFiles();
                var categories = await store.LoadCategories();
                var list = new ObservableCollection<Category>();
                if (categories == null || categories.Count == 0)
                {
                    var client = new My2chClient();
                    try
                    {
                        var getcategory = await client.GetItaList();
                        await store.AddCategories(getcategory);
                    }
                    catch (Exception ex)
                    {
                        var message = new MessageDialog("板一覧を取得するため、インターネットに接続できる状態で起動してください。", "初回起動ですか");
                        await message.ShowAsync();
                        return;
                    }
                }

                foreach (var category in categories)
                {
                    list.Add(category);
                }
                categoryFlip.CategoryList = list;

                MainGrid.DataContext = viewModel;

            }
        }

        private async Task RefreshItaList()
        {
            var client = new My2chClient();
            try
            {
                var categories = await client.GetItaList();
                var store = new StoreToFiles();
                var changed = await store.AddOrUpdateCategories(categories);
                if (changed.Count > 0)
                {
                    string changedText = "";
                    foreach (var ita in changed)
                    {
                        changedText += (ita.Name + ", ");
                    }
                    var dialog = new MessageDialog(changedText, "以下の板のURLが変更されました。");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "板一覧の取得中にエラーが発生しました。");
                await dialog.ShowAsync();
            }
        }

        private void ToCreatePostPageButtonClicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreatePostPage), ((PostsFlipModel)MainFlip.SelectedValue).Thread);
        }

        private void ScrollToTheEnd(object sender, RoutedEventArgs e)
        {
            var container = MainFlip.ContainerFromIndex(3);
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
            var posts = MainFlip.Items[3] as PostsFlipModel;
            await posts.ReloadPostList();
            button.IsEnabled = true;
        }

        private async void RefreshItaListButton(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarButton;
            button.IsEnabled = false;

            await RefreshItaList();
            var store = new StoreToFiles();
            var categories = await store.LoadCategories();

            var cateFlip = MainFlip.Items[0] as CategoryFlipModel;
            cateFlip.CategoryList.Clear();

            foreach (var category in categories)
            {
                cateFlip.CategoryList.Add(category);
            }

            button.IsEnabled = true;
        }
    }
}
