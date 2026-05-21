using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Career_Path.Migrations
{
    /// <inheritdoc />
    public partial class updateAij : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhaseResource");

            migrationBuilder.DropTable(
                name: "PhaseSkill");

            migrationBuilder.DropTable(
                name: "ProjectImprovement");

            migrationBuilder.DropTable(
                name: "PrompetRoadMap");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "RoadmapPhase");

            migrationBuilder.DropTable(
                name: "Roadmaps");

            migrationBuilder.DropIndex(
                name: "IX_ModelExtrations_ApplicationUserId",
                table: "ModelExtrations");

            migrationBuilder.CreateTable(
                name: "JobInterviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInterviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobInterviews_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapJsons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoadmapData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSaved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapJsons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapJsons_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobInterviewOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobInterviewId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInterviewOption", x => new { x.JobInterviewId, x.Id });
                    table.ForeignKey(
                        name: "FK_JobInterviewOption_JobInterviews_JobInterviewId",
                        column: x => x.JobInterviewId,
                        principalTable: "JobInterviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelExtrations_ApplicationUserId",
                table: "ModelExtrations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobInterviews_JobId",
                table: "JobInterviews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapJsons_ApplicationUserId",
                table: "RoadmapJsons",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobInterviewOption");

            migrationBuilder.DropTable(
                name: "RoadmapJsons");

            migrationBuilder.DropTable(
                name: "JobInterviews");

            migrationBuilder.DropIndex(
                name: "IX_ModelExtrations_ApplicationUserId",
                table: "ModelExtrations");

            migrationBuilder.CreateTable(
                name: "PrompetRoadMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrompetRoadMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrompetRoadMap_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Roadmaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentDomain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: false),
                    IsValidTransition = table.Column<bool>(type: "bit", nullable: false),
                    MermaidDiagram = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransitionDifficulty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roadmaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roadmaps_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Skills_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImprovement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadmapId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImprovement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectImprovement_Roadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "Roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapPhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadmapId = table.Column<int>(type: "int", nullable: false),
                    FocusArea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapPhase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapPhase_Roadmaps_RoadmapId",
                        column: x => x.RoadmapId,
                        principalTable: "Roadmaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhaseResource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadmapPhaseId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhaseResource_RoadmapPhase_RoadmapPhaseId",
                        column: x => x.RoadmapPhaseId,
                        principalTable: "RoadmapPhase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhaseSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadmapPhaseId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhaseSkill_RoadmapPhase_RoadmapPhaseId",
                        column: x => x.RoadmapPhaseId,
                        principalTable: "RoadmapPhase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelExtrations_ApplicationUserId",
                table: "ModelExtrations",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhaseResource_RoadmapPhaseId",
                table: "PhaseResource",
                column: "RoadmapPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PhaseSkill_RoadmapPhaseId",
                table: "PhaseSkill",
                column: "RoadmapPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImprovement_RoadmapId",
                table: "ProjectImprovement",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_PrompetRoadMap_UserId",
                table: "PrompetRoadMap",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapPhase_RoadmapId",
                table: "RoadmapPhase",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_ApplicationUserId",
                table: "Roadmaps",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_UserProfileId",
                table: "Skills",
                column: "UserProfileId");
        }
    }
}
