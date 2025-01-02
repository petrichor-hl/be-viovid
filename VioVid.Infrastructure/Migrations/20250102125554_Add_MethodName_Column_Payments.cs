using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VioVid.Infrastructure.Migrations
{
    public partial class Add_MethodName_Column_Payments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MethodName",
                table: "Payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MethodName",
                table: "Payments");
        }
    }
}
