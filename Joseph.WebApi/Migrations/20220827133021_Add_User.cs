using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Joseph.WebApi.Migrations
{
    public partial class Add_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId1",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_IdentityUserId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId1",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Jobs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Answers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_IdentityUserId",
                table: "Jobs",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_IdentityUserId",
                table: "Answers",
                column: "IdentityUserId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_IdentityUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_IdentityUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Answers_IdentityUserId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Jobs");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdentityUserId",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId1",
                table: "Answers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_IdentityUserId1",
                table: "Answers",
                column: "IdentityUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId1",
                table: "Answers",
                column: "IdentityUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
