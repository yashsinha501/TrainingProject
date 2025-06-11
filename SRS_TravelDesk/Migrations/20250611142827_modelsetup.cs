using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SRS_TravelDesk.Migrations
{
    /// <inheritdoc />
    public partial class modelsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_TravelRequest_TravelRequestId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_CommentedById",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_TravelRequest_TravelRequestId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_TravelRequest_Users_UserId",
                table: "TravelRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelRequest",
                table: "TravelRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentedByUserId",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "TravelRequest",
                newName: "TravelRequests");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_TravelRequest_UserId",
                table: "TravelRequests",
                newName: "IX_TravelRequests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_TravelRequestId",
                table: "Comments",
                newName: "IX_Comments_TravelRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CommentedById",
                table: "Comments",
                newName: "IX_Comments_CommentedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelRequests",
                table: "TravelRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "TravelHr" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_TravelRequests_TravelRequestId",
                table: "Comments",
                column: "TravelRequestId",
                principalTable: "TravelRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_CommentedById",
                table: "Comments",
                column: "CommentedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_TravelRequests_TravelRequestId",
                table: "Document",
                column: "TravelRequestId",
                principalTable: "TravelRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TravelRequests_Users_UserId",
                table: "TravelRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_TravelRequests_TravelRequestId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_CommentedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_TravelRequests_TravelRequestId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_TravelRequests_Users_UserId",
                table: "TravelRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TravelRequests",
                table: "TravelRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.RenameTable(
                name: "TravelRequests",
                newName: "TravelRequest");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_TravelRequests_UserId",
                table: "TravelRequest",
                newName: "IX_TravelRequest_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TravelRequestId",
                table: "Comment",
                newName: "IX_Comment_TravelRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CommentedById",
                table: "Comment",
                newName: "IX_Comment_CommentedById");

            migrationBuilder.AddColumn<int>(
                name: "CommentedByUserId",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TravelRequest",
                table: "TravelRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_TravelRequest_TravelRequestId",
                table: "Comment",
                column: "TravelRequestId",
                principalTable: "TravelRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_CommentedById",
                table: "Comment",
                column: "CommentedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_TravelRequest_TravelRequestId",
                table: "Document",
                column: "TravelRequestId",
                principalTable: "TravelRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TravelRequest_Users_UserId",
                table: "TravelRequest",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
