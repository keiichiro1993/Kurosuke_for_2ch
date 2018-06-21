using Kurosuke_for_2ch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.ViewModels
{
    public class CreatePostPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        private string _message;
        public string message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("message");
            }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        private string _mail;
        public string mail
        {
            get { return _mail; }
            set
            {
                _mail = value;
                OnPropertyChanged("mail");
            }
        }

        public Thread Thread { get; set; }

        public CreatePostPageViewModel(Thread thread)
        {
            this.Thread = thread;
            _message = "";
            _name = "";
            _mail = "sage";
        }
    }
}
