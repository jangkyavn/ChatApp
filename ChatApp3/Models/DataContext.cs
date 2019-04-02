using ChatApp3.Models;
using System.Data.Entity;

namespace ChatApp3.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("ChatAppConnectionString")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}