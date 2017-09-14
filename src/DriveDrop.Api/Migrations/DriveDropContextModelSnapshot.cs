using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DriveDrop.Api.Infrastructure;

namespace DriveDrop.Api.Migrations
{
    [DbContext(typeof(DriveDropContext))]
    partial class DriveDropContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:Sequence:.customer_hilo", "'customer_hilo', '', '1', '10', '', '', 'Int64', 'False'")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Contact");

                    b.Property<string>("Country");

                    b.Property<int?>("CustomerId");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Phone");

                    b.Property<string>("State");

                    b.Property<string>("Street");

                    b.Property<string>("TypeAddress");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("address","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.CardType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("cardtypes","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:HiLoSequenceName", "customer_hilo")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<decimal>("Commission");

                    b.Property<int?>("CustomerStatusId");

                    b.Property<int?>("CustomerTypeId");

                    b.Property<int?>("DefaultAddressId");

                    b.Property<double?>("DeliverRadius");

                    b.Property<string>("DriverLincensePictureUri");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("IdentityGuid");

                    b.Property<string>("InsurancePhotoUri");

                    b.Property<string>("LastName");

                    b.Property<int?>("MaxPackage");

                    b.Property<string>("PaymentMethodId");

                    b.Property<string>("PersonalPhotoUri");

                    b.Property<string>("Phone");

                    b.Property<double?>("PickupRadius");

                    b.Property<string>("PrimaryPhone");

                    b.Property<int?>("TransportTypeId");

                    b.Property<string>("UserName");

                    b.Property<string>("UserNameToImpersonate");

                    b.Property<string>("VehicleColor");

                    b.Property<string>("VehicleMake");

                    b.Property<string>("VehicleModel");

                    b.Property<string>("VehiclePhotoUri");

                    b.Property<string>("VehicleYear");

                    b.Property<string>("VerificationId");

                    b.HasKey("Id");

                    b.HasIndex("CustomerStatusId");

                    b.HasIndex("CustomerTypeId");

                    b.HasIndex("DefaultAddressId");

                    b.HasIndex("TransportTypeId");

                    b.ToTable("customer","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CardTypeId");

                    b.HasKey("Id");

                    b.HasIndex("CardTypeId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.AddressType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("addressType","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("customerStatus","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("clientType","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.PriorityType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("priorityType","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AmountPay");

                    b.Property<decimal>("ChargeAmount");

                    b.Property<string>("DeliveredPictureUri");

                    b.Property<int?>("DeliveryAddressId");

                    b.Property<decimal>("Discount");

                    b.Property<double>("Distance");

                    b.Property<int?>("DriverId");

                    b.Property<decimal>("ExtraCharge");

                    b.Property<string>("ExtraChargeNote");

                    b.Property<string>("IdentityCode");

                    b.Property<string>("Note");

                    b.Property<int?>("PackageSizeId");

                    b.Property<string>("PaymentNotes");

                    b.Property<string>("PaymentReceipt");

                    b.Property<string>("PaymentReceived");

                    b.Property<string>("PaymentReceivedDate");

                    b.Property<int?>("PickupAddressId");

                    b.Property<string>("PickupPictureUri");

                    b.Property<int>("PriorityTypeId");

                    b.Property<int>("PriorityTypeLevel");

                    b.Property<string>("PromoCode");

                    b.Property<int>("Quantity");

                    b.Property<int>("SenderId");

                    b.Property<DateTime>("ShippingCreateDate");

                    b.Property<int>("ShippingStatusId");

                    b.Property<DateTime>("ShippingUpdateDate");

                    b.Property<decimal>("ShippingValue");

                    b.Property<decimal>("ShippingWeight");

                    b.Property<decimal>("Tax");

                    b.Property<int>("TransportTypeId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryAddressId");

                    b.HasIndex("DriverId");

                    b.HasIndex("PackageSizeId");

                    b.HasIndex("PickupAddressId");

                    b.HasIndex("PriorityTypeId");

                    b.HasIndex("SenderId");

                    b.HasIndex("ShippingStatusId");

                    b.HasIndex("TransportTypeId");

                    b.ToTable("shipment","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.ShippingStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("shippingStatus","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.TransportType", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("transportType","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Coupon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<string>("Code");

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("MultipleTime");

                    b.Property<bool>("Percentage");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.PackageSize", b =>
                {
                    b.Property<int>("Id")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("packageSizes","shippings");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Rate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("OverHead");

                    b.Property<int?>("PackageSizeId");

                    b.HasKey("Id");

                    b.HasIndex("PackageSizeId");

                    b.ToTable("Rates");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.RateDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Charge");

                    b.Property<decimal>("From");

                    b.Property<string>("MileOrLbs");

                    b.Property<decimal>("To");

                    b.Property<string>("WeightOrDistance");

                    b.HasKey("Id");

                    b.ToTable("RateDetails");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.RatePriority", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Charge");

                    b.Property<bool>("ChargePercentage");

                    b.Property<int>("PriorityTypeId");

                    b.Property<int>("RateId");

                    b.HasKey("Id");

                    b.HasIndex("PriorityTypeId");

                    b.HasIndex("RateId");

                    b.ToTable("RatePriorities");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Comment");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int?>("DriverId");

                    b.Property<bool>("Published");

                    b.Property<string>("Reviewed");

                    b.Property<int?>("SenderId");

                    b.Property<int?>("ShippingId");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.HasIndex("SenderId");

                    b.HasIndex("ShippingId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.ReviewDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ReviewId");

                    b.Property<int?>("ReviewQuestionId");

                    b.Property<int>("Values");

                    b.HasKey("Id");

                    b.HasIndex("ReviewId");

                    b.HasIndex("ReviewQuestionId");

                    b.ToTable("ReviewDetails");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.ReviewQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Group");

                    b.HasKey("Id");

                    b.ToTable("ReviewQuestions");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Tax", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("County");

                    b.Property<decimal>("Rate");

                    b.Property<bool>("RateDefault");

                    b.Property<string>("State");

                    b.HasKey("Id");

                    b.ToTable("TaxRates");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.ZipCodeState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("County");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("State");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

                    b.ToTable("ZipCodeStates");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.Address", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Customer")
                        .WithMany("Addresses")
                        .HasForeignKey("CustomerId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.Customer", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerStatus", "CustomerStatus")
                        .WithMany()
                        .HasForeignKey("CustomerStatusId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerType", "CustomerType")
                        .WithMany()
                        .HasForeignKey("CustomerTypeId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Address", "DefaultAddress")
                        .WithMany()
                        .HasForeignKey("DefaultAddressId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.TransportType", "TransportType")
                        .WithMany()
                        .HasForeignKey("TransportTypeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.PaymentMethod", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.CardType", "CardType")
                        .WithMany()
                        .HasForeignKey("CardTypeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.Shipment", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Address", "DeliveryAddress")
                        .WithMany()
                        .HasForeignKey("DeliveryAddressId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Customer", "Driver")
                        .WithMany("ShipmentDrivers")
                        .HasForeignKey("DriverId");

                    b.HasOne("ApplicationCore.Entities.Helpers.PackageSize", "PackageSize")
                        .WithMany()
                        .HasForeignKey("PackageSizeId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Address", "PickupAddress")
                        .WithMany()
                        .HasForeignKey("PickupAddressId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.PriorityType", "PriorityType")
                        .WithMany()
                        .HasForeignKey("PriorityTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Customer", "Sender")
                        .WithMany("ShipmentSenders")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.ShippingStatus", "ShippingStatus")
                        .WithMany()
                        .HasForeignKey("ShippingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.TransportType", "TransportType")
                        .WithMany()
                        .HasForeignKey("TransportTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Rate", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Helpers.PackageSize", "PackageSize")
                        .WithMany()
                        .HasForeignKey("PackageSizeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.RatePriority", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.PriorityType", "PriorityType")
                        .WithMany()
                        .HasForeignKey("PriorityTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.Helpers.Rate", "Rate")
                        .WithMany("RatePriorities")
                        .HasForeignKey("RateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.Review", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Customer", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Customer", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.Shipment", "Shipping")
                        .WithMany("Reviews")
                        .HasForeignKey("ShippingId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.ReviewDetail", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Helpers.Review", "Review")
                        .WithMany("Details")
                        .HasForeignKey("ReviewId");

                    b.HasOne("ApplicationCore.Entities.Helpers.ReviewQuestion", "ReviewQuestion")
                        .WithMany()
                        .HasForeignKey("ReviewQuestionId");
                });
        }
    }
}
