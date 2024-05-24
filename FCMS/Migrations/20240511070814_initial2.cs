using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCMS.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "ee4c458",
                columns: new[] { "DateCreated", "ProfilePicture" },
                values: new object[] { "5/11/2024", "my_passport.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "ee4c458",
                columns: new[] { "DateCreated", "ProfilePicture" },
                values: new object[] { "5/7/2024", null });
        }
    }
}
