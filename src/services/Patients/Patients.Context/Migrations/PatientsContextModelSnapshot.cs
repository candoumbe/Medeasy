﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Patients.Context;

namespace Patients.Context.Migrations
{
    [DbContext(typeof(PatientsContext))]
    partial class PatientsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Patients.Objects.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("BirthPlace");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("CreatedDate");

                    b.Property<string>("Firstname")
                        .HasMaxLength(255);

                    b.Property<string>("Lastname")
                        .HasMaxLength(255);

                    b.Property<Guid>("UUID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(255);

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.HasIndex("UUID")
                        .IsUnique();

                    b.ToTable("Patient");
                });
#pragma warning restore 612, 618
        }
    }
}
