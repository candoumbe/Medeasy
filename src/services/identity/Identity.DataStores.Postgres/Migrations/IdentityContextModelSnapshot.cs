﻿// <auto-generated />
using System;
using Identity.DataStores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Identity.DataStores.Postgres.Migrations
{
    [DbContext(typeof(IdentityDataStore))]
    partial class IdentityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Identity.Objects.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<Instant?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("Locked")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uuid");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<Instant?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Identity.Objects.AccountClaim", b =>
                {
                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Instant?>("End")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Instant>("Start")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AccountId", "Id");

                    b.ToTable("AccountClaim");
                });

            modelBuilder.Entity("Identity.Objects.AccountRole", b =>
                {
                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("AccountId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AccountRole");
                });

            modelBuilder.Entity("Identity.Objects.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<Instant?>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text");

                    b.Property<Instant?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Identity.Objects.RoleClaim", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.HasKey("RoleId", "Id");

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Identity.Objects.AccountClaim", b =>
                {
                    b.HasOne("Identity.Objects.Account", null)
                        .WithMany("Claims")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Identity.Objects.Claim", "Claim", b1 =>
                        {
                            b1.Property<Guid>("AccountClaimAccountId")
                                .HasColumnType("uuid");

                            b1.Property<Guid>("AccountClaimId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.HasKey("AccountClaimAccountId", "AccountClaimId");

                            b1.ToTable("AccountClaim");

                            b1.WithOwner()
                                .HasForeignKey("AccountClaimAccountId", "AccountClaimId");
                        });

                    b.Navigation("Claim");
                });

            modelBuilder.Entity("Identity.Objects.AccountRole", b =>
                {
                    b.HasOne("Identity.Objects.Account", "Account")
                        .WithMany("Roles")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Identity.Objects.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Identity.Objects.RoleClaim", b =>
                {
                    b.HasOne("Identity.Objects.Role", null)
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Identity.Objects.Claim", "Claim", b1 =>
                        {
                            b1.Property<Guid>("RoleClaimRoleId")
                                .HasColumnType("uuid");

                            b1.Property<Guid>("RoleClaimId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.HasKey("RoleClaimRoleId", "RoleClaimId");

                            b1.ToTable("RoleClaim");

                            b1.WithOwner()
                                .HasForeignKey("RoleClaimRoleId", "RoleClaimId");
                        });

                    b.Navigation("Claim");
                });

            modelBuilder.Entity("Identity.Objects.Account", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Identity.Objects.Role", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Claims");
                });
#pragma warning restore 612, 618
        }
    }
}
