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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public class ThreadFlipModel : IFlipModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private ObservableCollection<Thread> _threadList;
        public ObservableCollection<Thread> ThreadList
        {
            get { return _threadList; }
            set
            {
                _threadList = value;
                OnPropertyChanged("ThreadList");
            }
        }
        public ItaListPageViewModel.FlipRole Role { get; set; }
        public Ita Ita { get; set; }
        private FlipView MainFlip;

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

        private void CreateNextFlipPage()
        {
            var items = (ObservableCollection<IFlipModel>)MainFlip.ItemsSource;
            if (items.Count > MainFlip.SelectedIndex + 1)
            {
                var count = items.Count;
                for (var i = items.Count - 1; i > MainFlip.SelectedIndex; i--)
                {
                    items.RemoveAt(i);
                }
            }
            var newFLip = new PostsFlipModel(MainFlip, SelectedItem);
            items.Add(newFLip);
            //Task.Delay(300).Wait();
            //MainFlip.SelectedIndex++;
            //newFLip.ReloadPostList();
            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            MainFlip.SelectedItem = newFLip;
        }

        public ThreadFlipModel(FlipView MainFlip, Ita Ita)
        {
            this.MainFlip = MainFlip;
            this.Ita = Ita;
            this.Role = ItaListPageViewModel.FlipRole.ThreadList;
            ThreadList = new ObservableCollection<Thread>();
            ReloadThreadList();
        }

        public async void ReloadThreadList()
        {
            var client = new My2chClient();
            var store = new StoreToFiles();
            try
            {
                await client.GetThreadList(Ita, ThreadList);
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "スレッド一覧の取得中にエラーが発生しました。");
                await dialog.ShowAsync();
            }
        }
    }
}
