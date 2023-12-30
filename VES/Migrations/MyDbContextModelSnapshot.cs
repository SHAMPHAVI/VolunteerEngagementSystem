﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VES.Data;

#nullable disable

namespace VES.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("OpportunityModel", b =>
                {
                    b.Property<string>("Title")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Category")
                        .HasColumnType("longtext");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("District")
                        .HasColumnType("longtext");

                    b.Property<string>("Province")
                        .HasColumnType("longtext");

                    b.Property<string>("UserEmail")
                        .HasColumnType("longtext");

                    b.HasKey("Title");

                    b.ToTable("Opportunities");
                });

            modelBuilder.Entity("VES.Models.Alert", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("BloodGroup")
                        .HasColumnType("longtext");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("District")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Location")
                        .HasColumnType("longtext");

                    b.Property<string>("Province")
                        .HasColumnType("longtext");

                    b.Property<string>("Team")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserEmail")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("VES.Models.AlertModel", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("BloodGroup")
                        .HasColumnType("longtext");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("District")
                        .HasColumnType("longtext");

                    b.Property<string>("Province")
                        .HasColumnType("longtext");

                    b.Property<string>("Team")
                        .HasColumnType("longtext");

                    b.HasKey("Email");

                    b.ToTable("AlertInfo");
                });

            modelBuilder.Entity("VES.Models.EventRegistration", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("EventEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("EventName")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("JoinedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserEmail")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("EventRegistrations");
                });

            modelBuilder.Entity("VES.Models.Location.City", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("district_id")
                        .HasColumnType("int");

                    b.Property<string>("name_en")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("VES.Models.Location.District", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("name_en")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("province_id")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("Districts");
                });

            modelBuilder.Entity("VES.Models.Location.Province", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("name_en")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("VES.Models.RateModel", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EventEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("EventName")
                        .HasColumnType("longtext");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("UserEmail")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("EventRating");
                });

            modelBuilder.Entity("VES.Models.VolunteerRegister", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Category")
                        .HasColumnType("longtext");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Full_Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Role")
                        .HasColumnType("longtext");

                    b.HasKey("Email");

                    b.ToTable("Volunteers");
                });
#pragma warning restore 612, 618
        }
    }
}
