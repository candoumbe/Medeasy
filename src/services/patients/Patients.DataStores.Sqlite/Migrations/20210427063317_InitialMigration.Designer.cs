﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Patients.Context;

namespace Patients.DataStores.Sqlite.Migrations
{
    [DbContext(typeof(PatientsContext))]
    [Migration("20210427063317_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("Patients.Objects.Patient", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("BirthDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("BirthPlace")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Firstname")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Lastname")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Patient");
                });
#pragma warning restore 612, 618
        }
    }
}
