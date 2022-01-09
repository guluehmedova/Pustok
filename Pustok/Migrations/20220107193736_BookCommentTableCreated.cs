using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class BookCommentTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "Tags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    Text = table.Column<string>(maxLength: 500, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    FullName = table.Column<string>(maxLength: 50, nullable: true),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookComments_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookComments_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BookId",
                table: "Tags",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_AppUserId",
                table: "BookComments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_BookId",
                table: "BookComments",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Books_BookId",
                table: "Tags",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Books_BookId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "BookComments");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BookId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Tags");
        }
    }
}
