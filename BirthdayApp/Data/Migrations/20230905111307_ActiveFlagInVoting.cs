using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BirthdayApp.Data.Migrations
{
    public partial class ActiveFlagInVoting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Voting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Voting");
        }
    }
}
