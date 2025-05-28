using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Base;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        #region DbSet
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductCategorySize> ProductCategorySizes { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<ProductStyle> ProductStyles { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }


        #endregion

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Roles>()
                .HasMany(r => r.Users)
                .WithOne()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Roles>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Roles>().HasData(
                 new Roles { RoleId = 1, RoleName = "Admin", RoleDescription = "Nguoi quan ly he thong" },
                 new Roles { RoleId = 2, RoleName = "User", RoleDescription = "Nguoi su dung" },
                 new Roles { RoleId = 3, RoleName = "Manager", RoleDescription = "Nguoi quan ly" }
);
            // Cấu hình composite key cho ProductCategorySize
            modelBuilder.Entity<ProductCategorySize>()
                .HasKey(pcs => new { pcs.CategoryId, pcs.SizeId });

            // Cấu hình quan hệ cho Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Color)
                .WithMany()
                .HasForeignKey(p => p.ColorId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Style)
                .WithMany()
                .HasForeignKey(p => p.StyleId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Material)
                .WithMany()
                .HasForeignKey(p => p.MaterialId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.TypeId);

            // Cấu hình many-to-many giữa ProductCategory và ProductSize
            modelBuilder.Entity<ProductCategory>()
                .HasMany(pc => pc.ProductCategorySizes)
                .WithOne(pcs => pcs.Category)
                .HasForeignKey(pcs => pcs.CategoryId);

            modelBuilder.Entity<ProductSize>()
                .HasMany(ps => ps.ProductCategorySizes)
                .WithOne(pcs => pcs.Size)
                .HasForeignKey(pcs => pcs.SizeId);
        }

    }
}
