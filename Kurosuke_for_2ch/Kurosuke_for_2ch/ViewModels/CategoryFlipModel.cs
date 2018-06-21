using Kurosuke_for_2ch.Models;
using Kurosuke_Universal.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public class CategoryFlipModel : IFlipModel
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
        public ObservableCollection<Category> CategoryList { get; set; }

        private FlipView MainFlip;

        private Category _selectedItem;
        public Category SelectedItem
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
            var newFlip = new ItaFlipModel(MainFlip, SelectedItem);
            items.Add(newFlip);
            //Task.Delay(300).Wait();
            //MainFlip.SelectedIndex++;
            MainFlip.GetBindingExpression(FlipView.ItemsSourceProperty).UpdateSource();
            MainFlip.SelectedItem = newFlip;
        }

        public CategoryFlipModel(FlipView MainFlip)
        {
            this.MainFlip = MainFlip;
            this.Role = ItaListPageViewModel.FlipRole.CategoryList;
            CategoryList = new ObservableCollection<Category>();
        }


        /*private ICommand _listSelectionChangedCommand;
        public ICommand ListSelectionChangedCommand
        {
            get
            {
                if (_listSelectionChangedCommand == null) _listSelectionChangedCommand = new DelegateCommand<object>(ListSelectionCHangedImpliment);
                return _listSelectionChangedCommand;
            }
        }

        private void ListSelectionCHangedImpliment(object obj)
        {
            var item = (Category)obj;
        }*/
    }
}
