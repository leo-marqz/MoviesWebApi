using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesWebApi.Migrations
{
    public partial class AdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4359b4b4-2b30-48cc-8e77-6ceabaa1794a", "fadbb0fd-1fa2-48d7-84bc-eec1e00cd29c", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e8c76789-28be-4e42-baab-26a4e53cf1da", 0, "54858df5-3f27-4bb8-b53e-6db11e891be9", "leomarqz@gmail.com", false, false, null, "leomarqz@gmail.com", "leomarqz@gmail.com", "AQAAAAEAACcQAAAAEKL682AByAUaXVTH0/vcnNneo224UBziToJeaOM3qa3r8pTatRV0aE4pFOGGuabv4w==", null, false, "43d5a0bd-f489-46f6-bb49-976ebbe12985", false, "leomarqz@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "e8c76789-28be-4e42-baab-26a4e53cf1da" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4359b4b4-2b30-48cc-8e77-6ceabaa1794a");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e8c76789-28be-4e42-baab-26a4e53cf1da");
        }
    }
}
