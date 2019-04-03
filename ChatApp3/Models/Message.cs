using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApp3.Models
{
    public class Message
    {
        [Key]
        public Guid MessageID { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string ContentType { get; set; }
    }
}