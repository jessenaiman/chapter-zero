using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable RCS1021, CA1861

namespace OmegaSpiral.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SaveSlot = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SaveVersion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CurrentScene = table.Column<int>(type: "INTEGER", nullable: false),
                    DreamweaverThread = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PlayerSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SelectedDreamweaver = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartySaveData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartySaveData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartySaveData_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DreamweaverScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    DreamweaverType = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DreamweaverScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DreamweaverScores_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NarratorMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarratorMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NarratorMessages_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SceneData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SceneData_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryShards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameSaveId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShardId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryShards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryShards_GameSaves_GameSaveId",
                        column: x => x.GameSaveId,
                        principalTable: "GameSaves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSaveData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartySaveDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Experience = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxHealth = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSaveData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterSaveData_PartySaveData_PartySaveDataId",
                        column: x => x.PartySaveDataId,
                        principalTable: "PartySaveData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSaveData_PartySaveDataId",
                table: "CharacterSaveData",
                column: "PartySaveDataId");

            migrationBuilder.CreateIndex(
                name: "IX_DreamweaverScores_GameSaveId_DreamweaverType",
                table: "DreamweaverScores",
                columns: new[] { "GameSaveId", "DreamweaverType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_SaveSlot",
                table: "GameSaves",
                column: "SaveSlot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NarratorMessages_GameSaveId",
                table: "NarratorMessages",
                column: "GameSaveId");

            migrationBuilder.CreateIndex(
                name: "IX_PartySaveData_GameSaveId",
                table: "PartySaveData",
                column: "GameSaveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SceneData_GameSaveId_Key",
                table: "SceneData",
                columns: new[] { "GameSaveId", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_StoryShards_GameSaveId",
                table: "StoryShards",
                column: "GameSaveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterSaveData");

            migrationBuilder.DropTable(
                name: "DreamweaverScores");

            migrationBuilder.DropTable(
                name: "NarratorMessages");

            migrationBuilder.DropTable(
                name: "SceneData");

            migrationBuilder.DropTable(
                name: "StoryShards");

            migrationBuilder.DropTable(
                name: "PartySaveData");

            migrationBuilder.DropTable(
                name: "GameSaves");
        }
    }
}
