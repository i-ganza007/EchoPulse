using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Echo.Migrations
{
    /// <inheritdoc />
    public partial class initialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    displayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isBanned = table.Column<bool>(type: "bit", nullable: false),
                    avatarUrl = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Jam",
                columns: table => new
                {
                    jamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    h3Index = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    h3Resolution = table.Column<int>(type: "int", nullable: false),
                    isPublic = table.Column<bool>(type: "bit", nullable: false),
                    jamQueueItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    startingTrackjamQueueItemid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    currentTrackId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    currentPositionMs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isplaying = table.Column<bool>(type: "bit", nullable: false),
                    participantCount = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    lastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jam", x => x.jamId);
                    table.ForeignKey(
                        name: "FK_Jam_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JamParticipant",
                columns: table => new
                {
                    jamParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    jamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    joinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leftAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isHost = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JamParticipant", x => x.jamParticipantId);
                    table.ForeignKey(
                        name: "FK_JamParticipant_Jam_jamId",
                        column: x => x.jamId,
                        principalTable: "Jam",
                        principalColumn: "jamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JamParticipant_User_userId",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JamQueueItem",
                columns: table => new
                {
                    jamQueueItemid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    jamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    trackId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trackTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trackArtist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    addedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    addingUseruserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    position = table.Column<int>(type: "int", nullable: false),
                    addedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isPlayed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JamQueueItem", x => x.jamQueueItemid);
                    table.ForeignKey(
                        name: "FK_JamQueueItem_Jam_jamId",
                        column: x => x.jamId,
                        principalTable: "Jam",
                        principalColumn: "jamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JamQueueItem_User_addingUseruserId",
                        column: x => x.addingUseruserId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JamUser",
                columns: table => new
                {
                    jamsJoinedjamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    participantsuserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JamUser", x => new { x.jamsJoinedjamId, x.participantsuserId });
                    table.ForeignKey(
                        name: "FK_JamUser_Jam_jamsJoinedjamId",
                        column: x => x.jamsJoinedjamId,
                        principalTable: "Jam",
                        principalColumn: "jamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JamUser_User_participantsuserId",
                        column: x => x.participantsuserId,
                        principalTable: "User",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jam_jamId",
                table: "Jam",
                column: "jamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jam_startingTrackjamQueueItemid",
                table: "Jam",
                column: "startingTrackjamQueueItemid");

            migrationBuilder.CreateIndex(
                name: "IX_Jam_userId",
                table: "Jam",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_JamParticipant_jamId",
                table: "JamParticipant",
                column: "jamId");

            migrationBuilder.CreateIndex(
                name: "IX_JamParticipant_userId",
                table: "JamParticipant",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_JamQueueItem_addingUseruserId",
                table: "JamQueueItem",
                column: "addingUseruserId");

            migrationBuilder.CreateIndex(
                name: "IX_JamQueueItem_jamId",
                table: "JamQueueItem",
                column: "jamId");

            migrationBuilder.CreateIndex(
                name: "IX_JamUser_participantsuserId",
                table: "JamUser",
                column: "participantsuserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_userId",
                table: "User",
                column: "userId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jam_JamQueueItem_startingTrackjamQueueItemid",
                table: "Jam",
                column: "startingTrackjamQueueItemid",
                principalTable: "JamQueueItem",
                principalColumn: "jamQueueItemid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jam_JamQueueItem_startingTrackjamQueueItemid",
                table: "Jam");

            migrationBuilder.DropTable(
                name: "JamParticipant");

            migrationBuilder.DropTable(
                name: "JamUser");

            migrationBuilder.DropTable(
                name: "JamQueueItem");

            migrationBuilder.DropTable(
                name: "Jam");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
