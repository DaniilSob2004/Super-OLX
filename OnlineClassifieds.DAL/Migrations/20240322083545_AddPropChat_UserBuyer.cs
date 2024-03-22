using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineClassifieds.DAL.Migrations
{
    public partial class AddPropChat_UserBuyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdBuyer",
                table: "Chat",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_IdBuyer",
                table: "Chat",
                column: "IdBuyer");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_IdBuyer",
                table: "Chat",
                column: "IdBuyer",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_IdBuyer",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_IdBuyer",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "IdBuyer",
                table: "Chat");
        }
    }
}
