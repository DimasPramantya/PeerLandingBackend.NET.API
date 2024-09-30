using System;
using System.Collections.Generic;
using DAL.Enum;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public partial class P2plandingContext : DbContext
{
    public P2plandingContext()
    {
    }

    public P2plandingContext(DbContextOptions<P2plandingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MstUser> MstUsers { get; set; }
    public virtual DbSet<MstLoans> MstLoans { get; set; }
    public virtual DbSet<CashFlow> CashFlows { get; set; }
    public virtual DbSet<TrnRepayment> TrnRepayments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MstUser>(static entity =>
        {
            entity.HasKey(e => e.Id).HasName("mst_user_pkey");

            entity.ToTable("mst_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .HasColumnName("password");

            entity.HasMany(u => u.Loans)
            .WithOne(l => l.Borrower)
            .HasForeignKey(l => l.BorrowerId)
            .OnDelete(DeleteBehavior.Cascade); 

            entity.HasMany(u => u.Fundings)
                .WithOne(l => l.Lender)
                .HasForeignKey(l => l.LenderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
