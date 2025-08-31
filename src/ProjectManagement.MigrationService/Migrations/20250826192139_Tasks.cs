using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetProjectManagement.ProjectManagement.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class Tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskCreatedActivityId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Activities",
                type: "character varying(8191)",
                maxLength: 8191,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewDescription",
                table: "Activities",
                type: "character varying(8191)",
                maxLength: 8191,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldDescription",
                table: "Activities",
                type: "character varying(8191)",
                maxLength: 8191,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskCreatedActivity_DisplayName",
                table: "Activities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Activities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskUpdatedActivity_NewDisplayName",
                table: "Activities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskUpdatedActivity_OldDisplayName",
                table: "Activities",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(8191)", maxLength: 8191, nullable: false),
                    Open = table.Column<bool>(type: "boolean", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskUpdatedActivityUser",
                columns: table => new
                {
                    NewAssigneesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskUpdatedActivityNewAssigneesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUpdatedActivityUser", x => new { x.NewAssigneesId, x.TaskUpdatedActivityNewAssigneesId });
                    table.ForeignKey(
                        name: "FK_TaskUpdatedActivityUser_Activities_TaskUpdatedActivityNewAs~",
                        column: x => x.TaskUpdatedActivityNewAssigneesId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskUpdatedActivityUser_Users_NewAssigneesId",
                        column: x => x.NewAssigneesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskUpdatedActivityUser1",
                columns: table => new
                {
                    OldAssigneesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskUpdatedActivityOldAssigneesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUpdatedActivityUser1", x => new { x.OldAssigneesId, x.TaskUpdatedActivityOldAssigneesId });
                    table.ForeignKey(
                        name: "FK_TaskUpdatedActivityUser1_Activities_TaskUpdatedActivityOldA~",
                        column: x => x.TaskUpdatedActivityOldAssigneesId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskUpdatedActivityUser1_Users_OldAssigneesId",
                        column: x => x.OldAssigneesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskUser",
                columns: table => new
                {
                    AssignedTasksId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssigneesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskUser", x => new { x.AssignedTasksId, x.AssigneesId });
                    table.ForeignKey(
                        name: "FK_ProjectTaskUser_Tasks_AssignedTasksId",
                        column: x => x.AssignedTasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTaskUser_Users_AssigneesId",
                        column: x => x.AssigneesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TaskCreatedActivityId",
                table: "Users",
                column: "TaskCreatedActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TaskId",
                table: "Activities",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskUser_AssigneesId",
                table: "ProjectTaskUser",
                column: "AssigneesId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskUpdatedActivityUser_TaskUpdatedActivityNewAssigneesId",
                table: "TaskUpdatedActivityUser",
                column: "TaskUpdatedActivityNewAssigneesId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskUpdatedActivityUser1_TaskUpdatedActivityOldAssigneesId",
                table: "TaskUpdatedActivityUser1",
                column: "TaskUpdatedActivityOldAssigneesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Activities_TaskCreatedActivityId",
                table: "Users",
                column: "TaskCreatedActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Activities_TaskCreatedActivityId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ProjectTaskUser");

            migrationBuilder.DropTable(
                name: "TaskUpdatedActivityUser");

            migrationBuilder.DropTable(
                name: "TaskUpdatedActivityUser1");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Users_TaskCreatedActivityId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Activities_TaskId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskCreatedActivityId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "NewDescription",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "OldDescription",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskCreatedActivity_DisplayName",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskUpdatedActivity_NewDisplayName",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TaskUpdatedActivity_OldDisplayName",
                table: "Activities");

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_CreatedByUserId",
                table: "Document",
                column: "CreatedByUserId");
        }
    }
}
