using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverForest.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class KeysToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[] { -1, "ryan@miaz.xyz", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
