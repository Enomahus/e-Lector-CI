using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class Fix_AddTypeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SoumissionDate",
                table: "RegistrationRequests",
                newName: "SubmissionDate");

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "RegistrationRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "RegistrationRequests");

            migrationBuilder.RenameColumn(
                name: "SubmissionDate",
                table: "RegistrationRequests",
                newName: "SoumissionDate");
        }
    }
}
