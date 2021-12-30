using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class NewBookImagesTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewBookImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    Image = table.Column<string>(maxLength: 100, nullable: true),
                    PosterStatus = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewBookImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewBookImages_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewBookImages_BookId",
                table: "NewBookImages",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewBookImages");
        }
    }
}
