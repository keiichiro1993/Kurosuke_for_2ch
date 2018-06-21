using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurosuke_for_2ch.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public string MailTo { get; set; }
        public int Id { get; set; }
        public int DataId { get; set; }
        public string DataUserId { get; set; }
        public string DataDate { get; set; }
        public string Number { get; set; }
        public string Date { get; set; }

        public int ThreadId { get; set; }
        public Thread Thread { get; set; }

        public Post(int Id, int DataId, string DataUserId, string DataDate, string Number, string Name, string MailTo, string Date, string Message, Thread ParentThread)
        {
            this.Id = Id;
            this.DataId = DataId;
            this.DataUserId = DataUserId;
            this.DataDate = DataDate;
            this.Number = Number;
            this.Name = Name;
            this.MailTo = MailTo;
            this.Date = Date;
            this.Message = Message;
            this.Thread = ParentThread;
        }

        private Post() { }
    }
}
