using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToNewsArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "NewsArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_CreatedBy",
                table: "NewsArticles",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsArticles_SystemAccounts_CreatedBy",
                table: "NewsArticles",
                column: "CreatedBy",
                principalTable: "SystemAccounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsArticles_SystemAccounts_CreatedBy",
                table: "NewsArticles");

            migrationBuilder.DropIndex(
                name: "IX_NewsArticles_CreatedBy",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "NewsArticles");
        }
    }
}
