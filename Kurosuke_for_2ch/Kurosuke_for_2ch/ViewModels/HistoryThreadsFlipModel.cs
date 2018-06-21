using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    class HistoryThreadsFlipModel : IHistoryPageFlipModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
                CreateNextFlipPage();
            }
        }

        private async void CreateNextFlipPage()
        {
            var items = (ObservableCollection<IHistoryPageFlipModel>)MainFlip.ItemsSource;
            if (items.Count > MainFlip.SelectedIndex + 1)
            {
                var count = items.Count;
                for (var i = items.Count - 1; i > MainFlip.SelectedIndex; i--)
                {
                    items.RemoveAt(i);
                }
            }
            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            var newFlip = new HistoryPagePostsFlipModel(MainFlip, SelectedItem);
            items.Add(newFlip);
            //Task.Delay(300).Wait();
            //MainFlip.SelectedIndex++;
            //newFlip.Refresh();

            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            MainFlip.SelectedItem = newFlip;
            newFlip.Init(0);
            //await Task.Delay(200);
        }

        private FlipView MainFlip { get; set; }
        public ItaListPageViewModel.FlipRole Role { get; set; }
        public ObservableCollection<Thread> threadList { get; set; }

        public HistoryThreadsFlipModel(FlipView MainFlip)
        {
            this.MainFlip = MainFlip;
            threadList = new ObservableCollection<Thread>();
            Role = ItaListPageViewModel.FlipRole.ThreadList;
            var store = new StoreToFiles();
            store.LoadThreads(threadList);
        }
        public async Task Refresh()
        {
            var store = new StoreToFiles();
            var client = new My2chClient();
            foreach (var thread in threadList)
            {
                try
                {
                    var posts = await client.GetFullPostList(thread);
                    await store.AddPostsWithMidokuCount(posts, thread);
                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog(thread.Name + ": " + ex.Message, "スレッドの更新に失敗しました。");
                    await dialog.ShowAsync();
                }
            }

            store.LoadThreads(threadList);
        }
    }
}
