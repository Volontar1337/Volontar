using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationMemberEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationProfileId = table.Column<Guid>(type: "TEXT", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_OrganizationProfiles_OrganizationProfileId",
                        column: x => x.OrganizationProfileId,
                        principalTable: "OrganizationProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_OrganizationProfileId",
                table: "OrganizationMembers",
                column: "OrganizationProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_UserId",
                table: "OrganizationMembers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationMembers");
        }
    }
}
