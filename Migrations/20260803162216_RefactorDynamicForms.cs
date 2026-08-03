using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace worklinnEdu.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDynamicForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "field_required",
                schema: "core",
                table: "field");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "field_required",
                schema: "core",
                table: "field",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
