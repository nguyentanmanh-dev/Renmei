using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Renmei.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredStudyTime",
                table: "Users",
                type: "varchar(5)",
                unicode: false,
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReviewLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiId = table.Column<int>(type: "int", nullable: false),
                    ClientEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    PrevEase = table.Column<double>(type: "float", nullable: false),
                    NewEase = table.Column<double>(type: "float", nullable: false),
                    PrevInterval = table.Column<int>(type: "int", nullable: false),
                    NewInterval = table.Column<int>(type: "int", nullable: false),
                    TimeTakenMs = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewLogs", x => x.Id);
                    table.CheckConstraint("CK_ReviewLogs_Rating", "[Rating] BETWEEN 0 AND 5");
                    table.CheckConstraint("CK_ReviewLogs_TimeTakenMs", "[TimeTakenMs] IS NULL OR [TimeTakenMs] >= 0");
                    table.ForeignKey(
                        name: "FK_ReviewLogs_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserKanjiProgress",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KanjiId = table.Column<int>(type: "int", nullable: false),
                    EaseFactor = table.Column<double>(type: "float", nullable: false, defaultValue: 2.5),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false),
                    LapseCount = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "New"),
                    IsLeech = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKanjiProgress", x => new { x.UserId, x.KanjiId });
                    table.CheckConstraint("CK_UserKanjiProgress_EaseFactor", "[EaseFactor] >= 1.3");
                    table.CheckConstraint("CK_UserKanjiProgress_IntervalDays", "[IntervalDays] >= 0");
                    table.CheckConstraint("CK_UserKanjiProgress_LapseCount", "[LapseCount] >= 0");
                    table.CheckConstraint("CK_UserKanjiProgress_Repetitions", "[Repetitions] >= 0");
                    table.ForeignKey(
                        name: "FK_UserKanjiProgress_Kanji_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserKanjiProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStreaks",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    LastStudyDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StreakFreezeCount = table.Column<int>(type: "int", nullable: false),
                    LastFreezeGrantMonth = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStreaks", x => x.UserId);
                    table.CheckConstraint("CK_UserStreaks_Current", "[CurrentStreak] >= 0");
                    table.CheckConstraint("CK_UserStreaks_Freeze", "[StreakFreezeCount] >= 0");
                    table.CheckConstraint("CK_UserStreaks_Longest", "[LongestStreak] >= [CurrentStreak]");
                    table.ForeignKey(
                        name: "FK_UserStreaks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewLogs_ClientEventId",
                table: "ReviewLogs",
                column: "ClientEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewLogs_KanjiId",
                table: "ReviewLogs",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewLogs_UserId_ReviewedAt",
                table: "ReviewLogs",
                columns: new[] { "UserId", "ReviewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserKanjiProgress_KanjiId",
                table: "UserKanjiProgress",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKanjiProgress_UserId_DueDate",
                table: "UserKanjiProgress",
                columns: new[] { "UserId", "DueDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewLogs");

            migrationBuilder.DropTable(
                name: "UserKanjiProgress");

            migrationBuilder.DropTable(
                name: "UserStreaks");

            migrationBuilder.DropColumn(
                name: "PreferredStudyTime",
                table: "Users");
        }
    }
}
