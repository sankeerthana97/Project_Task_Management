using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdenty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "67be1a08-2e75-4335-897f-d42b5724fff7", new DateTime(2025, 6, 3, 19, 45, 1, 382, DateTimeKind.Utc).AddTicks(8192) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "577cfb34-9722-4d34-998e-95450ab9f26a", new DateTime(2025, 6, 3, 19, 45, 1, 382, DateTimeKind.Utc).AddTicks(8266) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "2b05dd14-b62c-4c97-be9b-b96a9694ecae", new DateTime(2025, 6, 3, 19, 45, 1, 382, DateTimeKind.Utc).AddTicks(8281) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "e35db034-646a-4727-911e-09562b7756ab", new DateTime(2025, 6, 3, 19, 43, 27, 915, DateTimeKind.Utc).AddTicks(8858) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "2d0198bb-6515-4b55-a9a0-87d29a396b85", new DateTime(2025, 6, 3, 19, 43, 27, 915, DateTimeKind.Utc).AddTicks(8949) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "40fbb9e5-33ef-4906-8b55-149bfc4a2197", new DateTime(2025, 6, 3, 19, 43, 27, 915, DateTimeKind.Utc).AddTicks(8964) });
        }
    }
}
