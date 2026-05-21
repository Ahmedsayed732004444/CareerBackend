using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Career_Path.Migrations
{
    /// <inheritdoc />
    public partial class addProfilIdforikeyProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HardSkills_UserProfiles_UserProfileID",
                table: "HardSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_SoftSkills_UserProfiles_UserProfileID",
                table: "SoftSkills");

            migrationBuilder.RenameColumn(
                name: "UserProfileID",
                table: "SoftSkills",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_SoftSkills_UserProfileID",
                table: "SoftSkills",
                newName: "IX_SoftSkills_UserProfileId");

            migrationBuilder.RenameColumn(
                name: "UserProfileID",
                table: "HardSkills",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_HardSkills_UserProfileID",
                table: "HardSkills",
                newName: "IX_HardSkills_UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_HardSkills_UserProfiles_UserProfileId",
                table: "HardSkills",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SoftSkills_UserProfiles_UserProfileId",
                table: "SoftSkills",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HardSkills_UserProfiles_UserProfileId",
                table: "HardSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_SoftSkills_UserProfiles_UserProfileId",
                table: "SoftSkills");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "SoftSkills",
                newName: "UserProfileID");

            migrationBuilder.RenameIndex(
                name: "IX_SoftSkills_UserProfileId",
                table: "SoftSkills",
                newName: "IX_SoftSkills_UserProfileID");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "HardSkills",
                newName: "UserProfileID");

            migrationBuilder.RenameIndex(
                name: "IX_HardSkills_UserProfileId",
                table: "HardSkills",
                newName: "IX_HardSkills_UserProfileID");

            migrationBuilder.AddForeignKey(
                name: "FK_HardSkills_UserProfiles_UserProfileID",
                table: "HardSkills",
                column: "UserProfileID",
                principalTable: "UserProfiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SoftSkills_UserProfiles_UserProfileID",
                table: "SoftSkills",
                column: "UserProfileID",
                principalTable: "UserProfiles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
