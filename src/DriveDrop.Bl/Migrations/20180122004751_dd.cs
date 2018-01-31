using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DriveDrop.Bl.Migrations
{
    public partial class dd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shippings");

            migrationBuilder.CreateSequence(
                name: "customer_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    MultipleTime = table.Column<bool>(nullable: false),
                    Percentage = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueuedEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachedDownloadId = table.Column<int>(nullable: false),
                    AttachmentFileName = table.Column<string>(nullable: true),
                    AttachmentFilePath = table.Column<string>(nullable: true),
                    Bcc = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    CC = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    DontSendBeforeDateUtc = table.Column<DateTime>(nullable: true),
                    EmailAccountId = table.Column<int>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    FromName = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    PriorityId = table.Column<int>(nullable: false),
                    ReplyTo = table.Column<string>(nullable: true),
                    ReplyToName = table.Column<string>(nullable: true),
                    SentOnUtc = table.Column<DateTime>(nullable: true),
                    SentTries = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    ToName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEmails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RateDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Charge = table.Column<decimal>(nullable: false),
                    From = table.Column<decimal>(nullable: false),
                    MileOrLbs = table.Column<string>(nullable: true),
                    To = table.Column<decimal>(nullable: false),
                    WeightOrDistance = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Group = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false),
                    RateDefault = table.Column<bool>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZipCodeStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCodeStates", x => x.Id);
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
                name: "packageSizes",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_packageSizes", x => x.Id);
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
                        name: "FK_Payments_cardtypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalSchema: "shippings",
                        principalTable: "cardtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OverHead = table.Column<decimal>(nullable: false),
                    PackageSizeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_packageSizes_PackageSizeId",
                        column: x => x.PackageSizeId,
                        principalSchema: "shippings",
                        principalTable: "packageSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RatePriorities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Charge = table.Column<decimal>(nullable: false),
                    ChargePercentage = table.Column<bool>(nullable: false),
                    PriorityTypeId = table.Column<int>(nullable: false),
                    RateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatePriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatePriorities_priorityType_PriorityTypeId",
                        column: x => x.PriorityTypeId,
                        principalSchema: "shippings",
                        principalTable: "priorityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RatePriorities_Rates_RateId",
                        column: x => x.RateId,
                        principalTable: "Rates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(nullable: true),
                    ReviewQuestionId = table.Column<int>(nullable: true),
                    Values = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewDetails_ReviewQuestions_ReviewQuestionId",
                        column: x => x.ReviewQuestionId,
                        principalTable: "ReviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Commission = table.Column<decimal>(nullable: false),
                    CustomerStatusId = table.Column<int>(nullable: true),
                    CustomerTypeId = table.Column<int>(nullable: true),
                    DefaultAddressId = table.Column<int>(nullable: true),
                    DeliverRadius = table.Column<double>(nullable: true),
                    DriverLincensePictureUri = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    IdentityGuid = table.Column<string>(nullable: true),
                    InsurancePhotoUri = table.Column<string>(nullable: true),
                    Isdeleted = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    MaxPackage = table.Column<int>(nullable: true),
                    PaymentMethodId = table.Column<string>(nullable: true),
                    PersonalPhotoUri = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    PickupRadius = table.Column<double>(nullable: true),
                    PrimaryPhone = table.Column<string>(nullable: true),
                    TransportTypeId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    UserNameToImpersonate = table.Column<string>(nullable: true),
                    VehicleColor = table.Column<string>(nullable: true),
                    VehicleMake = table.Column<string>(nullable: true),
                    VehicleModel = table.Column<string>(nullable: true),
                    VehiclePhotoUri = table.Column<string>(nullable: true),
                    VehicleYear = table.Column<string>(nullable: true),
                    VerificationId = table.Column<string>(nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_clientType_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalSchema: "shippings",
                        principalTable: "clientType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_customer_transportType_TransportTypeId",
                        column: x => x.TransportTypeId,
                        principalSchema: "shippings",
                        principalTable: "transportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    CustomerId = table.Column<int>(nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    TypeAddress = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_address_customer_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "shippings",
                        principalTable: "customer",
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
                    AmountPay = table.Column<decimal>(nullable: false),
                    ChargeAmount = table.Column<decimal>(nullable: false),
                    DeliveredPictureUri = table.Column<string>(nullable: true),
                    DeliveryAddressId = table.Column<int>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    Distance = table.Column<double>(nullable: false),
                    DriverId = table.Column<int>(nullable: true),
                    DropComment = table.Column<string>(nullable: true),
                    DropPictureUri = table.Column<string>(nullable: true),
                    Dropby = table.Column<DateTime>(nullable: false),
                    ExtraCharge = table.Column<decimal>(nullable: false),
                    ExtraChargeNote = table.Column<string>(nullable: true),
                    IdentityCode = table.Column<string>(nullable: true),
                    NeedaVanOrPickup = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    PackageSizeId = table.Column<int>(nullable: true),
                    PaymentNotes = table.Column<string>(nullable: true),
                    PaymentReceipt = table.Column<string>(nullable: true),
                    PaymentReceived = table.Column<string>(nullable: true),
                    PaymentReceivedDate = table.Column<string>(nullable: true),
                    PickupAddressId = table.Column<int>(nullable: true),
                    PickupPictureUri = table.Column<string>(nullable: true),
                    PriorityTypeId = table.Column<int>(nullable: false),
                    PriorityTypeLevel = table.Column<int>(nullable: false),
                    PromoCode = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    SecurityCode = table.Column<string>(nullable: true),
                    SenderId = table.Column<int>(nullable: false),
                    ShippingCreateDate = table.Column<DateTime>(nullable: false),
                    ShippingPickupDate = table.Column<DateTime>(nullable: false),
                    ShippingStatusId = table.Column<int>(nullable: false),
                    ShippingUpdateDate = table.Column<DateTime>(nullable: false),
                    ShippingValue = table.Column<decimal>(nullable: false),
                    ShippingWeight = table.Column<decimal>(nullable: false),
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
                        name: "FK_shipment_packageSizes_PackageSizeId",
                        column: x => x.PackageSizeId,
                        principalSchema: "shippings",
                        principalTable: "packageSizes",
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

            migrationBuilder.CreateTable(
                name: "PackageStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DriverId = table.Column<int>(nullable: false),
                    ShipmentId = table.Column<int>(nullable: false),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "shippings",
                        principalTable: "shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_shippingStatus_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "shippings",
                        principalTable: "shippingStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DriverId = table.Column<int>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    Reviewed = table.Column<string>(nullable: true),
                    SenderId = table.Column<int>(nullable: true),
                    ShippingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_customer_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_customer_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_shipment_ShippingId",
                        column: x => x.ShippingId,
                        principalSchema: "shippings",
                        principalTable: "shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_ShipmentId",
                table: "PackageStatusHistories",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CardTypeId",
                table: "Payments",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePriorities_PriorityTypeId",
                table: "RatePriorities",
                column: "PriorityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePriorities_RateId",
                table: "RatePriorities",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_PackageSizeId",
                table: "Rates",
                column: "PackageSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetails_ReviewId",
                table: "ReviewDetails",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetails_ReviewQuestionId",
                table: "ReviewDetails",
                column: "ReviewQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DriverId",
                table: "Reviews",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SenderId",
                table: "Reviews",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ShippingId",
                table: "Reviews",
                column: "ShippingId");

            migrationBuilder.CreateIndex(
                name: "IX_address_CustomerId",
                schema: "shippings",
                table: "address",
                column: "CustomerId");

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
                name: "IX_customer_DefaultAddressId",
                schema: "shippings",
                table: "customer",
                column: "DefaultAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_customer_TransportTypeId",
                schema: "shippings",
                table: "customer",
                column: "TransportTypeId");

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
                name: "IX_shipment_PackageSizeId",
                schema: "shippings",
                table: "shipment",
                column: "PackageSizeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewDetails_Reviews_ReviewId",
                table: "ReviewDetails",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_address_DefaultAddressId",
                schema: "shippings",
                table: "customer",
                column: "DefaultAddressId",
                principalSchema: "shippings",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_address_customer_CustomerId",
                schema: "shippings",
                table: "address");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "PackageStatusHistories");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "QueuedEmails");

            migrationBuilder.DropTable(
                name: "RateDetails");

            migrationBuilder.DropTable(
                name: "RatePriorities");

            migrationBuilder.DropTable(
                name: "ReviewDetails");

            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropTable(
                name: "ZipCodeStates");

            migrationBuilder.DropTable(
                name: "addressType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "cardtypes",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ReviewQuestions");

            migrationBuilder.DropTable(
                name: "shipment",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "packageSizes",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "priorityType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "shippingStatus",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "customer",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "customerStatus",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "clientType",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "address",
                schema: "shippings");

            migrationBuilder.DropTable(
                name: "transportType",
                schema: "shippings");

            migrationBuilder.DropSequence(
                name: "customer_hilo");
        }
    }
}
