using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatApp3.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public bool Connected { get; set; }
        public string ConnectionId { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}