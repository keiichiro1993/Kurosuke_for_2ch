using Kurosuke_for_2ch.Models;
using Kurosuke_Universal.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public class ItaListPageViewModel
    {
        public ObservableCollection<IFlipModel> FlipList { get; set; }
        private FlipView MainFlip;
        public enum FlipRole { CategoryList, ItaList, ThreadList, Thread };


        public ItaListPageViewModel(FlipView MainFlip)
        {
            FlipList = new ObservableCollection<IFlipModel>();
            this.MainFlip = MainFlip;
            MainFlip.SelectionChanged += MainFlip_SelectionChanged;
            MainFlip.DataContextChanged += MainFlip_DataContextChanged;
            MainFlip.SizeChanged += MainFlip_SizeChanged;
        }

        private void MainFlip_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            /*var collection = MainFlip.Items;
            MainFlip.SelectedItem = collection.Last();*/
        }

        private void MainFlip_DataContextChanged(Windows.UI.Xaml.FrameworkElement sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            
        }

        private void MainFlip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
