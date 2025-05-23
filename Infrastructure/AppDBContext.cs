using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        #endregion

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
        }
    }   
}
