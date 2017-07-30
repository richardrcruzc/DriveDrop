using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveDrop.Api.Infrastructure
{

    public class DriveDropContext
         : DbContext
    {

        const string DEFAULT_SCHEMA = "shippings";

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<PriorityType> PriorityTypes { get; set; }
        public DbSet<ShippingStatus> ShippingStatuses { get; set; }
        public DbSet<TransportType> TransportTypes { get; set; }
        public DbSet<PackageSize> PackageSizes { get; set; }

        public DbSet<CustomerStatus> CustomerStatuses { get; set; }

        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ZipCodeState> ZipCodeStates { get; set; }

        public DbSet<Rate> Rates { get; set; }
        public DbSet<RateDetail> RateDetails { get; set; }
        public DbSet<RatePriority> RatePriorities { get; set; }
        public DbSet<RateTranportType> RateTranportTypes { get; set; }
        public DbSet<RatePackageSize> RatePackageSizes { get; set; }

        
        //public DbSet<ShipmentAddress> ShipmentAddresses { get; set; }
        //public DbSet<ShipmentCustomer> ShipmentCustomers { get; set; }


        public DriveDropContext(DbContextOptions<DriveDropContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PackageSize>(ConfigurePackageSize);
            modelBuilder.Entity<CardType>(ConfigureCardTypes);
            modelBuilder.Entity<AddressType>(ConfigureAddressType);
            modelBuilder.Entity<CustomerType>(ConfigureClientType);
            modelBuilder.Entity<PriorityType>(ConfigureDeliveryType);
            modelBuilder.Entity<ShippingStatus>(ConfigureShippingStatus);
            modelBuilder.Entity<TransportType>(ConfigureTransportType);
            modelBuilder.Entity<CustomerStatus>(ConfigureCustomerStatus);


            modelBuilder.Entity<Customer>(ConfigureCustomer);
            modelBuilder.Entity<Shipment>(ConfigureShipment);
            modelBuilder.Entity<Address>(ConfigureAddress);

            //modelBuilder.Entity<RateDetail>()
            // .HasOne(p => p.Rate)
            // .WithMany(b => b.RateDetails)
            // .HasForeignKey(p=>p.RateId)
            // .IsRequired();

        }
 

            void ConfigureAddress(EntityTypeBuilder<Address> addressConfiguration)
        {
            addressConfiguration.ToTable("address", DEFAULT_SCHEMA);

            // DDD Pattern comment: Implementing the Address Id as "Shadow property"
            // becuase the Address is a Value-Object (VO) and an Id (Identity) is not desired for a VO
            // EF Core just needs the Id so it is capable to store it in a database table
            // See: https://docs.microsoft.com/en-us/ef/core/modeling/shadow-properties 
            addressConfiguration.Property<int>("Id")
                .IsRequired();

            addressConfiguration.HasKey("Id");
        }
        void ConfigureCustomer(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("customer", DEFAULT_SCHEMA);
            builder.Property(ci => ci.Id)
               .ForSqlServerUseSequenceHiLo("customer_hilo")
               .IsRequired();
            
            builder.HasOne(ci => ci.CustomerType)
              .WithMany()
              .HasForeignKey(ci => ci.CustomerTypeId);


            builder.HasOne(ci => ci.TransportType)
          .WithMany()
          .HasForeignKey(ci => ci.TransportTypeId);


            builder.HasOne(ci => ci.CustomerStatus)
       .WithMany()
       .HasForeignKey(ci => ci.CustomerStatusId);

            builder.HasMany(b => b.Addresses)
            .WithOne();
        }

        void ConfigureShipment(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("shipment", DEFAULT_SCHEMA);

            builder.Property(ci => ci.Id)
               .IsRequired();


            builder.HasOne(ci => ci.Sender)
     .WithMany(f=>f.ShipmentSenders)
     .HasForeignKey(c=>c.DriverId)
     .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(ci => ci.Driver)
     .WithMany(ci => ci.ShipmentDrivers)  
     .HasForeignKey(d=>d.DriverId)
      .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(ci => ci.ShippingStatus)
     .WithMany()
     .HasForeignKey(ci => ci.ShippingStatusId);

            builder.HasOne(ci => ci.PriorityType)
      .WithMany()
      .HasForeignKey(ci => ci.PriorityTypeId);

            builder.HasOne(ci => ci.TransportType)
     .WithMany()
     .HasForeignKey(ci => ci.TransportTypeId);

        }

        void ConfigureCustomerStatus(EntityTypeBuilder<CustomerStatus> typeConfiguration)
        {
            typeConfiguration.ToTable("customerStatus", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureAddressType(EntityTypeBuilder<AddressType> typeConfiguration)
        {
            typeConfiguration.ToTable("addressType", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureClientType(EntityTypeBuilder<CustomerType> typeConfiguration)
        {
            typeConfiguration.ToTable("clientType", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }


        void ConfigureDeliveryType(EntityTypeBuilder<PriorityType> typeConfiguration)
        {
            typeConfiguration.ToTable("priorityType", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
        void ConfigureShippingStatus(EntityTypeBuilder<ShippingStatus> typeConfiguration)
        {
            typeConfiguration.ToTable("shippingStatus", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureTransportType(EntityTypeBuilder<TransportType> typeConfiguration)
        {
            typeConfiguration.ToTable("transportType", DEFAULT_SCHEMA);

            typeConfiguration.HasKey(o => o.Id);

            typeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            typeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        void ConfigureCardTypes(EntityTypeBuilder<CardType> cardTypesConfiguration)
        {
            cardTypesConfiguration.ToTable("cardtypes", DEFAULT_SCHEMA);

            cardTypesConfiguration.HasKey(ct => ct.Id);

            cardTypesConfiguration.Property(ct => ct.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            cardTypesConfiguration.Property(ct => ct.Name)
                .HasMaxLength(200)
                .IsRequired();
        }


        void ConfigurePackageSize(EntityTypeBuilder<PackageSize> config)
        {
            config.ToTable("packageSizes", DEFAULT_SCHEMA);

            config.HasKey(o => o.Id);

            config.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            config.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        //public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    // Dispatch Domain Events collection. 
        //    // Choices:
        //    // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        //    // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        //    // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        //    // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        //    await _mediator.DispatchDomainEventsAsync(this);


        //    // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        //    // performed throught the DbContext will be commited
        //    var result = await base.SaveChangesAsync();

        //    return true;
        //}
    }
}
