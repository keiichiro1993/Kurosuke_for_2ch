using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.ViewModels
{
    public interface IFlipModel
    {
        ItaListPageViewModel.FlipRole Role { get; set; }
    }
}
