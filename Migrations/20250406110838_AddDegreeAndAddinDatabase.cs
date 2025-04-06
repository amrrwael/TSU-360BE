using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TSU360.Migrations
{
    /// <inheritdoc />
    public partial class AddDegreeAndAddinDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "AspNetUsers");
        }
    }
}
