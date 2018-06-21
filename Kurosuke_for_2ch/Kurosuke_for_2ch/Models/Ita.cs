using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.Models
{
    public class Ita
    {
        public int ItaId { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ObservableCollection<Thread> Threads { get; set; }

        public Ita(string Name, string Url)
        {
            this.Name = Name;
            this.Url = Url;
        }

        private Ita()
        { }


        public void UpdateInformation(Ita ita)
        {
            this.Url = ita.Url;
        }

        public Uri GetThreadListUri()
        {
            return new Uri(this.Url + "subback.html");
        }
    }
}
