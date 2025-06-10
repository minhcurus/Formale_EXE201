using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addGoogle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "PremiumPackages",
                newName: "PremiumPackages",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payments",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "sps13686_hiTech");

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                schema: "sps13686_hiTech",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginProvider",
                schema: "sps13686_hiTech",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "sps13686_hiTech",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "sps13686_hiTech",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "PremiumPackages",
                schema: "sps13686_hiTech",
                newName: "PremiumPackages");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "sps13686_hiTech",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "sps13686_hiTech",
                newName: "Orders");
        }
    }
}
