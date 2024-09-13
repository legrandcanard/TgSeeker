﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TgSeeker.Persistent.Contexts;

#nullable disable

namespace TgSeeker.Persistent.Sqlite.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240913121916_TgsVideoNoteMessage")]
    partial class TgsVideoNoteMessage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("TgSeeker.Persistent.Entities.Option", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("TgSeeker.Persistent.Entities.TgsMessage", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("Id");

                    b.ToTable("Messages");

                    b.HasDiscriminator().HasValue("TgsMessage");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("TgSeeker.Persistent.Entities.TgsTextMessage", b =>
                {
                    b.HasBaseType("TgSeeker.Persistent.Entities.TgsMessage");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("TgsTextMessage");
                });

            modelBuilder.Entity("TgSeeker.Persistent.Entities.TgsVideoNoteMessage", b =>
                {
                    b.HasBaseType("TgSeeker.Persistent.Entities.TgsMessage");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocalFileId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("MinithumbnailData")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<int>("MinithumbnailHeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinithumbnailWidth")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ThumbnailFormat")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ThumbnailHeight")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ThumbnailLocalFileId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ThumbnailWidth")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Waveform")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasDiscriminator().HasValue("TgsVideoNoteMessage");
                });

            modelBuilder.Entity("TgSeeker.Persistent.Entities.TgsVoiceMessage", b =>
                {
                    b.HasBaseType("TgSeeker.Persistent.Entities.TgsMessage");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocalFileId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Waveform")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.ToTable("Messages", t =>
                        {
                            t.Property("Duration")
                                .HasColumnName("TgsVoiceMessage_Duration");

                            t.Property("LocalFileId")
                                .HasColumnName("TgsVoiceMessage_LocalFileId");

                            t.Property("Waveform")
                                .HasColumnName("TgsVoiceMessage_Waveform");
                        });

                    b.HasDiscriminator().HasValue("TgsVoiceMessage");
                });
#pragma warning restore 612, 618
        }
    }
}
