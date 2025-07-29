using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AskFm.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Test_SavedThread_Creation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedThreads",
                columns: table => new
                {
                    SavedThreadId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedThreads", x => new { x.SavedThreadId, x.UserId });
            
                    table.ForeignKey(
                        name: "FK_SavedThreads_Thread_SavedThreadId",
                        column: x => x.SavedThreadId,
                        principalTable: "Thread",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                
                    table.ForeignKey(
                        name: "FK_SavedThreads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedThreads_UserId",
                table: "SavedThreads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedThreads");
        }
    }
}
