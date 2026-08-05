using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace worklinnEdu.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFormsStructure2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "grouping_priority",
                schema: "core",
                table: "field",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grouping_priority",
                schema: "core",
                table: "field");
        }
    }
}
