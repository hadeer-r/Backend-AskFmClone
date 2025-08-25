using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskFm.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixIsActiveColumn_Follow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Follows",
                type: "BIT",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Follows",
                type: "bit",
                nullable: false,
                defaultValue: true
                );
        }
    }
}
