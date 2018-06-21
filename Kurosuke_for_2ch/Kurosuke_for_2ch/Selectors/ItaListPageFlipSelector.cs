using Kurosuke_for_2ch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kurosuke_for_2ch.Selectors
{
    public class ItaListPageFlipSelector : DataTemplateSelector
    {
        public DataTemplate CategoryListTemplate { get; set; }
        public DataTemplate ItaListTemplate { get; set; }
        public DataTemplate ThreadListTemplate { get; set; }
        public DataTemplate ThreadTemplate { get; set; }
        public ItaListPageFlipSelector()
        { }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var itemModel = (IFlipModel)item;
            switch (itemModel.Role)
            {
                case ItaListPageViewModel.FlipRole.CategoryList:
                    return CategoryListTemplate;
                case ItaListPageViewModel.FlipRole.ItaList:
                    return ItaListTemplate;
                case ItaListPageViewModel.FlipRole.ThreadList:
                    return ThreadListTemplate;
                case ItaListPageViewModel.FlipRole.Thread:
                    return ThreadTemplate;
                default:
                    return null;
            }
        }
    }
}
