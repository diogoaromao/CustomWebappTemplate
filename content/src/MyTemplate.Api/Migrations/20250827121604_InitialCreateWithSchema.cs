using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyTemplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MyTemplate");

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "MyTemplate",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "MyTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "MyTemplate",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_UserId",
                        column: x => x.UserId,
                        principalSchema: "MyTemplate",
                        principalTable: "Carts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "MyTemplate",
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 8, 22, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2872), "A sample product", "Sample Product 1", 19.99m, new DateTime(2025, 8, 22, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2879) },
                    { 2, new DateTime(2025, 8, 24, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2881), "Another sample product", "Sample Product 2", 29.99m, new DateTime(2025, 8, 24, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2882) },
                    { 3, new DateTime(2025, 8, 26, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2883), "Yet another sample product", "Sample Product 3", 39.99m, new DateTime(2025, 8, 26, 12, 16, 3, 726, DateTimeKind.Utc).AddTicks(2884) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "MyTemplate");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "MyTemplate");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "MyTemplate");
        }
    }
}
