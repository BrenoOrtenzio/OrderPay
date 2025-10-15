using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderMessageExceptionFieldsToRetrySuccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RetrySuccess",
                table: "SendOrderMessageExceptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetrySuccess",
                table: "SendOrderMessageExceptions");
        }
    }
}
