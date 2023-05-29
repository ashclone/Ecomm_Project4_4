using Microsoft.EntityFrameworkCore.Migrations;

namespace Ecomm_Project4_4.DataAccess.Migrations
{
    public partial class alterCartTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "Carts",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_ApplicationId",
                table: "Carts",
                newName: "IX_Carts_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationUserId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Carts",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_ApplicationUserId",
                table: "Carts",
                newName: "IX_Carts_ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_ApplicationId",
                table: "Carts",
                column: "ApplicationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
