﻿// <auto-generated />
using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241015184848_UserScoreConparer")]
    partial class UserScoreConparer
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("API.DTOs.GameDTO", b =>
                {
                    b.Property<int>("GameID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GameID"));

                    b.Property<string>("AltText")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Route")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("GameID");

                    b.ToTable("Games", (string)null);

                    b.HasData(
                        new
                        {
                            GameID = 1,
                            AltText = "Long number memory icon",
                            Description = "Memorize as many digits of a long number as possible.",
                            Icon = "src/assets/longNumberIcon.svg",
                            Route = "/longNumber",
                            Title = "Long number memory"
                        },
                        new
                        {
                            GameID = 2,
                            AltText = "Chimp test icon",
                            Description = "Test your memory by remembering the order of numbers displayed on the screen.",
                            Icon = "src/assets/chimpIcon.svg",
                            Route = "/chimpTest",
                            Title = "Chimp Test"
                        },
                        new
                        {
                            GameID = 3,
                            AltText = "Sequence memory icon",
                            Description = "Remember and recall increasingly larger sequence of action showed.",
                            Icon = "src/assets/sequenceIcon.svg",
                            Route = "/sequence",
                            Title = "Sequence Memorization"
                        });
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("UserId"));

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ProfileImagePath")
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("API.Models.UserScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("GameDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GameType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserScores");
                });

            modelBuilder.Entity("API.Models.UserScore", b =>
                {
                    b.HasOne("API.Models.User", null)
                        .WithMany("Scores")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Navigation("Scores");
                });
#pragma warning restore 612, 618
        }
    }
}