using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Career_Path.Migrations
{
    /// <inheritdoc />
    public partial class seedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0191a4b6-c4fc-752e-9d95-40b5e4e68054", "0191a4b6-c4fc-752e-9d95-40b631d1866d", false, false, "Admin", "ADMIN" },
                    { "0191a4b6-c4fc-752e-9d95-40b7a5cb88f0", "0191a4b6-c4fc-752e-9d95-40b85cf3fd22", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6", 0, "0191a4b6-c4fc-752e-9d95-40b42a925b8e", "sayed732004444@gmail.com", true, "Career Path", false, "Admin", false, null, "SAYED732004444@GMAIL.COM", "SAYED732004444@GMAIL.COM", "AQAAAAIAAYagAAAAEKRku5u6K325Irl1Utujiuil/WUhjTvShS9mJLXxO+2v/GKrMT1Ofhdp/0taFUO2bA==", null, false, "55BF92C9EF0249CDA210D85D1A851BC9", false, "sayed732004444@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "0191a4b6-c4fc-752e-9d95-40b5e4e68054", "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0191a4b6-c4fc-752e-9d95-40b7a5cb88f0");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "0191a4b6-c4fc-752e-9d95-40b5e4e68054", "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0191a4b6-c4fc-752e-9d95-40b5e4e68054");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6");
        }
    }
}
