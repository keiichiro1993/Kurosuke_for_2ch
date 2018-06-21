using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public ObservableCollection<Ita> Itas { get; set; }

        public Category(string Name, ObservableCollection<Ita> ItaList)
        {
            this.Name = Name;
            this.Itas = ItaList;
        }

        private Category()
        { }
    }
}
