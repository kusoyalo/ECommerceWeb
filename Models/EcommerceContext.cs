using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWeb.Models;

public partial class ecommerceContext : DbContext
{
    public ecommerceContext(DbContextOptions<ecommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.ImagePath).HasMaxLength(200);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(50);
            entity.Property(e => e.LastModifiedTime).HasColumnType("datetime");
            entity.Property(e => e.ProductCategory).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Account);

            entity.Property(e => e.Account)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(50);
            entity.Property(e => e.LastModifiedTime).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
