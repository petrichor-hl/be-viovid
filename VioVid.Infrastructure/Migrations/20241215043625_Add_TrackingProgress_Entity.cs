using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VioVid.Infrastructure.Migrations
{
    public partial class Add_TrackingProgress_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackingProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    EpisodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackingProgresses_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackingProgresses_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingProgresses_ApplicationUserId",
                table: "TrackingProgresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackingProgresses_EpisodeId",
                table: "TrackingProgresses",
                column: "EpisodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingProgresses");
        }
    }
}
