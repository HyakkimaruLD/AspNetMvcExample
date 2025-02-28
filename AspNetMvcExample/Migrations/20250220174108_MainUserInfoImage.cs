using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class MainUserInfoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageFileUserInfo_ImageFiles_ImageFilesId",
                table: "ImageFileUserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageFileUserInfo_UserInfos_UserInfosId",
                table: "ImageFileUserInfo");

            migrationBuilder.RenameColumn(
                name: "UserInfosId",
                table: "ImageFileUserInfo",
                newName: "UserInfoId");

            migrationBuilder.RenameColumn(
                name: "ImageFilesId",
                table: "ImageFileUserInfo",
                newName: "ImageFileId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageFileUserInfo_UserInfosId",
                table: "ImageFileUserInfo",
                newName: "IX_ImageFileUserInfo_UserInfoId");

            migrationBuilder.AddColumn<int>(
                name: "MainImageFileId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_MainImageFileId",
                table: "UserInfos",
                column: "MainImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFileUserInfo_ImageFiles_ImageFileId",
                table: "ImageFileUserInfo",
                column: "ImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFileUserInfo_UserInfos_UserInfoId",
                table: "ImageFileUserInfo",
                column: "UserInfoId",
                principalTable: "UserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_ImageFiles_MainImageFileId",
                table: "UserInfos",
                column: "MainImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageFileUserInfo_ImageFiles_ImageFileId",
                table: "ImageFileUserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageFileUserInfo_UserInfos_UserInfoId",
                table: "ImageFileUserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_ImageFiles_MainImageFileId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_MainImageFileId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "MainImageFileId",
                table: "UserInfos");

            migrationBuilder.RenameColumn(
                name: "UserInfoId",
                table: "ImageFileUserInfo",
                newName: "UserInfosId");

            migrationBuilder.RenameColumn(
                name: "ImageFileId",
                table: "ImageFileUserInfo",
                newName: "ImageFilesId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageFileUserInfo_UserInfoId",
                table: "ImageFileUserInfo",
                newName: "IX_ImageFileUserInfo_UserInfosId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFileUserInfo_ImageFiles_ImageFilesId",
                table: "ImageFileUserInfo",
                column: "ImageFilesId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFileUserInfo_UserInfos_UserInfosId",
                table: "ImageFileUserInfo",
                column: "UserInfosId",
                principalTable: "UserInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
