using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure;

namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Migrations
{
    [DbContext(typeof(LocationsContext))]
    [Migration("20170601140634_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:Sequence:.frontier_seq", "'frontier_seq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:Sequence:.locations_seq", "'locations_seq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:Sequence:.UserLocation_seq", "'UserLocation_seq', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.FrontierPoints", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "frontier_seq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<double>("Latitude");

                    b.Property<int?>("LocationId")
                        .IsRequired();

                    b.Property<double>("Longitude");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("FrontierPoints");
                });

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "locations_seq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnName("LocationCode")
                        .HasMaxLength(15);

                    b.Property<string>("Description")
                        .HasMaxLength(100);

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int?>("ParentId");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.UserLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "UserLocation_seq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<int?>("LocationId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserLocation");
                });

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.FrontierPoints", b =>
                {
                    b.HasOne("Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations", "Location")
                        .WithMany("Polygon")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations", b =>
                {
                    b.HasOne("Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations")
                        .WithMany("ChildLocations")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Locations.API.Model.UserLocation", b =>
                {
                    b.HasOne("Microsoft.eShopOnContainers.Services.Locations.API.Model.Locations", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });
        }
    }
}
