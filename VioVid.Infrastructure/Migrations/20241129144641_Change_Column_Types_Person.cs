using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VioVid.Infrastructure.Migrations
{
    public partial class Change_Column_Types_Person : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePath",
                table: "Persons",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "Popularity",
                table: "Persons",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Dob",
                table: "Persons",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePath",
                table: "Persons",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Popularity",
                table: "Persons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "Persons",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
