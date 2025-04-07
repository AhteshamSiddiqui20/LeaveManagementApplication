using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveService.Migrations
{
    /// <inheritdoc />
    public partial class changesinLeaveEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaveType",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "LeaveType",
                table: "LeaveBalance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaveType",
                table: "LeaveRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LeaveType",
                table: "LeaveBalance",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
