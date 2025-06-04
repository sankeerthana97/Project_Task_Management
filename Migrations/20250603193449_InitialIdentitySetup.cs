using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTaskManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "7da7bd79-6448-49cd-936e-8842d5c94501", new DateTime(2025, 6, 3, 19, 17, 42, 199, DateTimeKind.Utc).AddTicks(2154) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "b5eda1ea-2085-4347-9e0f-9ba9883dca82", new DateTime(2025, 6, 3, 19, 17, 42, 199, DateTimeKind.Utc).AddTicks(2231) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "1049c57c-ab90-4b93-b165-40d9d0590d0a", new DateTime(2025, 6, 3, 19, 17, 42, 199, DateTimeKind.Utc).AddTicks(2240) });
        }
    }
}
