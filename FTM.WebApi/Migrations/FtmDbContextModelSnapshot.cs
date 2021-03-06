﻿// <auto-generated />
using System;
using FTM.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FTM.WebApi.Migrations
{
    [DbContext(typeof(FtmDbContext))]
    partial class FtmDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("FTM.WebApi.Entities.FtmCalendarInfo", b =>
                {
                    b.Property<string>("CalendarId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CalendarName");

                    b.Property<string>("Description");

                    b.Property<bool>("IsUseable");

                    b.HasKey("CalendarId");

                    b.ToTable("FtmCalendarInfo");
                });

            modelBuilder.Entity("FTM.WebApi.Entities.FtmTokenResponse", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccessToken");

                    b.Property<long?>("ExpiresInSeconds");

                    b.Property<string>("IdToken");

                    b.Property<DateTime>("Issued");

                    b.Property<DateTime>("IssuedUtc");

                    b.Property<string>("RefreshToken");

                    b.Property<string>("Scope");

                    b.Property<string>("TokenType");

                    b.HasKey("UserId");

                    b.ToTable("FtmTokenResponses");
                });
#pragma warning restore 612, 618
        }
    }
}
