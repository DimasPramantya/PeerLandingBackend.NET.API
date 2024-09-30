﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(P2plandingContext))]
    [Migration("20240928120539_migrationv2.3")]
    partial class migrationv23
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DAL.Models.Funding", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("FundedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LenderId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LoanId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LenderId");

                    b.HasIndex("LoanId")
                        .IsUnique();

                    b.ToTable("trn_funding");
                });

            modelBuilder.Entity("DAL.Models.MstLoans", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<string>("BorrowerId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("borrower_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("DurationMonth")
                        .HasColumnType("integer")
                        .HasColumnName("duration_month");

                    b.Property<decimal>("InterestRate")
                        .HasColumnType("numeric")
                        .HasColumnName("interest_rate");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("BorrowerId");

                    b.ToTable("mst_loans");
                });

            modelBuilder.Entity("DAL.Models.TrnRepayment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LoanId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("LoanId");

                    b.ToTable("trn_repayment");
                });

            modelBuilder.Entity("MstUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric")
                        .HasColumnName("balance");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("password");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("mst_user_pkey");

                    b.ToTable("mst_user", (string)null);
                });

            modelBuilder.Entity("DAL.Models.Funding", b =>
                {
                    b.HasOne("MstUser", "Lender")
                        .WithMany("Fundings")
                        .HasForeignKey("LenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Models.MstLoans", "Loan")
                        .WithOne("Fundings")
                        .HasForeignKey("DAL.Models.Funding", "LoanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lender");

                    b.Navigation("Loan");
                });

            modelBuilder.Entity("DAL.Models.MstLoans", b =>
                {
                    b.HasOne("MstUser", "User")
                        .WithMany("Loans")
                        .HasForeignKey("BorrowerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Models.TrnRepayment", b =>
                {
                    b.HasOne("DAL.Models.MstLoans", "Loan")
                        .WithMany("Repayments")
                        .HasForeignKey("LoanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Loan");
                });

            modelBuilder.Entity("DAL.Models.MstLoans", b =>
                {
                    b.Navigation("Fundings")
                        .IsRequired();

                    b.Navigation("Repayments");
                });

            modelBuilder.Entity("MstUser", b =>
                {
                    b.Navigation("Fundings");

                    b.Navigation("Loans");
                });
#pragma warning restore 612, 618
        }
    }
}
