﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SpecFlow.Demo.Api;

#nullable disable

namespace SpecFlow.Demo.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221005144747_AddGroupTables")]
    partial class AddGroupTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.Backpack", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Backpacks");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AdminId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.GroupMember", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupsMembers");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.Backpack", b =>
                {
                    b.HasOne("SpecFlow.Demo.Api.Entities.User", "Owner")
                        .WithMany("Backpacks")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.Group", b =>
                {
                    b.HasOne("SpecFlow.Demo.Api.Entities.User", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.GroupMember", b =>
                {
                    b.HasOne("SpecFlow.Demo.Api.Entities.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SpecFlow.Demo.Api.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.Group", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("SpecFlow.Demo.Api.Entities.User", b =>
                {
                    b.Navigation("Backpacks");
                });
#pragma warning restore 612, 618
        }
    }
}
