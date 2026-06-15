using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class Fix_registrationDocument_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "RegistrationRequestDocuments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IssueDate",
                table: "RegistrationRequestDocuments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuePlace",
                table: "RegistrationRequestDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "RegistrationRequestDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "RegistrationRequestDocuments");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "RegistrationRequestDocuments");

            migrationBuilder.DropColumn(
                name: "IssuePlace",
                table: "RegistrationRequestDocuments");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "RegistrationRequestDocuments");
        }
    }
}
