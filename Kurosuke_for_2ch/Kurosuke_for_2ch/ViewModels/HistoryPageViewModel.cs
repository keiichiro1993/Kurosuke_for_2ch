using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public class HistoryPageViewModel
    {
        public FlipView MainFlip { get; set; }
        public ObservableCollection<IHistoryPageFlipModel> FlipCollection { get; set; }
        public HistoryPageViewModel(FlipView MainFlip)
        {
            this.MainFlip = MainFlip;
            FlipCollection = new ObservableCollection<IHistoryPageFlipModel>();
            FlipCollection.Add(new HistoryThreadsFlipModel(MainFlip));
        }
    }
}
