using FCMS.Model.Entities;
using FCMS.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace FCMS.Persistence
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = "ee4c458",
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john@gmail.com",
                        Password = "JohnDoe",
                        PhoneNumber = "08155850462",
                        Gender = (Gender)1,
                        Role = (Role)1,

                    }
                );
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId);


        }
    }


}
