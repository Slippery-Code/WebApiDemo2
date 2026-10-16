using Microsoft.EntityFrameworkCore;
using WebApiDemo2.Models;
namespace WebApiDemo2.Data
{
    public class AplicationDbContext : DbContext
    {
        //prepei na kserei to dbContext pou uparxei h basi mas auto to kanoume me ton constructor
        public AplicationDbContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<Shirt> Shirts { get; set; }

        //xrisimopoioume generic gia anaparastisoume ti tipou db set kai tha dosoume thn klasi shirt
        //leme oti to shirt table 
        //otan dinoume stin klasi shirt me to db set prosdiorizoume enan pinaka bash 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //auth h methodos mas leei oti to in-memory database dimiourgeite mporoume na tropopoihsoume thn basi

            //framework core gnwrizei oti to ShirtId prepei na einai to primary key
            //data seeding

            modelBuilder.Entity<Shirt>().HasData(
            new Shirt { ShirtId = 1, Brand = "My Brand", Color = "Blue", Gender = "Men", Price = 30, Size = 10 },
            new Shirt { ShirtId = 2, Brand = "My Brand", Color = "Blue", Gender = "Men", Price = 35, Size = 12 },
            new Shirt { ShirtId = 3, Brand = "My Brand", Color = "Blue", Gender = "Women", Price = 28, Size = 8 },
            new Shirt { ShirtId = 4, Brand = "My Brand", Color = "Blue", Gender = "Women", Price = 30, Size = 9 }
            );
            //otan sto melon dimiourghsoume enan kainourio pinaka mporoume na prosdiorisoume kai to relationship metaksi ton pinakwn
        }
    }
}
