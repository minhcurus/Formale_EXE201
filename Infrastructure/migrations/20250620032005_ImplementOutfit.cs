using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImplementOutfit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemCreated",
                schema: "sps13686_hiTech",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "sps13686_hiTech",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutfitCombos",
                schema: "sps13686_hiTech",
                columns: table => new
                {
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutfitCombos", x => x.ComboId);
                    table.ForeignKey(
                        name: "FK_OutfitCombos_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutfitComboItems",
                schema: "sps13686_hiTech",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutfitComboItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutfitComboItems_OutfitCombos_ComboId",
                        column: x => x.ComboId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "OutfitCombos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutfitComboItems_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "ProductCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutfitComboItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClosets",
                schema: "sps13686_hiTech",
                columns: table => new
                {
                    ClosetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComboId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClosets", x => x.ClosetId);
                    table.ForeignKey(
                        name: "FK_UserClosets_OutfitCombos_ComboId",
                        column: x => x.ComboId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "OutfitCombos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserClosets_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserClosets_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "sps13686_hiTech",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                schema: "sps13686_hiTech",
                table: "Products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitComboItems_CategoryId",
                schema: "sps13686_hiTech",
                table: "OutfitComboItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitComboItems_ComboId",
                schema: "sps13686_hiTech",
                table: "OutfitComboItems",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitComboItems_ProductId",
                schema: "sps13686_hiTech",
                table: "OutfitComboItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitCombos_UserId",
                schema: "sps13686_hiTech",
                table: "OutfitCombos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClosets_ComboId",
                schema: "sps13686_hiTech",
                table: "UserClosets",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClosets_ProductId",
                schema: "sps13686_hiTech",
                table: "UserClosets",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClosets_UserId",
                schema: "sps13686_hiTech",
                table: "UserClosets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_UserId",
                schema: "sps13686_hiTech",
                table: "Products",
                column: "UserId",
                principalSchema: "sps13686_hiTech",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserId",
                schema: "sps13686_hiTech",
                table: "Products");

            migrationBuilder.DropTable(
                name: "OutfitComboItems",
                schema: "sps13686_hiTech");

            migrationBuilder.DropTable(
                name: "UserClosets",
                schema: "sps13686_hiTech");

            migrationBuilder.DropTable(
                name: "OutfitCombos",
                schema: "sps13686_hiTech");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserId",
                schema: "sps13686_hiTech",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsSystemCreated",
                schema: "sps13686_hiTech",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "sps13686_hiTech",
                table: "Products");
        }
    }
}
