using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cuba_Staterkit.Migrations
{
    /// <inheritdoc />
    public partial class addCOOLLL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "tgl",
                table: "t_attendance",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tgl",
                table: "t_attendance");
        }
    }
}
