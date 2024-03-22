using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineClassifieds.DAL.Migrations
{
    public partial class AddChat_Prop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdOwner",
                table: "Chat",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_IdOwner",
                table: "Chat",
                column: "IdOwner");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_IdOwner",
                table: "Chat",
                column: "IdOwner",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_IdOwner",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_IdOwner",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "IdOwner",
                table: "Chat");
        }
    }
}
