using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace X.Scheduler.Persistence.Migrations
{
    public partial class firstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    StaffGuidId = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Shift = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
