using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Joseph.WebApi.Migrations
{
    public partial class Setting_ManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobJobTypes",
                columns: table => new
                {
                    JobTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobJobTypes", x => new { x.JobId, x.JobTypeId });
                    table.ForeignKey(
                        name: "FK_JobJobTypes_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobJobTypes_JobTypes_JobTypeId",
                        column: x => x.JobTypeId,
                        principalTable: "JobTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobJobTypes_JobTypeId",
                table: "JobJobTypes",
                column: "JobTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobJobTypes");
        }
    }
}
