using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FileUpload.Migrations
{
    public partial class final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileOnDatabaseModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UploadedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileOnDatabaseModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileOnFileSystemModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UploadedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    FilePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileOnFileSystemModel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileOnDatabaseModels");

            migrationBuilder.DropTable(
                name: "FileOnFileSystemModel");
        }
    }
}
