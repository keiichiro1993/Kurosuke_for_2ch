using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public class ItaFlipModel : IFlipModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private Ita _selectedItem;
        public Ita SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                CreateNextFlipPage();
            }
        }

        public ItaListPageViewModel.FlipRole Role { get; set; }
        private FlipView MainFlip { get; set; }
        public Category Category { get; set; }

        public ItaFlipModel(FlipView MainFlip, Category Category)
        {
            this.MainFlip = MainFlip;
            this.Role = ItaListPageViewModel.FlipRole.ItaList;
            this.Category = Category;
            ReloadItalist();
        }

        private void ReloadItalist()
        {
            var store = new StoreToFiles();
            this.Category.Itas = store.LoadItas(this.Category);
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
            var newFlip = new ThreadFlipModel(MainFlip, SelectedItem);
            items.Add(newFlip);
            //Task.Delay(300).Wait();
            //MainFlip.SelectedIndex++;
            //newFlip.ReloadThreadList();
            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            MainFlip.SelectedItem = newFlip;
        }
    }
}
