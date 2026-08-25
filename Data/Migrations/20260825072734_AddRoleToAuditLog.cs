using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourManagement.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AuditLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AuditLogs");
        }
    }
}
