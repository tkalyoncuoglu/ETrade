using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FifthMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStores_Products_ProductId",
                table: "ProductStores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStores_Stores_StoreId",
                table: "ProductStores");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStores_Products_ProductId",
                table: "ProductStores",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStores_Stores_StoreId",
                table: "ProductStores",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStores_Products_ProductId",
                table: "ProductStores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStores_Stores_StoreId",
                table: "ProductStores");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStores_Products_ProductId",
                table: "ProductStores",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStores_Stores_StoreId",
                table: "ProductStores",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");
        }
    }
}
