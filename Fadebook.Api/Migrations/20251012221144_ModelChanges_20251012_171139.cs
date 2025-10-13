using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ModelChanges_20251012_171139 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "barberWorkHoursTable",
                columns: table => new
                {
                    WorkHourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarberId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barberWorkHoursTable", x => x.WorkHourId);
                    table.ForeignKey(
                        name: "FK_barberWorkHoursTable_barberTable_BarberId",
                        column: x => x.BarberId,
                        principalTable: "barberTable",
                        principalColumn: "BarberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_barberWorkHoursTable_BarberId_DayOfWeek_StartTime_EndTime",
                table: "barberWorkHoursTable",
                columns: new[] { "BarberId", "DayOfWeek", "StartTime", "EndTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "barberWorkHoursTable");
        }
    }
}
