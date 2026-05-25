using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hr_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add indexes to Employee table
            migrationBuilder.CreateIndex(
                name: "IX_Employees_NationalId",
                table: "Employees",
                column: "NationalId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            // Add indexes to EmployeeAttachments table
            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttachments_EmployeeId",
                table: "EmployeeAttachments",
                column: "EmployeeId");

            // Add indexes to LeaveRequests table
            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_Status",
                table: "LeaveRequests",
                column: "Status");

            // Add indexes to AuditLogs table
            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TableName",
                table: "AuditLogs",
                column: "TableName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs",
                column: "ChangedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes in reverse order
            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TableName",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_Status",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_EmployeeId",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAttachments_EmployeeId",
                table: "EmployeeAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Department",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_NationalId",
                table: "Employees");
        }
    }
}
