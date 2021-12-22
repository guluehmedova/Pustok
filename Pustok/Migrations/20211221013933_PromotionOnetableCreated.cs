using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustok.Migrations
{
    public partial class PromotionOnetableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Books_BookId",
                table: "BookTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTag_Tags_TagId",
                table: "BookTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookTag",
                table: "BookTag");

            migrationBuilder.RenameTable(
                name: "BookTag",
                newName: "BookTags");

            migrationBuilder.RenameIndex(
                name: "IX_BookTag_TagId",
                table: "BookTags",
                newName: "IX_BookTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_BookTag_BookId",
                table: "BookTags",
                newName: "IX_BookTags_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookTags",
                table: "BookTags",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PromotionOnes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(nullable: true),
                    RedirectUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionOnes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Books_BookId",
                table: "BookTags",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Tags_TagId",
                table: "BookTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Books_BookId",
                table: "BookTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Tags_TagId",
                table: "BookTags");

            migrationBuilder.DropTable(
                name: "PromotionOnes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookTags",
                table: "BookTags");

            migrationBuilder.RenameTable(
                name: "BookTags",
                newName: "BookTag");

            migrationBuilder.RenameIndex(
                name: "IX_BookTags_TagId",
                table: "BookTag",
                newName: "IX_BookTag_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_BookTags_BookId",
                table: "BookTag",
                newName: "IX_BookTag_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookTag",
                table: "BookTag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Books_BookId",
                table: "BookTag",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTag_Tags_TagId",
                table: "BookTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
