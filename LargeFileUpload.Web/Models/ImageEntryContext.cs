using System.Data.Entity;

namespace PrDCOldApp.Web.Models
{
    public class ImageEntryContext : DbContext
    {
        public DbSet<ImageEntry> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}