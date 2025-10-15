using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderMessageExceptionFieldsToRetryQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "SendOrderMessageExceptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Queue",
                table: "SendOrderMessageExceptions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "SendOrderMessageExceptions");

            migrationBuilder.DropColumn(
                name: "Queue",
                table: "SendOrderMessageExceptions");
        }
    }
}
