using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DriveDrop.Web.Infrastructure;
using ApplicationCore.Entities.Helpers;

namespace DriveDrop.Web.Migrations
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

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Phone");

                    b.Property<string>("State");

                    b.Property<string>("Street");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

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

                    b.Property<int>("CustomerStatusId");

                    b.Property<int>("CustomerTypeId");

                    b.Property<int?>("DefaultAddressId");

                    b.Property<int?>("DeliverRadius");

                    b.Property<string>("DriverLincensePictureUri");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("IdentityGuid");

                    b.Property<string>("LastName");

                    b.Property<int?>("MaxPackage");

                    b.Property<string>("Phone");

                    b.Property<int?>("PickupRadius");

                    b.Property<int?>("TransportTypeId");

                    b.Property<string>("UserGuid");

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

                    b.Property<decimal>("Amount");

                    b.Property<string>("DeliveredPictureUri");

                    b.Property<int?>("DeliveryAddressId");

                    b.Property<decimal>("Discount");

                    b.Property<int?>("DriverId");

                    b.Property<string>("IdentityCode");

                    b.Property<string>("Note");

                    b.Property<int?>("PickupAddressId");

                    b.Property<string>("PickupPictureUri");

                    b.Property<int>("PriorityTypeId");

                    b.Property<int>("PriorityTypeLevel");

                    b.Property<string>("PromoCode");

                    b.Property<int>("SenderId");

                    b.Property<DateTime>("ShippingCreateDate");

                    b.Property<int>("ShippingStatusId");

                    b.Property<DateTime>("ShippingUpdateDate");

                    b.Property<decimal>("Tax");

                    b.Property<int>("TransportTypeId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryAddressId");

                    b.HasIndex("DriverId");

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

            modelBuilder.Entity("ApplicationCore.Entities.Helpers.HelperTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HelperType");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("HelperTable");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.Customer", b =>
                {
                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerStatus", "CustomerStatus")
                        .WithMany()
                        .HasForeignKey("CustomerStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.CustomerType", "CustomerType")
                        .WithMany()
                        .HasForeignKey("CustomerTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.Address", "DefaultAddress")
                        .WithMany()
                        .HasForeignKey("DefaultAddressId");

                    b.HasOne("ApplicationCore.Entities.ClientAgregate.ShipmentAgregate.TransportType", "TransportType")
                        .WithMany()
                        .HasForeignKey("TransportTypeId");
                });

            modelBuilder.Entity("ApplicationCore.Entities.ClientAgregate.PaymentMethod", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Helpers.HelperTable", "CardType")
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
        }
    }
}
