using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TicketProjectWEB.Models;

public partial class CoreProject5DbContext : DbContext
{
    public CoreProject5DbContext()
    {
    }

    public CoreProject5DbContext(DbContextOptions<CoreProject5DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DepartmentTbl> DepartmentTbls { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ForgotPassword> ForgotPasswords { get; set; }

    public virtual DbSet<LoginTbl> LoginTbls { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Register> Registers { get; set; }

    public virtual DbSet<ResetPassword> ResetPasswords { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

    public virtual DbSet<Tickett> Ticketts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=NTZ-CPU-096\\MSSQLSERVER3; Database=CoreProject5DB; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DepartmentTbl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC074DEBF155");

            entity.ToTable("DepartmentTBL");

            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.File)
                .IsUnicode(false)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Salary)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ForgotPassword>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable("ForgotPassword");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LoginTbl>(entity =>
        {
            entity.ToTable("LoginTbl");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Salt).IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3213E83F3CF535BB");

            entity.ToTable("Product");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Brand)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("brand");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("category");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discountPercentage");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Rating)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("('Pending')");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("thumbnail");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Register>(entity =>
        {
            entity.ToTable("Register");

            entity.Property(e => e.ConfirmPassword).IsUnicode(false);
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.PasswordResetToken).IsUnicode(false);
            entity.Property(e => e.Salt).IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TokenExpiration).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ResetPassword>(entity =>
        {
            entity.ToTable("ResetPassword");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ConfirmPassword).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.PasswordResetToken).IsUnicode(false);
            entity.Property(e => e.Salt).IsUnicode(false);
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryId).HasName("PK__SubCateg__26BE5B194164E926");

            entity.ToTable("SubCategory");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__SubCatego__Depar__4CA06362");
        });

        modelBuilder.Entity<Tickett>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickett__712CC607FCD958AF");

            entity.ToTable("Tickett");

            entity.Property(e => e.AssignedTo).IsUnicode(false);
            entity.Property(e => e.Attachment).IsUnicode(false);
            entity.Property(e => e.Category).IsUnicode(false);
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Status)
                .IsUnicode(false)
                .HasDefaultValueSql("('Pending')");
            entity.Property(e => e.SubCategory).IsUnicode(false);
            entity.Property(e => e.Title).IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Ticketts)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Tickett__Created__5AEE82B9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
