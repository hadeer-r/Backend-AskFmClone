using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskFm.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Typo_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FolloweingCount",
                table: "Users",
                newName: "FollowingCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FollowingCount",
                table: "Users",
                newName: "FolloweingCount");
        }
    }
}
