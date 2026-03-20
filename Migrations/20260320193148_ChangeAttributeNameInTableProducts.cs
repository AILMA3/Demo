using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttributeNameInTableProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductNames_ProductNamesId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductNamesId",
                table: "Products",
                newName: "ProductNameId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductNamesId",
                table: "Products",
                newName: "IX_Products_ProductNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductNames_ProductNameId",
                table: "Products",
                column: "ProductNameId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductNames_ProductNameId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductNameId",
                table: "Products",
                newName: "ProductNamesId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductNameId",
                table: "Products",
                newName: "IX_Products_ProductNamesId");

            migrationBuilder.AddColumn<int>(
                name: "NameId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductNames_ProductNamesId",
                table: "Products",
                column: "ProductNamesId",
                principalTable: "ProductNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
