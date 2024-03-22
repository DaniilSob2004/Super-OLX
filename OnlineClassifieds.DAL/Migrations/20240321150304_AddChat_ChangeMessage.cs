using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineClassifieds.DAL.Migrations
{
    public partial class AddChat_ChangeMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Announcements_IdAnnouncement",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "IdAnnouncement",
                table: "Messages",
                newName: "IdChat");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_IdAnnouncement",
                table: "Messages",
                newName: "IX_Messages_IdChat");

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAnnouncement = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chat_Announcements_IdAnnouncement",
                        column: x => x.IdAnnouncement,
                        principalTable: "Announcements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_IdAnnouncement",
                table: "Chat",
                column: "IdAnnouncement");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chat_IdChat",
                table: "Messages",
                column: "IdChat",
                principalTable: "Chat",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chat_IdChat",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.RenameColumn(
                name: "IdChat",
                table: "Messages",
                newName: "IdAnnouncement");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_IdChat",
                table: "Messages",
                newName: "IX_Messages_IdAnnouncement");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Announcements_IdAnnouncement",
                table: "Messages",
                column: "IdAnnouncement",
                principalTable: "Announcements",
                principalColumn: "Id");
        }
    }
}
