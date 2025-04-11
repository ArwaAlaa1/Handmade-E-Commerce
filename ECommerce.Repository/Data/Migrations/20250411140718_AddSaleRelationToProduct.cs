using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleRelationToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Products_ProductId",
                table: "Sale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sale",
                table: "Sale");

            migrationBuilder.RenameTable(
                name: "Sale",
                newName: "Sales");

            migrationBuilder.RenameIndex(
                name: "IX_Sale_ProductId",
                table: "Sales",
                newName: "IX_Sales_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sales",
                table: "Sales",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Products_ProductId",
                table: "Sales",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Products_ProductId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sales",
                table: "Sales");

            migrationBuilder.RenameTable(
                name: "Sales",
                newName: "Sale");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_ProductId",
                table: "Sale",
                newName: "IX_Sale_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sale",
                table: "Sale",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Products_ProductId",
                table: "Sale",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
