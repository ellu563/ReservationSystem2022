using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationSystem2022.Migrations
{
    public partial class AddAccessCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "accessCount",
                table: "Items",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accessCount",
                table: "Items");
        }
    }
}
