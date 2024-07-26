using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cachara.Data.Migrations
{
    /// <inheritdoc />
    public partial class Create_Table_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Posts");

            migrationBuilder.EnsureSchema(
                name: "Social");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Posts",
                newSchema: "Social");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                schema: "Social",
                table: "Posts",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Social",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                schema: "Social",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_User_AuthorId",
                schema: "Social",
                table: "Posts",
                column: "AuthorId",
                principalSchema: "Social",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_User_AuthorId",
                schema: "Social",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Social");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AuthorId",
                schema: "Social",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "Social",
                newName: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
