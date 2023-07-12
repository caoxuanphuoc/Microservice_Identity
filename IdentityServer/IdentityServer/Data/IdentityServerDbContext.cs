using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityServer.Data
{
    public class IdentitySeverDbContext : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long>
    {
        public IdentitySeverDbContext(DbContextOptions<IdentitySeverDbContext> options)
     : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser<long>>(b =>
            {
                b.ToTable("abpUsers");
                b.Property(u => u.EmailConfirmed).HasColumnName("IsEmailConfirmed");
                b.Property(u => u.LockoutEnabled).HasColumnName("IsLockoutEnabled");
                /*b.Property(u => u.LockoutEnd).HasColumnName("CreationTime").HasConversion(
                    v => v,
                    v => new DateTimeOffset(v));*/
                b.Ignore(u => u.LockoutEnd);
                b.Property(u => u.Email).HasColumnName("EmailAddress");
                b.Property(u => u.NormalizedEmail).HasColumnName("NormalizedEmailAddress");
                b.Property(u => u.PasswordHash).HasColumnName("Password");
                b.Property(u => u.PhoneNumberConfirmed).HasColumnName("IsPhoneNumberConfirmed");
                b.Property(u => u.TwoFactorEnabled).HasColumnName("IsTwoFactorEnabled");
            });

            modelBuilder.Entity<IdentityUserLogin<long>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("AbpUserClaims");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("AbpUserRoles");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("AbpRoles")
                 .Property(u => u.Id)
                .HasConversion<int>(); 
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("AbpRoleClaims");


        }
    }
}
