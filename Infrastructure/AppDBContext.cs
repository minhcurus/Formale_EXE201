using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PremiumPackage> PremiumPackages { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductCategorySize> ProductCategorySizes { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<ProductStyle> ProductStyles { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }

        public DbSet<UserCloset> UserClosets { get; set; }
        public DbSet<OutfitCombo> OutfitCombos { get; set; }
        public DbSet<OutfitComboItem> OutfitComboItems { get; set; }


        #endregion
        public override int SaveChanges()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var e in ChangeTracker.Entries<BaseEntity>())
            {
                switch (e.State)
                {
                    case EntityState.Added:
                        e.Entity.CreatedAt = utcNow;   // tạo mới
                        e.Entity.UpdatedAt = utcNow;
                        break;

                    case EntityState.Modified:
                        e.Property(p => p.CreatedAt).IsModified = false; // giữ nguyên
                        e.Entity.UpdatedAt = utcNow;    // cập nhật
                        break;
                }
            }
            return base.SaveChanges();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // lấy tất cả kiểu kế thừa BaseEntity
            var baseTypes = modelBuilder.Model.GetEntityTypes()
                              .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType));

            foreach (var t in baseTypes)
            {
                modelBuilder.Entity(t.ClrType).Property<DateTime>("CreatedAt")
                            .HasDefaultValueSql("GETUTCDATE()");   // mặc định khi INSERT

                modelBuilder.Entity(t.ClrType).Property<bool>("IsDeleted")
                            .HasDefaultValue(false);
            }

            modelBuilder.Entity<Roles>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment - User
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.UserAccount)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Roles>()
            //    .HasMany(r => r.Users)
            //    .WithOne(u => u.Role)
            //    .HasForeignKey(u => u.RoleId)
            //    .OnDelete(DeleteBehavior.Restrict);


            // Payment - Order
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Payment - PremiumPackage
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PremiumPackage)
                .WithMany(pp => pp.Payments)
                .HasForeignKey(p => p.PremiumPackageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.UserAccount)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.BuyerName).HasMaxLength(200);
                entity.Property(p => p.BuyerEmail).HasMaxLength(200);
                entity.Property(p => p.BuyerPhone).HasMaxLength(20);
                entity.Property(p => p.BuyerAddress).HasMaxLength(500);
                entity.Property(p => p.CancelUrl).HasMaxLength(1000);
                entity.Property(p => p.ReturnUrl).HasMaxLength(1000);
                entity.Property(p => p.PaymentUrl).HasMaxLength(1000);
            });


            // Cập nhật trong phần cấu hình UserAccount
            modelBuilder.Entity<UserAccount>()
                .HasMany(u => u.OutfitCombos)
                .WithOne(oc => oc.User)
                .HasForeignKey(oc => oc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAccount>()
                .HasMany(u => u.UserClosets)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Roles>().HasData(
                 new Roles { RoleId = 1, RoleName = "Admin", RoleDescription = "Nguoi quan ly he thong" },
                 new Roles { RoleId = 2, RoleName = "User", RoleDescription = "Nguoi su dung" },
                 new Roles { RoleId = 3, RoleName = "Manager", RoleDescription = "Nguoi quan ly" }
);
            // Cấu hình composite key cho ProductCategorySize
            modelBuilder.Entity<ProductCategorySize>()
                .HasKey(pcs => new { pcs.CategoryId, pcs.SizeId });

            // Cấu hình ID kiểu Guid + Identity cho bảng Product*
            void ConfigGuidEntity<TEntity>(ModelBuilder mb, string keyName) where TEntity : class
            {
                mb.Entity<TEntity>(b =>
                {
                    b.HasKey(keyName);
                    b.Property<Guid>(keyName)
                     .ValueGeneratedOnAdd()
                     .HasColumnType("uniqueidentifier");
                });
            }
            ConfigGuidEntity<Product>(modelBuilder, nameof(Product.ProductId));
            ConfigGuidEntity<ProductBrand>(modelBuilder, nameof(ProductBrand.BrandId));
            ConfigGuidEntity<ProductCategory>(modelBuilder, nameof(ProductCategory.CategoryId));
            ConfigGuidEntity<ProductColor>(modelBuilder, nameof(ProductColor.ColorId));
            ConfigGuidEntity<ProductMaterial>(modelBuilder, nameof(ProductMaterial.MaterialId));
            ConfigGuidEntity<ProductSize>(modelBuilder, nameof(ProductSize.SizeId));
            ConfigGuidEntity<ProductStyle>(modelBuilder, nameof(ProductStyle.StyleId));
            ConfigGuidEntity<ProductType>(modelBuilder, nameof(ProductType.TypeId));
            ConfigGuidEntity<UserCloset>(modelBuilder, nameof(UserCloset.ClosetId));
            ConfigGuidEntity<OutfitCombo>(modelBuilder, nameof(OutfitCombo.ComboId));
            ConfigGuidEntity<OutfitComboItem>(modelBuilder, nameof(OutfitComboItem.Id));

            modelBuilder.Entity<ProductBrand>()
        .Property(b => b.IsDeleted)
        .HasDefaultValue(false);

            modelBuilder.Entity<ProductColor>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductMaterial>()
                .Property(m => m.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductSize>()
                .Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductCategory>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductStyle>()
                .Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductType>()
                .Property(t => t.IsDeleted)
                .HasDefaultValue(false);

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



            //PremiumPackage
            modelBuilder.Entity<PremiumPackage>().HasData(
                new PremiumPackage
                {
                    Id = 1,
                    Tier = Domain.Enum.PremiumPackageTier.Bronze,
                    Name = "Bronze Package",
                    Description = "Gói cơ bản, hỗ trợ hạn chế.",
                    Price = 50000,
                    DurationInDays = 7
                },
                new PremiumPackage
                {
                    Id = 2,
                    Tier = Domain.Enum.PremiumPackageTier.Silver,
                    Name = "Silver Package",
                    Description = "Gói nâng cao với nhiều tính năng hơn.",
                    Price = 120000,
                    DurationInDays = 30
                },
                new PremiumPackage
                {
                    Id = 3,
                    Tier = Domain.Enum.PremiumPackageTier.Gold,
                    Name = "Gold Package",
                    Description = "Gói cao cấp đầy đủ tính năng.",
                    Price = 300000,
                    DurationInDays = 90
                }
            );


            // Cấu hình many-to-many giữa ProductCategory và ProductSize
            modelBuilder.Entity<ProductCategory>()
                .HasMany(pc => pc.ProductCategorySizes)
                .WithOne(pcs => pcs.Category)
                .HasForeignKey(pcs => pcs.CategoryId);

            modelBuilder.Entity<ProductSize>()
                .HasMany(ps => ps.ProductCategorySizes)
                .WithOne(pcs => pcs.Size)
                .HasForeignKey(pcs => pcs.SizeId);

            string schema = "sps13686_hiTech";


            modelBuilder.Entity<Order>().ToTable("Orders", schema);
            modelBuilder.Entity<Payment>().ToTable("Payments", schema);
            modelBuilder.Entity<PremiumPackage>().ToTable("PremiumPackages", schema);
            modelBuilder.Entity<Product>().ToTable("Products", schema);
            modelBuilder.Entity<ProductBrand>().ToTable("ProductBrands", schema);
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategories", schema);
            modelBuilder.Entity<ProductColor>().ToTable("ProductColors", schema);
            modelBuilder.Entity<ProductMaterial>().ToTable("ProductMaterials", schema);
            modelBuilder.Entity<ProductSize>().ToTable("ProductSizes", schema);
            modelBuilder.Entity<ProductStyle>().ToTable("ProductStyles", schema);
            modelBuilder.Entity<ProductType>().ToTable("ProductTypes", schema);
            modelBuilder.Entity<ProductCategorySize>().ToTable("ProductCategorySizes", schema);
            modelBuilder.Entity<UserAccount>().ToTable("Users", schema);
            modelBuilder.Entity<Roles>().ToTable("Roles", schema);
            modelBuilder.Entity<UserCloset>().ToTable("UserClosets", schema);
            modelBuilder.Entity<OutfitCombo>().ToTable("OutfitCombos", schema);
            modelBuilder.Entity<OutfitComboItem>().ToTable("OutfitComboItems", schema);
            modelBuilder.Entity<Cart>().ToTable("Carts", schema);
            modelBuilder.Entity<CartItem>().ToTable("CartItems", schema);
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems", schema);



            // Cấu hình cho UserCloset
            modelBuilder.Entity<UserCloset>(entity =>
            {
                entity.HasKey(uc => uc.ClosetId);

                entity.HasOne(uc => uc.User)
                    .WithMany(u => u.UserClosets)
                    .HasForeignKey(uc => uc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(uc => uc.Product)
                    .WithMany()
                    .HasForeignKey(uc => uc.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Optional relationship

                entity.HasOne(uc => uc.Combo)
                    .WithMany(c => c.UserClosets)
                    .HasForeignKey(uc => uc.ComboId)
                    .OnDelete(DeleteBehavior.Restrict); // Optional relationship
            });

            // Cấu hình cho OutfitCombo
            modelBuilder.Entity<OutfitCombo>(entity =>
            {
                entity.HasKey(oc => oc.ComboId);

                entity.HasOne(oc => oc.User)
                    .WithMany(u => u.OutfitCombos)
                    .HasForeignKey(oc => oc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình cho OutfitComboItem
            modelBuilder.Entity<OutfitComboItem>(entity =>
            {
                entity.HasKey(oci => oci.Id);

                entity.HasOne(oci => oci.Combo)
                    .WithMany(oc => oc.Items)
                    .HasForeignKey(oci => oci.ComboId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oci => oci.Product)
                    .WithMany()
                    .HasForeignKey(oci => oci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oci => oci.Category)
                    .WithMany()
                    .HasForeignKey(oci => oci.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // tránh xóa sp kéo theo giỏ hàng

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
