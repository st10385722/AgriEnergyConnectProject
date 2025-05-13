using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Models;

public partial class St10385722AgriEnergyConnectDbContext : DbContext
{
    public St10385722AgriEnergyConnectDbContext()
    {
    }

    public St10385722AgriEnergyConnectDbContext(DbContextOptions<St10385722AgriEnergyConnectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=IBM-Laptop;Database=st10385722-agri-energy-connect-db;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Farmer>(entity =>
        {
            entity.HasKey(e => e.FarmerId).HasName("PK__FARMER__C61558255A8D15B7");

            entity.ToTable("FARMER");

            entity.Property(e => e.FarmerId)
                .ValueGeneratedNever()
                .HasColumnName("farmer_id");
            entity.Property(e => e.CropType)
                .IsUnicode(false)
                .HasColumnName("crop_type");
            entity.Property(e => e.FarmName)
                .IsUnicode(false)
                .HasColumnName("farm_name");
            entity.Property(e => e.FarmType)
                .IsUnicode(false)
                .HasColumnName("farm_type");
            entity.Property(e => e.HavestingDate)
                .HasColumnType("datetime")
                .HasColumnName("havesting_date");
            entity.Property(e => e.LivestockType)
                .IsUnicode(false)
                .HasColumnName("livestock_type");
            entity.Property(e => e.NumberOfEmployees).HasColumnName("number_of_employees");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Farmers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__FARMER__user_id__4CA06362");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__PRODUCT__47027DF5A2EE7320");

            entity.ToTable("PRODUCT");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FarmerId).HasColumnName("farmer_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductDescription)
                .IsUnicode(false)
                .HasColumnName("product_description");
            entity.Property(e => e.ProductImageId).HasColumnName("product_image_id");
            entity.Property(e => e.ProductName)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductType)
                .IsUnicode(false)
                .HasColumnName("product_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Farmer).WithMany(p => p.Products)
                .HasForeignKey(d => d.FarmerId)
                .HasConstraintName("FK__PRODUCT__farmer___52593CB8");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__PRODUCT___DC9AC95558D620A2");

            entity.ToTable("PRODUCT_IMAGE");

            entity.Property(e => e.ImageId)
                .ValueGeneratedNever()
                .HasColumnName("image_id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .HasColumnName("content_type");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.ImageData).HasColumnName("image_data");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PRODUCT_I__produ__5812160E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__USERS__B9BE370FEE27D9FA");

            entity.ToTable("USERS");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__USERS__role_id__3D5E1FD2");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__USER_ROL__760965CC2104AB8B");

            entity.ToTable("USER_ROLE");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
