using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.ViewModels
{
    public interface IHistoryPageFlipModel
    {
        ItaListPageViewModel.FlipRole Role { get; set; }
        Task Refresh();
    }
}
