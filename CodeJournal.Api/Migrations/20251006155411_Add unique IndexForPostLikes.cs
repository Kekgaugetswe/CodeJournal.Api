using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeJournal.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdduniqueIndexForPostLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPostLike_BlogPostId",
                table: "BlogPostLike");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BlogPostLike",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "LikedAt",
                table: "BlogPostLike",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostLike_BlogPostId_UserId",
                table: "BlogPostLike",
                columns: new[] { "BlogPostId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPostLike_BlogPostId_UserId",
                table: "BlogPostLike");

            migrationBuilder.DropColumn(
                name: "LikedAt",
                table: "BlogPostLike");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "BlogPostLike",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostLike_BlogPostId",
                table: "BlogPostLike",
                column: "BlogPostId");
        }
    }
}
