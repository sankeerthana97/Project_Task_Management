using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "a25d4e4e-1bf0-4e28-8069-5bbc3d901497", new DateTime(2025, 6, 3, 19, 40, 51, 605, DateTimeKind.Utc).AddTicks(7990) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "cf8d932b-b1e2-4d62-a5cd-0080698aebe5", new DateTime(2025, 6, 3, 19, 40, 51, 605, DateTimeKind.Utc).AddTicks(8069) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "5956d1ff-bcc3-4da1-b6c3-0f63d10a9a26", new DateTime(2025, 6, 3, 19, 40, 51, 605, DateTimeKind.Utc).AddTicks(8083) });
        }
    }
}
