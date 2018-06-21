using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.Models
{
    public class Thread : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public int ThreadId { get; set; }

        public string Url { get; set; }
        public string Name { get; set; }

        public int ItaId { get; set; }
        public Ita Ita { get; set; }
        public int ItaNumber { get; set; }

        private int _MidokuCount;
        public int MidokuCount
        {
            get { return _MidokuCount; }
            set
            {
                _MidokuCount = value;
                OnPropertyChanged("MidokuCount");
            }
        }
        public float CurrentOffset { get; set; }

        public int Count { get; set; }

        private ObservableCollection<Post> _Posts;
        public ObservableCollection<Post> Posts
        {
            get { return _Posts; }
            set
            {
                _Posts = value;
                OnPropertyChanged("Posts");
            }
        }

        public Thread(string Name, string Url, int ItaNumber, Ita ParentIta, int Count)
        {
            this.Url = Url;
            this.Name = Name;
            this.Ita = ParentIta;
            this.ItaNumber = ItaNumber;
            this.Count = Count;
        }

        public void UpdateInformation(Thread thread)
        {
            this.Url = thread.Url;
            this.Name = thread.Name;
            this.MidokuCount = thread.MidokuCount;
            //this.CurrentOffset = thread.CurrentOffset;
            this.Count = thread.Count;
            //this.Ita = thread.Ita;
            //this.ItaId = thread.ItaId;
        }

        public void UpdateOffset(Thread thread)
        {
            this.CurrentOffset = thread.CurrentOffset;
        }

        private Thread()
        { }

        public Uri GetFullThreadUri()
        {
            return new Uri(Ita.Url.Replace("2ch.net/", "2ch.net/test/read.cgi/") + Url);
        }
    }
}
