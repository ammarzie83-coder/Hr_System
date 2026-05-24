using System;
using Hr_System.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hr_System.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260520100000_AddLoginLockFields")]
    public partial class AddLoginLockFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "UserAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndUtc",
                table: "UserAccounts",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "LockoutEndUtc",
                table: "UserAccounts");
        }
    }
}
