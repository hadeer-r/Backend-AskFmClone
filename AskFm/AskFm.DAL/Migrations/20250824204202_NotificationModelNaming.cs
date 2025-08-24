using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskFm.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NotificationModelNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isRead",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "jsonContent",
                table: "Notifications",
                newName: "Message");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "Notifications",
                newName: "isRead");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notifications",
                newName: "jsonContent");
        }
    }
}
