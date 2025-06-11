using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRS_TravelDesk.Migrations
{
    /// <inheritdoc />
    public partial class updatedtravelrequestentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_TravelRequests_TravelRequestId",
                table: "Document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Document");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_Document_TravelRequestId",
                table: "Documents",
                newName: "IX_Documents_TravelRequestId");

            migrationBuilder.AddColumn<string>(
                name: "AadharCardNumber",
                table: "TravelRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DaysOfStay",
                table: "TravelRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealPreference",
                table: "TravelRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealRequired",
                table: "TravelRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "TravelRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TravelDate",
                table: "TravelRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Documents",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_TravelRequests_TravelRequestId",
                table: "Documents",
                column: "TravelRequestId",
                principalTable: "TravelRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_TravelRequests_TravelRequestId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "AadharCardNumber",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "DaysOfStay",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "MealPreference",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "MealRequired",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "TravelDate",
                table: "TravelRequests");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_TravelRequestId",
                table: "Document",
                newName: "IX_Document_TravelRequestId");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Document",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_TravelRequests_TravelRequestId",
                table: "Document",
                column: "TravelRequestId",
                principalTable: "TravelRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
