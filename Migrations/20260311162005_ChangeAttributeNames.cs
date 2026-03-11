using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttributeNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "Roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductSupplierName",
                table: "ProductSuppliers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "ProductNames",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductMeasureName",
                table: "ProductMeasures",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductManufacturerName",
                table: "ProductManufacturers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductCategoryName",
                table: "ProductCategorys",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StatusName",
                table: "OrderStatuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Deliverydate",
                table: "Orders",
                newName: "DeliveryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Roles",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductSuppliers",
                newName: "ProductSupplierName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductNames",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductMeasures",
                newName: "ProductMeasureName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductManufacturers",
                newName: "ProductManufacturerName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductCategorys",
                newName: "ProductCategoryName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderStatuses",
                newName: "StatusName");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "Orders",
                newName: "Deliverydate");
        }
    }
}
