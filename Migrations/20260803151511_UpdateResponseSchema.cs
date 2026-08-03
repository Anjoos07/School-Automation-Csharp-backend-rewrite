using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace worklinnEdu.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResponseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kdrrod",
                schema: "responses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_forms",
                schema: "core",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "event_title",
                schema: "core",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "hs_events",
                schema: "core",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "hss_events",
                schema: "core",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "lp_events",
                schema: "core",
                table: "forms");

            migrationBuilder.DropColumn(
                name: "up_events",
                schema: "core",
                table: "forms");

            migrationBuilder.RenameTable(
                name: "forms",
                schema: "core",
                newName: "form",
                newSchema: "core");

            migrationBuilder.AddColumn<bool>(
                name: "form_closed",
                schema: "core",
                table: "form",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "form_name",
                schema: "core",
                table: "form",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_form",
                schema: "core",
                table: "form",
                column: "form_id");

            migrationBuilder.CreateTable(
                name: "field",
                schema: "core",
                columns: table => new
                {
                    field_id = table.Column<string>(type: "text", nullable: false),
                    form_id = table.Column<string>(type: "text", nullable: false),
                    field_name = table.Column<string>(type: "text", nullable: false),
                    field_type = table.Column<string>(type: "text", nullable: false),
                    field_required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_field", x => x.field_id);
                    table.ForeignKey(
                        name: "FK_field_form_form_id",
                        column: x => x.form_id,
                        principalSchema: "core",
                        principalTable: "form",
                        principalColumn: "form_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "response",
                schema: "responses",
                columns: table => new
                {
                    submission_id = table.Column<string>(type: "text", nullable: false),
                    form_id = table.Column<string>(type: "text", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    respondent_id = table.Column<string>(type: "text", nullable: true),
                    response = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_response", x => x.submission_id);
                    table.ForeignKey(
                        name: "FK_response_form_form_id",
                        column: x => x.form_id,
                        principalSchema: "core",
                        principalTable: "form",
                        principalColumn: "form_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_field_form_id",
                schema: "core",
                table: "field",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_response_form_id",
                schema: "responses",
                table: "response",
                column: "form_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "field",
                schema: "core");

            migrationBuilder.DropTable(
                name: "response",
                schema: "responses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_form",
                schema: "core",
                table: "form");

            migrationBuilder.DropColumn(
                name: "form_closed",
                schema: "core",
                table: "form");

            migrationBuilder.DropColumn(
                name: "form_name",
                schema: "core",
                table: "form");

            migrationBuilder.RenameTable(
                name: "form",
                schema: "core",
                newName: "forms",
                newSchema: "core");

            migrationBuilder.AddColumn<string>(
                name: "event_title",
                schema: "core",
                table: "forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hs_events",
                schema: "core",
                table: "forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hss_events",
                schema: "core",
                table: "forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lp_events",
                schema: "core",
                table: "forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "up_events",
                schema: "core",
                table: "forms",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_forms",
                schema: "core",
                table: "forms",
                column: "form_id");

            migrationBuilder.CreateTable(
                name: "kdrrod",
                schema: "responses",
                columns: table => new
                {
                    submission_id = table.Column<string>(type: "text", nullable: false),
                    classification = table.Column<string>(type: "text", nullable: true),
                    division = table.Column<string>(type: "text", nullable: true),
                    events = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    respondent_id = table.Column<string>(type: "text", nullable: true),
                    roll_no = table.Column<string>(type: "text", nullable: true),
                    student_class = table.Column<string>(type: "text", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kdrrod", x => x.submission_id);
                });
        }
    }
}
