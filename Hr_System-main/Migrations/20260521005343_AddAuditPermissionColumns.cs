using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hr_System.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditPermissionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditAuditLogs",
                table: "UserAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanViewAuditLogs",
                table: "UserAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEditAuditLogs",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "CanViewAuditLogs",
                table: "UserAccounts");
        }
    }
}
