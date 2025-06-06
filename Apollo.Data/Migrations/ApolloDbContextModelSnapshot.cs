﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Apollo.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Apollo.Data.Migrations
{
    [DbContext(typeof(ApolloDbContext))]
    partial class ApolloDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Apollo.Data.Models.Research", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Research");
                });

            modelBuilder.Entity("Apollo.Data.Models.ResearchPlan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.PrimitiveCollection<List<string>>("Questions")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<Guid>("ResearchId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ResearchId")
                        .IsUnique();

                    b.ToTable("ResearchPlans");
                });

            modelBuilder.Entity("Apollo.Data.Models.ResearchReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ResearchId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ResearchId")
                        .IsUnique();

                    b.ToTable("ResearchReports");
                });

            modelBuilder.Entity("Apollo.Data.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AuthProvider")
                        .HasColumnType("integer");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Apollo.Data.Models.Research", b =>
                {
                    b.HasOne("Apollo.Data.Models.User", "User")
                        .WithMany("ResearchLibrary")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Apollo.Data.Models.ResearchPlan", b =>
                {
                    b.HasOne("Apollo.Data.Models.Research", "Research")
                        .WithOne("Plan")
                        .HasForeignKey("Apollo.Data.Models.ResearchPlan", "ResearchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Research");
                });

            modelBuilder.Entity("Apollo.Data.Models.ResearchReport", b =>
                {
                    b.HasOne("Apollo.Data.Models.Research", "Research")
                        .WithOne("Report")
                        .HasForeignKey("Apollo.Data.Models.ResearchReport", "ResearchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Research");
                });

            modelBuilder.Entity("Apollo.Data.Models.Research", b =>
                {
                    b.Navigation("Plan")
                        .IsRequired();

                    b.Navigation("Report");
                });

            modelBuilder.Entity("Apollo.Data.Models.User", b =>
                {
                    b.Navigation("ResearchLibrary");
                });
#pragma warning restore 612, 618
        }
    }
}
