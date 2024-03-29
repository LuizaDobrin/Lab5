﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskAgendaProj.Migrations
{
    public partial class AddOwnerForTaskAndComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Comment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OwnerId",
                table: "Comment",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_OwnerId",
                table: "Comment",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_OwnerId",
                table: "Tasks",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_OwnerId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_OwnerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_OwnerId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Comment_OwnerId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Comment");
        }
    }
}
