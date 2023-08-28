using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataModelAndMigration.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyModels",
                columns: table => new
                {
                    GuidPartOfKey = table.Column<Guid>(type: "uuid", nullable: false),
                    DatePartOfKey = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SomeValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyModels", x => new { x.GuidPartOfKey, x.DatePartOfKey });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyModels");
        }
    }
}
