using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.eShopOnContainers.Services.Ordering.API.UnitOfWork;

namespace Ordering.API.Migrations
{
    [DbContext(typeof(OrderingContext))]
    [Migration("20160909202620_MyFirstMigration")]
    partial class MyFirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BuyerId");

                    b.Property<DateTime>("OrderDate");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });
        }
    }
}
