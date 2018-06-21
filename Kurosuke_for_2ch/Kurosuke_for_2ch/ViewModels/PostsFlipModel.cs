using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.Utils;
using Kurosuke_Universal.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Kurosuke_for_2ch.ViewModels
{
    public class PostsFlipModel : IFlipModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public ItaListPageViewModel.FlipRole Role { get; set; }

        private Thread _Thread;
        public Thread Thread
        {
            get { return _Thread; }
            set
            {
                _Thread = value;
                OnPropertyChanged("Thread");
            }
        }
        private FlipView MainFlip;

        private string _firstPostDate;
        public string FirstPostDate
        {
            get { return _firstPostDate; }
            set
            {
                _firstPostDate = value;
                OnPropertyChanged("FirstPostDate");
            }
        }

        private Thread _selectedItem;
        public Thread SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }


        public PostsFlipModel(FlipView MainFlip, Thread Thread)
        {
            this.MainFlip = MainFlip;
            this.Thread = Thread;
            this.Role = ItaListPageViewModel.FlipRole.Thread;
            InnerReloadPostList();
            //PostList = new ObservableCollection<Post>();
            FirstPostDate = "";
        }

        public void ScrollToCurrentOffset()
        {
            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            var container = MainFlip.ContainerFromItem(this);
            var scrollViewer = FindChildrenObjectByType<ScrollViewer>(container);

            var store = new StoreToFiles();
            var offset = store.LoadOffset(Thread);

            scrollViewer.ScrollToVerticalOffset(offset);
            this.MainFlip.SelectionChanged += MainFlip_SelectionChanged;

            //scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        private async void MainFlip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainFlip.Items.Count == 4)
            {
                var container = MainFlip.ContainerFromItem(this);
                var scrollViewer = FindChildrenObjectByType<ScrollViewer>(container);
                if (scrollViewer != null)
                {
                    var store = new StoreToFiles();
                    var offset = (float)scrollViewer.VerticalOffset;
                    Thread.CurrentOffset = offset;
                    await store.UpdateCurrentOffset(Thread);
                }
            }
        }

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

        private async void InnerReloadPostList()
        {
            await ReloadPostList();
        }

        public async Task ReloadPostList()
        {
            var store = new StoreToFiles();
            var client = new My2chClient();
            try
            {
                var tmpThread = await store.AddOrUpdateThreadInformation(Thread);
                var posts = await client.GetFullPostList(tmpThread);
                posts = await store.AddPosts(posts, tmpThread);
                tmpThread.Posts = posts;
                Thread = tmpThread;
                await Task.Delay(300);
                ScrollToCurrentOffset();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "スレッドの取得中にエラーが発生しました。");
                await dialog.ShowAsync();
            }
        }

        /*private ICommand _postPageButtonCommand;

        public ICommand ToPostPageButtonClicked
        {
            get
            {
                if (_postPageButtonCommand == null) _postPageButtonCommand = new DelegateCommand<object>(ToPostPageButton);
                return _postPageButtonCommand;
            }
        }

        private void ToPostPageButton(object obj)
        {
           
        }*/
    }
}
