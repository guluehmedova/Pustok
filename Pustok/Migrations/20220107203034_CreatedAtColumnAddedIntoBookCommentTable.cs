using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class CreatedAtColumnAddedIntoBookCommentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateddAt",
                table: "BookComments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateddAt",
                table: "BookComments");
        }
    }
}
