using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GrpcTestResultService.Migrations
{
    /// <inheritdoc />
    public partial class TestTypeRealtionship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_testTypes",
                table: "testTypes");

            migrationBuilder.RenameTable(
                name: "testTypes",
                newName: "TestTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestTypes",
                table: "TestTypes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TestTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestTypeId",
                table: "TestResults",
                column: "TestTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestTypes",
                table: "TestTypes");

            migrationBuilder.RenameTable(
                name: "TestTypes",
                newName: "testTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_testTypes",
                table: "testTypes",
                column: "Id");
        }
    }
}
