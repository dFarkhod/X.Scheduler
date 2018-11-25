using Microsoft.EntityFrameworkCore.Migrations;

namespace X.Scheduler.Persistence.Migrations
{
    public partial class someRefactoring3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Schedule_StaffId",
                table: "Schedule",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Staff_StaffId",
                table: "Schedule",
                column: "StaffId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Staff_StaffId",
                table: "Schedule");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_StaffId",
                table: "Schedule");
        }
    }
}
