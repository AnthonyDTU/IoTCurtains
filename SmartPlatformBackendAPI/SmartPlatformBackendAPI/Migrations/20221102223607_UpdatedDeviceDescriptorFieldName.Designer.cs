﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartPlatformBackendAPI.Data;

#nullable disable

namespace SmartPlatformBackendAPI.Migrations
{
    [DbContext(typeof(SmartPlatformAPIDbContext))]
    [Migration("20221102223607_UpdatedDeviceDescriptorFieldName")]
    partial class UpdatedDeviceDescriptorFieldName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc.2.22472.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SmartPlatformBackendAPI.Models.DeviceDecriptor", b =>
                {
                    b.Property<Guid>("DeviceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DeviceKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeviceModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeviceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("backendUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DeviceID");

                    b.HasIndex("UserID");

                    b.ToTable("DeviceDecriptors");
                });

            modelBuilder.Entity("SmartPlatformBackendAPI.Models.User", b =>
                {
                    b.Property<Guid>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SmartPlatformBackendAPI.Models.DeviceDecriptor", b =>
                {
                    b.HasOne("SmartPlatformBackendAPI.Models.User", null)
                        .WithMany("DeviceDescriptors")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartPlatformBackendAPI.Models.User", b =>
                {
                    b.Navigation("DeviceDescriptors");
                });
#pragma warning restore 612, 618
        }
    }
}
