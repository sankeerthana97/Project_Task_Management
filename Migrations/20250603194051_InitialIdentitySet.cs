using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "41c3ab9f-d4a6-4f10-b22a-8a510495235c", new DateTime(2025, 6, 3, 19, 34, 49, 65, DateTimeKind.Utc).AddTicks(3174) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "c1306c5d-206d-4d35-89ec-93e6f8f4a836", new DateTime(2025, 6, 3, 19, 34, 49, 65, DateTimeKind.Utc).AddTicks(3252) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "46444ab9-6ed0-43f4-9a3f-af5a9db9f294", new DateTime(2025, 6, 3, 19, 34, 49, 65, DateTimeKind.Utc).AddTicks(3266) });
        }
    }
}
