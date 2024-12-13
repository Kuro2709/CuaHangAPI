using CuaHangAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CuaHangAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public required DbSet<ThongTinSanPham> Products { get; set; }
        public required DbSet<ThongTinKhachHang> Customers { get; set; }
        public required DbSet<ThongTinHoaDon> Invoices { get; set; }
        public required DbSet<ThongTinChiTietHoaDon> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThongTinSanPham>().ToTable("Product");
            modelBuilder.Entity<ThongTinKhachHang>().ToTable("Customer");
            modelBuilder.Entity<ThongTinHoaDon>().ToTable("Invoice");
            modelBuilder.Entity<ThongTinChiTietHoaDon>().ToTable("InvoiceDetails");
        }
    }
}
