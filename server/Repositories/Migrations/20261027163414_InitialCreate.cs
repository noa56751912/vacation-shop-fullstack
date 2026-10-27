using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Categori__6DB38D6E545AA12F", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "RATING",
                columns: table => new
                {
                    RATING_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HOST = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    METHOD = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    PATH = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    REFERER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    USER_AGENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Record_Date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RATING", x => x.RATING_ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Last_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Is_Admin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__206D91703D18E153", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Product_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category_Id = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__9834FBBADE735A24", x => x.Product_Id);
                    table.ForeignKey(
                        name: "FK__Products__Catego__5FB337D6",
                        column: x => x.Category_Id,
                        principalTable: "Categories",
                        principalColumn: "Category_Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<int>(type: "int", nullable: true),
                    Order_Date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    Order_Sum = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__F1E4607B6DAC32CD", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK__Orders__Status__6B24EA82",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Image_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Id = table.Column<int>(type: "int", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Main = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Images__3CAB4D5902A5E525", x => x.Image_Id);
                    table.ForeignKey(
                        name: "FK__Images__Product___6754599E",
                        column: x => x.Product_Id,
                        principalTable: "Products",
                        principalColumn: "Product_Id");
                });

            migrationBuilder.CreateTable(
                name: "Product_Month_Config",
                columns: table => new
                {
                    Config_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Id = table.Column<int>(type: "int", nullable: false),
                    Month_Number = table.Column<int>(type: "int", nullable: true),
                    Is_Available = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    Special_Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product___622D047554263B54", x => x.Config_Id);
                    table.ForeignKey(
                        name: "FK__Product_M__Produ__6477ECF3",
                        column: x => x.Product_Id,
                        principalTable: "Products",
                        principalColumn: "Product_Id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Item",
                columns: table => new
                {
                    Order_Item_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Id = table.Column<int>(type: "int", nullable: true),
                    Product_Id = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Departure_Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Return_Date = table.Column<DateOnly>(type: "date", nullable: true),
                    Nights_Count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_It__484568217FD3CDED", x => x.Order_Item_Id);
                    table.ForeignKey(
                        name: "FK__Order_Ite__Order__6E01572D",
                        column: x => x.Order_Id,
                        principalTable: "Orders",
                        principalColumn: "Order_Id");
                    table.ForeignKey(
                        name: "FK__Order_Ite__Produ__6EF57B66",
                        column: x => x.Product_Id,
                        principalTable: "Products",
                        principalColumn: "Product_Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Category_Id", "Category_Name" },
                values: new object[,]
                {
                    { 1, "Ski" },
                    { 2, "City & Culture" },
                    { 3, "Nature & Landscapes" },
                    { 4, "Sun & Beach" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Product_Id", "Category_Id", "Description", "Is_Active", "Price", "Product_Name" },
                values: new object[,]
                {
                    { 1, 1, "A popular ski resort in the Pirin Mountains of Bulgaria, offering great winter sports and cultural heritage.", true, 450m, "Bansko Bulgaria" },
                    { 2, 1, "Skiing in the breathtaking views of the Dolomites.", true, 1000m, "Madonna di Campiglio Italy" },
                    { 3, 1, "A luxurious ski vacation in an exclusive atmosphere in the Alps.", true, 1650m, "St. Moritz Switzerland" },
                    { 4, 1, "A breathtaking Arctic city in Norway, famous for its stunning fjords and as a premier destination for viewing the Northern Lights.", true, 950m, "Tromsø Norway" },
                    { 5, 1, "Europe's highest ski resort, famous for its exceptional snow and vast alpine slopes in the French Alps.", true, 875m, "Val Thorens France" },
                    { 6, 2, "The city of baths and bridges, spectacular architecture on the Danube.", true, 400m, "Budapest Hungary" },
                    { 7, 2, "Experience the timeless elegance of London: a perfect blend of royal heritage and modern sophistication.", true, 1600m, "London England" },
                    { 8, 2, "New York City: the vibrant heart of the world, where iconic skylines meet endless energy and architectural wonder.", true, 1400m, "New York USA" },
                    { 9, 2, "A fairytale city of Gothic spires and cobblestone charm, where history comes to life on every corner.", true, 600m, "Prague Czech Republic" },
                    { 10, 2, "An eternal city where ancient majesty meets timeless culture in every breathtaking street.", true, 725m, "Rome Italy" },
                    { 11, 2, "A majestic capital of imperial grandeur, world-class music, and refined architectural beauty.", true, 600m, "Vienna Austria" },
                    { 12, 3, "A serene landscape of mist-covered mountains, towering evergreens, and timeless Alpine charm.", true, 550m, "Black Forest Germany" },
                    { 13, 3, "A majestic wilderness of turquoise lakes, snow-capped peaks, and breathtaking natural wonders.", true, 780m, "Canadian Rockies Canada" },
                    { 14, 3, "A majestic gateway between emerald lakes and soaring Alpine peaks, where natural beauty and adventure meet.", true, 925m, "Interlaken Switzerland" },
                    { 15, 3, "A serene tapestry of ancient temples, traditional gardens, and the timeless elegance of Japanese heritage.", true, 850m, "Kyoto Japan" },
                    { 16, 3, "A breathtaking paradise of cascading waterfalls, crystal-clear turquoise lakes, and lush emerald forests.", true, 450m, "Plitvice Lakes Croatia" },
                    { 17, 4, "A futuristic desert oasis of soaring skyscrapers, luxury, and architectural marvels in the heart of the Middle East.", true, 950m, "Dubai United Arab Emirates" },
                    { 18, 4, "A pristine paradise of crystalline turquoise waters, vibrant coral reefs, and serene white-sand islands.", true, 1450m, "Maldives" },
                    { 19, 4, "A coastal treasure of ancient heritage, azure Mediterranean waters, and sun-drenched landscapes.", true, 420m, "Paphos Cyprus" },
                    { 20, 4, "Whitewashed villages on volcanic cliffs above the deep-blue Aegean, famous for its sunsets.", true, 1100m, "Santorini Greece" }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Image_Id", "Is_Main", "Product_Id", "Url" },
                values: new object[,]
                {
                    { 1, true, 1, "images/Ski/Bansko Bulgaria/1.jpg" },
                    { 2, false, 1, "images/Ski/Bansko Bulgaria/2.jpg" },
                    { 3, false, 1, "images/Ski/Bansko Bulgaria/3.jpg" },
                    { 4, false, 1, "images/Ski/Bansko Bulgaria/4.jpg" },
                    { 5, true, 2, "images/Ski/Madonna di Campiglio Italy/1.jpg" },
                    { 6, false, 2, "images/Ski/Madonna di Campiglio Italy/2.jpg" },
                    { 7, false, 2, "images/Ski/Madonna di Campiglio Italy/3.jpg" },
                    { 8, false, 2, "images/Ski/Madonna di Campiglio Italy/4.jpg" },
                    { 9, true, 3, "images/Ski/St. Moritz Switzerland/1.jpg" },
                    { 10, false, 3, "images/Ski/St. Moritz Switzerland/2.jpg" },
                    { 11, false, 3, "images/Ski/St. Moritz Switzerland/3.jpg" },
                    { 12, false, 3, "images/Ski/St. Moritz Switzerland/4.jpg" },
                    { 13, true, 4, "images/Ski/Tromsø Norway/1.jpg" },
                    { 14, false, 4, "images/Ski/Tromsø Norway/2.jpg" },
                    { 15, false, 4, "images/Ski/Tromsø Norway/3.jpg" },
                    { 16, false, 4, "images/Ski/Tromsø Norway/4.jpg" },
                    { 17, true, 5, "images/Ski/Val Thorens France/1.jpg" },
                    { 18, false, 5, "images/Ski/Val Thorens France/2.jpg" },
                    { 19, false, 5, "images/Ski/Val Thorens France/3.jpg" },
                    { 20, false, 5, "images/Ski/Val Thorens France/4.jpg" },
                    { 21, true, 6, "images/City & Culture/Budapest Hungary/1.jpg" },
                    { 22, false, 6, "images/City & Culture/Budapest Hungary/2.jpg" },
                    { 23, false, 6, "images/City & Culture/Budapest Hungary/3.jpg" },
                    { 24, false, 6, "images/City & Culture/Budapest Hungary/4.jpg" },
                    { 25, true, 7, "images/City & Culture/London England/1.jpg" },
                    { 26, false, 7, "images/City & Culture/London England/2.jpg" },
                    { 27, false, 7, "images/City & Culture/London England/3.jpg" },
                    { 28, false, 7, "images/City & Culture/London England/4.jpg" },
                    { 29, true, 8, "images/City & Culture/New York USA/1.jpg" },
                    { 30, false, 8, "images/City & Culture/New York USA/2.jpg" },
                    { 31, false, 8, "images/City & Culture/New York USA/3.jpg" },
                    { 32, false, 8, "images/City & Culture/New York USA/4.jpg" },
                    { 33, true, 9, "images/City & Culture/Prague Czech Republic/1.jpg" },
                    { 34, false, 9, "images/City & Culture/Prague Czech Republic/2.jpg" },
                    { 35, false, 9, "images/City & Culture/Prague Czech Republic/3.jpg" },
                    { 36, false, 9, "images/City & Culture/Prague Czech Republic/4.jpg" },
                    { 37, true, 10, "images/City & Culture/Rome Italy/1.jpg" },
                    { 38, false, 10, "images/City & Culture/Rome Italy/2.jpg" },
                    { 39, false, 10, "images/City & Culture/Rome Italy/3.jpg" },
                    { 40, false, 10, "images/City & Culture/Rome Italy/4.jpg" },
                    { 41, true, 11, "images/City & Culture/Vienna Austria/1.jpg" },
                    { 42, false, 11, "images/City & Culture/Vienna Austria/2.jpg" },
                    { 43, false, 11, "images/City & Culture/Vienna Austria/3.jpg" },
                    { 44, false, 11, "images/City & Culture/Vienna Austria/4.jpg" },
                    { 45, true, 12, "images/Nature & Landscapes/Black Forest Germany/1.jpg" },
                    { 46, false, 12, "images/Nature & Landscapes/Black Forest Germany/2.jpg" },
                    { 47, false, 12, "images/Nature & Landscapes/Black Forest Germany/3.jpg" },
                    { 48, false, 12, "images/Nature & Landscapes/Black Forest Germany/4.jpg" },
                    { 49, true, 13, "images/Nature & Landscapes/Canadian Rockies Canada/1.jpg" },
                    { 50, false, 13, "images/Nature & Landscapes/Canadian Rockies Canada/2.jpg" },
                    { 51, false, 13, "images/Nature & Landscapes/Canadian Rockies Canada/3.jpg" },
                    { 52, false, 13, "images/Nature & Landscapes/Canadian Rockies Canada/4.jpg" },
                    { 53, true, 14, "images/Nature & Landscapes/Interlaken Switzerland/1.jpg" },
                    { 54, false, 14, "images/Nature & Landscapes/Interlaken Switzerland/2.jpg" },
                    { 55, false, 14, "images/Nature & Landscapes/Interlaken Switzerland/3.jpg" },
                    { 56, false, 14, "images/Nature & Landscapes/Interlaken Switzerland/4.jpg" },
                    { 57, true, 15, "images/Nature & Landscapes/Kyoto Japan/1.jpg" },
                    { 58, false, 15, "images/Nature & Landscapes/Kyoto Japan/2.jpg" },
                    { 59, false, 15, "images/Nature & Landscapes/Kyoto Japan/3.jpg" },
                    { 60, false, 15, "images/Nature & Landscapes/Kyoto Japan/4.jpg" },
                    { 61, true, 16, "images/Nature & Landscapes/Plitvice Lakes Croatia/1.jpg" },
                    { 62, false, 16, "images/Nature & Landscapes/Plitvice Lakes Croatia/2.jpg" },
                    { 63, false, 16, "images/Nature & Landscapes/Plitvice Lakes Croatia/3.jpg" },
                    { 64, false, 16, "images/Nature & Landscapes/Plitvice Lakes Croatia/4.jpg" },
                    { 65, true, 17, "images/Sun & Beach/Dubai United Arab Emirates/1.jpg" },
                    { 66, false, 17, "images/Sun & Beach/Dubai United Arab Emirates/2.jpg" },
                    { 67, false, 17, "images/Sun & Beach/Dubai United Arab Emirates/3.jpg" },
                    { 68, false, 17, "images/Sun & Beach/Dubai United Arab Emirates/4.jpg" },
                    { 69, true, 18, "images/Sun & Beach/Maldives/1.jpg" },
                    { 70, false, 18, "images/Sun & Beach/Maldives/2.jpg" },
                    { 71, false, 18, "images/Sun & Beach/Maldives/3.jpg" },
                    { 72, false, 18, "images/Sun & Beach/Maldives/4.jpg" },
                    { 73, true, 19, "images/Sun & Beach/Paphos Cyprus/1.jpg" },
                    { 74, false, 19, "images/Sun & Beach/Paphos Cyprus/2.jpg" },
                    { 75, false, 19, "images/Sun & Beach/Paphos Cyprus/3.jpg" },
                    { 76, false, 19, "images/Sun & Beach/Paphos Cyprus/4.jpg" },
                    { 77, true, 20, "images/Sun & Beach/Santorini Greece/1.jpg" },
                    { 78, false, 20, "images/Sun & Beach/Santorini Greece/2.jpg" },
                    { 79, false, 20, "images/Sun & Beach/Santorini Greece/3.jpg" },
                    { 80, false, 20, "images/Sun & Beach/Santorini Greece/4.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Product_Month_Config",
                columns: new[] { "Config_Id", "Is_Available", "Month_Number", "Product_Id", "Special_Price" },
                values: new object[,]
                {
                    { 1, true, 1, 1, 650m },
                    { 2, true, 2, 1, 650m },
                    { 3, false, 5, 1, null },
                    { 4, false, 6, 1, null },
                    { 5, false, 7, 1, null },
                    { 6, false, 8, 1, null },
                    { 7, false, 9, 1, null },
                    { 8, false, 10, 1, null },
                    { 9, true, 7, 6, 600m },
                    { 10, true, 8, 6, 600m },
                    { 11, true, 12, 6, 550m },
                    { 12, true, 6, 9, 1200m },
                    { 13, true, 7, 9, 1200m },
                    { 14, true, 8, 9, 1200m },
                    { 15, true, 12, 9, 1500m },
                    { 16, true, 4, 10, 1800m },
                    { 17, true, 5, 10, 1850m },
                    { 18, true, 6, 10, 1500m },
                    { 19, true, 7, 10, 1500m },
                    { 20, true, 8, 10, 1500m },
                    { 21, true, 9, 10, 1850m },
                    { 22, true, 10, 10, 1800m },
                    { 23, true, 6, 11, 1100m },
                    { 24, true, 7, 11, 1100m },
                    { 25, true, 8, 11, 1100m },
                    { 26, true, 12, 11, 1400m },
                    { 27, true, 1, 12, 1200m },
                    { 28, true, 2, 12, 1200m },
                    { 29, true, 6, 12, 1050m },
                    { 30, true, 7, 12, 1050m },
                    { 31, true, 8, 12, 1050m },
                    { 32, true, 12, 12, 1200m },
                    { 33, true, 1, 13, 1200m },
                    { 34, true, 2, 13, 1200m },
                    { 35, true, 7, 13, 1650m },
                    { 36, true, 8, 13, 1650m },
                    { 37, true, 3, 15, 1600m },
                    { 38, true, 4, 15, 1850m },
                    { 39, true, 11, 15, 1700m },
                    { 40, true, 6, 16, 850m },
                    { 41, true, 7, 16, 950m },
                    { 42, true, 8, 16, 950m },
                    { 43, true, 6, 19, 500m },
                    { 44, true, 7, 19, 800m },
                    { 45, true, 8, 19, 950m },
                    { 46, true, 9, 19, 800m },
                    { 47, true, 10, 19, 500m },
                    { 48, true, 7, 20, 1500m },
                    { 49, true, 8, 20, 1500m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_Product_Id",
                table: "Images",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Item_Order_Id",
                table: "Order_Item",
                column: "Order_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Item_Product_Id",
                table: "Order_Item",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_User_Id",
                table: "Orders",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Month_Config_Product_Id",
                table: "Product_Month_Config",
                column: "Product_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category_Id",
                table: "Products",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D105345DBA8F49",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Order_Item");

            migrationBuilder.DropTable(
                name: "Product_Month_Config");

            migrationBuilder.DropTable(
                name: "RATING");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
