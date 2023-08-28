// <auto-generated />
using System;
using DataModelAndMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataModelAndMigration.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230828081958_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataModelAndMigration.MyModel", b =>
                {
                    b.Property<Guid>("GuidPartOfKey")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DatePartOfKey")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SomeValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("GuidPartOfKey", "DatePartOfKey");

                    b.ToTable("MyModels");
                });
#pragma warning restore 612, 618
        }
    }
}