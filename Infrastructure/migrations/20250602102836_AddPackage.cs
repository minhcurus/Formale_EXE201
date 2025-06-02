using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductTypes",
                newName: "ProductTypes",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductStyles",
                newName: "ProductStyles",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductSizes",
                newName: "ProductSizes",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductMaterials",
                newName: "ProductMaterials",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductColors",
                newName: "ProductColors",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductCategorySizes",
                newName: "ProductCategorySizes",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                newName: "ProductCategories",
                newSchema: "sps13686_hiTech");

            migrationBuilder.RenameTable(
                name: "ProductBrands",
                newName: "ProductBrands",
                newSchema: "sps13686_hiTech");

            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumExpiryDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PremiumPackageId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Price",
                table: "PremiumPackages",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "PremiumPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "OrderCode",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageURL",
                schema: "sps13686_hiTech",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "PremiumPackages",
                columns: new[] { "Id", "Description", "DurationInDays", "Name", "Price", "Tier" },
                values: new object[,]
                {
                    { 1, "Gói cơ bản, hỗ trợ hạn chế.", 7, "Bronze Package", 50000L, 0 },
                    { 2, "Gói nâng cao với nhiều tính năng hơn.", 30, "Silver Package", 120000L, 1 },
                    { 3, "Gói cao cấp đầy đủ tính năng.", 90, "Gold Package", 300000L, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PremiumPackageId",
                table: "Users",
                column: "PremiumPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PremiumPackages_PremiumPackageId",
                table: "Users",
                column: "PremiumPackageId",
                principalTable: "PremiumPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PremiumPackages_PremiumPackageId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PremiumPackageId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "PremiumExpiryDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PremiumPackageId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "PremiumPackages");

            migrationBuilder.RenameTable(
                name: "ProductTypes",
                schema: "sps13686_hiTech",
                newName: "ProductTypes");

            migrationBuilder.RenameTable(
                name: "ProductStyles",
                schema: "sps13686_hiTech",
                newName: "ProductStyles");

            migrationBuilder.RenameTable(
                name: "ProductSizes",
                schema: "sps13686_hiTech",
                newName: "ProductSizes");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "sps13686_hiTech",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "ProductMaterials",
                schema: "sps13686_hiTech",
                newName: "ProductMaterials");

            migrationBuilder.RenameTable(
                name: "ProductColors",
                schema: "sps13686_hiTech",
                newName: "ProductColors");

            migrationBuilder.RenameTable(
                name: "ProductCategorySizes",
                schema: "sps13686_hiTech",
                newName: "ProductCategorySizes");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                schema: "sps13686_hiTech",
                newName: "ProductCategories");

            migrationBuilder.RenameTable(
                name: "ProductBrands",
                schema: "sps13686_hiTech",
                newName: "ProductBrands");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "PremiumPackages",
                type: "float",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "OrderCode",
                table: "Payments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ImageURL",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
