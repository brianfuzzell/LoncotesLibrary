using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LoncotesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedCheckouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 3, null },
                    { 3, new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 3, null },
                    { 4, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, null },
                    { 5, new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, null },
                    { 6, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, null },
                    { 7, new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
