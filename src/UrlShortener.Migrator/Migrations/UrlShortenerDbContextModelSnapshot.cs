﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrlShortener.Persistence;

#nullable disable

namespace UrlShortener.Migrator.Migrations
{
    [DbContext(typeof(UrlShortenerDbContext))]
    partial class UrlShortenerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0-preview.4.22229.2");

            modelBuilder.Entity("UrlShortener.Domain.ShortUrl", b =>
                {
                    b.Property<string>("ShortName")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreationDateTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DestinationUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("LastUpdateDateTime")
                        .HasColumnType("INTEGER");

                    b.HasKey("ShortName");

                    b.ToTable("ShortUrls");
                });

            modelBuilder.Entity("UrlShortener.Domain.ShortUrlClick", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreationDateTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortUrlId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAgent")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ShortUrlId");

                    b.ToTable("ShortUrlClicks");
                });

            modelBuilder.Entity("UrlShortener.Domain.ShortUrlClick", b =>
                {
                    b.HasOne("UrlShortener.Domain.ShortUrl", "ShortUrl")
                        .WithMany("ShortUrlClicks")
                        .HasForeignKey("ShortUrlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShortUrl");
                });

            modelBuilder.Entity("UrlShortener.Domain.ShortUrl", b =>
                {
                    b.Navigation("ShortUrlClicks");
                });
#pragma warning restore 612, 618
        }
    }
}
