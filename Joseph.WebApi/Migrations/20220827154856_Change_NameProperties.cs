using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Joseph.WebApi.Migrations
{
    public partial class Change_NameProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_IdentityUserId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Jobs",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_IdentityUserId",
                table: "Jobs",
                newName: "IX_Jobs_UserId");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Answers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_IdentityUserId",
                table: "Answers",
                newName: "IX_Answers_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_UserId",
                table: "Jobs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_UserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_UserId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Jobs",
                newName: "IdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_UserId",
                table: "Jobs",
                newName: "IX_Jobs_IdentityUserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Answers",
                newName: "IdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_UserId",
                table: "Answers",
                newName: "IX_Answers_IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId",
                table: "Answers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_IdentityUserId",
                table: "Jobs",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
