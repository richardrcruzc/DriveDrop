using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Web.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shippings");

            migrationBuilder.CreateSequence(
                name: "customer_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "address",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cardtypes",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardtypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "addressType",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addressType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customerStatus",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clientType",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "priorityType",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priorityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "shippingStatus",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shippingStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "transportType",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transportType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelperTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HelperType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelperTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CustomerStatusId = table.Column<int>(nullable: false),
                    CustomerTypeId = table.Column<int>(nullable: false),
                    DeliverRadius = table.Column<int>(nullable: true),
                    DriverLincensePictureUri = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    IdentityGuid = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    MaxPackage = table.Column<int>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PickupRadius = table.Column<int>(nullable: true),
                    TransportTypeId = table.Column<int>(nullable: true),
                    UserGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_customerStatus_CustomerStatusId",
                        column: x => x.CustomerStatusId,
                        principalSchema: "shippings",
                        principalTable: "customerStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_clientType_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalSchema: "shippings",
                        principalTable: "clientType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_transportType_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalSchema: "shippings",
                        principalTable: "transportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_HelperTable_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "HelperTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shipment",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    DeliveredPictureUri = table.Column<string>(nullable: true),
                    DeliveryAddressId = table.Column<int>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    DriverId = table.Column<int>(nullable: true),
                    IdentityCode = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    PickupAddressId = table.Column<int>(nullable: true),
                    PickupPictureUri = table.Column<string>(nullable: true),
                    PriorityTypeId = table.Column<int>(nullable: false),
                    PriorityTypeLevel = table.Column<int>(nullable: false),
                    PromoCode = table.Column<string>(nullable: true),
                    SenderId = table.Column<int>(nullable: false),
                    ShippingCreateDate = table.Column<DateTime>(nullable: false),
                    ShippingStatusId = table.Column<int>(nullable: false),
                    ShippingUpdateDate = table.Column<DateTime>(nullable: false),
                    Tax = table.Column<decimal>(nullable: false),
                    TransportTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_shipment_address_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalSchema: "shippings",
                        principalTable: "address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shipment_customer_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shipment_address_PickupAddressId",
                        column: x => x.PickupAddressId,
                        principalSchema: "shippings",
                        principalTable: "address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shipment_priorityType_PriorityTypeId",
                        column: x => x.PriorityTypeId,
                        principalSchema: "shippings",
                        principalTable: "priorityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shipment_customer_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shipment_shippingStatus_ShippingStatusId",
                        column: x => x.ShippingStatusId,
                        principalSchema: "shippings",
                        principalTable: "shippingStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shipment_transportType_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalSchema: "shippings",
                        principalTable: "transportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_CustomerStatusId",
                schema: "shippings",
                table: "customer",
                column: "CustomerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_CustomerTypeId",
                schema: "shippings",
                table: "customer",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_TransportTypeId",
                schema: "shippings",
                table: "customer",
                column: "TransportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CardTypeId",
                table: "Payments",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_DeliveryAddressId",
                schema: "shippings",
                table: "shipment",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_DriverId",
                schema: "shippings",
                table: "shipment",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_PickupAddressId",
                schema: "shippings",
                table: "shipment",
                column: "PickupAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_PriorityTypeId",
                schema: "shippings",
                table: "shipment",
                column: "PriorityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_SenderId",
                schema: "shippings",
                table: "shipment",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_ShippingStatusId",
                schema: "shippings",
                table: "shipment",
                column: "ShippingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_shipment_TransportTypeId",
                schema: "shippings",
                table: "shipment",
                column: "TransportTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cardtypes",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "addressType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "shipment",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "HelperTable");

            migrationBuilder.DropTable(
                name: "address",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "customer",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "priorityType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "shippingStatus",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "customerStatus",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "clientType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "transportType",
                schema: "shippings");

            migrationBuilder.DropSequence(
                name: "customer_hilo");
        }
    }
}
