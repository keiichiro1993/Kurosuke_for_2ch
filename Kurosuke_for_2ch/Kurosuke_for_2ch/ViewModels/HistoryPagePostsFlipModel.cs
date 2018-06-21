using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Kurosuke_for_2ch.ViewModels
{
    public class HistoryPagePostsFlipModel : IHistoryPageFlipModel
    {
        public ItaListPageViewModel.FlipRole Role { get; set; }
        public Thread thread { get; set; }
        public FlipView MainFlip { get; set; }

        public HistoryPagePostsFlipModel(FlipView MainFlip, Thread thread)
        {
            Role = ItaListPageViewModel.FlipRole.Thread;
            this.thread = thread;
            this.MainFlip = MainFlip;
        }

        public async void Init(int count)
        {
            var store = new StoreToFiles();
            thread.MidokuCount = 0;
            await store.AddOrUpdateThreadInformation(thread);
            try
            {
                store.LoadPosts(thread);
                await Task.Delay(100);
                ScrollToCurrentOffset();
            }
            catch (Exception ex)
            {
                if (count > 10)
                {
                    var message = new MessageDialog(ex.Message, "スレッドの読み込み中にエラーが発生しました。");
                    await message.ShowAsync();
                }
                else
                {
                    var random = new Random();
                    await Task.Delay(random.Next(1, 5) * 100);
                    Init(++count);
                }
            }
        }

        private async void MainFlip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainFlip.Items.Count == 2)
            {
                var container = MainFlip.ContainerFromItem(this);
                var scrollViewer = FindChildrenObjectByType<ScrollViewer>(container);
                var store = new StoreToFiles();
                if (scrollViewer != null)
                {
                    var offset = (float)scrollViewer.VerticalOffset;
                    thread.CurrentOffset = offset;
                    try
                    {
                        await store.UpdateCurrentOffset(thread);
                    }
                    catch (Exception ex)
                    {
                        //ここでの例外はおそらく同時に複数のタスクが更新しようとしているだけ
                    }
                }
            }
        }

        public async Task Refresh()
        {
            var client = new My2chClient();
            try
            {
                var newPosts = await client.GetFullPostList(thread);
                var store = new StoreToFiles();
                var posts = await store.AddPosts(newPosts, thread);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "スレッドの取得中にエラーが発生しました。");
                await dialog.ShowAsync();
            }
        }

        public void ScrollToCurrentOffset()
        {
            var container = MainFlip.ContainerFromItem(this);
            var scrollViewer = FindChildrenObjectByType<ScrollViewer>(container);

            var store = new StoreToFiles();
            var offset = store.LoadOffset(thread);

            scrollViewer.ScrollToVerticalOffset(offset);
            this.MainFlip.SelectionChanged += MainFlip_SelectionChanged;

            //scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        /*private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollviewer = sender as ScrollViewer;
            var store = new StoreToFiles();
            var offset = (float)scrollviewer.VerticalOffset;
            store.SaveOffset(thread.thread, offset);
        }*/

        public static T FindChildrenObjectByType<T>(DependencyObject root) where T : class
        {
            if (root != null)
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
            }
            return null;
        }
    }
}
