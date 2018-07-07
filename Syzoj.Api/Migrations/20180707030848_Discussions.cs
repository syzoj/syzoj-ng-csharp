using Microsoft.EntityFrameworkCore.Migrations;

namespace Syzoj.Api.Migrations
{
    public partial class Discussions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discussions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ShowInBoard = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discussions_Users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReplyEntry",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    AuthorEmail = table.Column<string>(nullable: true),
                    DiscussionEntryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplyEntry_Users_AuthorEmail",
                        column: x => x.AuthorEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplyEntry_Discussions_DiscussionEntryId",
                        column: x => x.DiscussionEntryId,
                        principalTable: "Discussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_AuthorEmail",
                table: "Discussions",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyEntry_AuthorEmail",
                table: "ReplyEntry",
                column: "AuthorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyEntry_DiscussionEntryId",
                table: "ReplyEntry",
                column: "DiscussionEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplyEntry");

            migrationBuilder.DropTable(
                name: "Discussions");
        }
    }
}
