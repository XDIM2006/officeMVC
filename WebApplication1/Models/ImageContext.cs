using System.Data.Entity;

namespace WebApplication1.Models
{
    public class ImageContext: DbContext
    {
        public ImageContext() 
                : base("name=ImageContext")
        {
        }
        public DbSet<Image> Images { get; set; }
    }
}