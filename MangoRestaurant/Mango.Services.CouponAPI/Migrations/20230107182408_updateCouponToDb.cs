using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.CouponAPI.Migrations
{
    public partial class updateCouponToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CuponCode",
                table: "Cuopons",
                newName: "CouponCode");

            migrationBuilder.RenameColumn(
                name: "CuponId",
                table: "Cuopons",
                newName: "CouponId");

            migrationBuilder.InsertData(
                table: "Cuopons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount" },
                values: new object[] { 1, "10OFF", 10.0 });

            migrationBuilder.InsertData(
                table: "Cuopons",
                columns: new[] { "CouponId", "CouponCode", "DiscountAmount" },
                values: new object[] { 2, "20OFF", 20.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cuopons",
                keyColumn: "CouponId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cuopons",
                keyColumn: "CouponId",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "CouponCode",
                table: "Cuopons",
                newName: "CuponCode");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "Cuopons",
                newName: "CuponId");
        }
    }
}
