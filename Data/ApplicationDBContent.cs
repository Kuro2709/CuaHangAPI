using CuaHangAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for the entities
        public required DbSet<ThongTinSanPham> Products { get; set; }
        public required DbSet<ThongTinKhachHang> Customers { get; set; }
        public required DbSet<ThongTinHoaDon> Invoices { get; set; }
        public required DbSet<ThongTinChiTietHoaDon> InvoiceDetails { get; set; }
        public required DbSet<User> Users { get; set; } // Add this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map entities to their corresponding database tables
            modelBuilder.Entity<ThongTinSanPham>().ToTable("Product");
            modelBuilder.Entity<ThongTinKhachHang>().ToTable("Customer");
            modelBuilder.Entity<ThongTinHoaDon>().ToTable("Invoice");
            modelBuilder.Entity<ThongTinChiTietHoaDon>().ToTable("InvoiceDetails");
            modelBuilder.Entity<User>().ToTable("Users"); // Add this line

            // Configure relationships

            // Invoice - InvoiceDetails (One-to-Many)
            modelBuilder.Entity<ThongTinHoaDon>()
                .HasMany(i => i.InvoiceDetails)
                .WithOne(d => d.Invoice)
                .HasForeignKey(d => d.InvoiceID)
                .IsRequired();

            // InvoiceDetails - Product (One-to-One)
            modelBuilder.Entity<ThongTinChiTietHoaDon>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductID)
                .IsRequired();

            // Invoice - Customer (One-to-One)
            modelBuilder.Entity<ThongTinHoaDon>()
                .HasOne(i => i.Customer)
                .WithMany()
                .HasForeignKey(i => i.CustomerID)
                .IsRequired();
        }
    }
}
