using CuaHangAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // Define DbSet properties for the entities
        public required DbSet<ThongTinSanPham> Products { get; set; }
        public required DbSet<ThongTinKhachHang> Customers { get; set; }
        public required DbSet<ThongTinHoaDon> Invoices { get; set; }
        public required DbSet<ThongTinChiTietHoaDon> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map entities to their corresponding database tables
            modelBuilder.Entity<ThongTinSanPham>().ToTable("Product");
            modelBuilder.Entity<ThongTinKhachHang>().ToTable("Customer");
            modelBuilder.Entity<ThongTinHoaDon>().ToTable("Invoice");
            modelBuilder.Entity<ThongTinChiTietHoaDon>().ToTable("InvoiceDetails");

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
