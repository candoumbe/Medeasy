﻿// <auto-generated />
using Identity.DataStores.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Identity.DataStores.SqlServer.Migrations
{
    [DbContext(typeof(IdentityContext))]
    partial class IdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Identity.Objects.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("Locked");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<int?>("RoleId");

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<Guid?>("TenantId");

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Identity.Objects.AccountClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountId");

                    b.Property<int>("ClaimId");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<DateTimeOffset?>("End");

                    b.Property<DateTimeOffset>("Start");

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ClaimId");

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.ToTable("AccountClaim");
                });

            modelBuilder.Entity("Identity.Objects.Claim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.ToTable("Claim");
                });

            modelBuilder.Entity("Identity.Objects.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AccountId");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Identity.Objects.RoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClaimId");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<int>("RoleId");

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.HasIndex("ClaimId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Identity.Objects.Account", b =>
                {
                    b.HasOne("Identity.Objects.Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Identity.Objects.AccountClaim", b =>
                {
                    b.HasOne("Identity.Objects.Account", "Account")
                        .WithMany("Claims")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.Objects.Claim", "Claim")
                        .WithMany("Users")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Identity.Objects.Role", b =>
                {
                    b.HasOne("Identity.Objects.Account")
                        .WithMany("Roles")
                        .HasForeignKey("AccountId");
                });

            modelBuilder.Entity("Identity.Objects.RoleClaim", b =>
                {
                    b.HasOne("Identity.Objects.Claim", "Claim")
                        .WithMany("Roles")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Identity.Objects.Role", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
