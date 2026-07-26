using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace worklinnEdu.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "responses");

            migrationBuilder.CreateTable(
                name: "forms",
                schema: "core",
                columns: table => new
                {
                    form_id = table.Column<string>(type: "text", nullable: false),
                    event_title = table.Column<string>(type: "text", nullable: true),
                    lp_events = table.Column<string>(type: "text", nullable: true),
                    up_events = table.Column<string>(type: "text", nullable: true),
                    hs_events = table.Column<string>(type: "text", nullable: true),
                    hss_events = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forms", x => x.form_id);
                });

            migrationBuilder.CreateTable(
                name: "kdrrod",
                schema: "responses",
                columns: table => new
                {
                    submission_id = table.Column<string>(type: "text", nullable: false),
                    respondent_id = table.Column<string>(type: "text", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    student_class = table.Column<string>(type: "text", nullable: true),
                    division = table.Column<string>(type: "text", nullable: true),
                    roll_no = table.Column<string>(type: "text", nullable: true),
                    events = table.Column<string>(type: "text", nullable: true),
                    classification = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kdrrod", x => x.submission_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forms",
                schema: "core");

            migrationBuilder.DropTable(
                name: "kdrrod",
                schema: "responses");
        }
    }
}
