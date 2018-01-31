
using ApplicationCore.Entities.ClientAgregate;
using ApplicationCore.Entities.ClientAgregate.ShipmentAgregate;
using ApplicationCore.Entities.Helpers;
using AutoMapper;
using DriveDrop.Bl.Models;
using DriveDrop.Bl.ViewModels;

namespace DriveDrop.Bl
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Shipment, ShipmentModel>();
            CreateMap<ShipmentModel, Shipment>();

            CreateMap<PackageStatusHistory, PackageStatusHistoryModel>();
            CreateMap<PackageStatusHistoryModel, PackageStatusHistory>();
            
            CreateMap<ShippingStatus, ShippingStatusModel>();
            CreateMap<ShippingStatusModel, ShippingStatus>();

            CreateMap<PriorityType , PriorityTypeModel>();
            CreateMap<PriorityTypeModel, PriorityType>();

            CreateMap<TransportType, TransportTypeModel>();
            CreateMap<TransportTypeModel, TransportType>();

            CreateMap<PackageSize, PackageSizeModel>();
            CreateMap<PackageSizeModel, PackageSize>();

            //CreateMap<Rating, RatingModel>();
            //CreateMap<RatingModel, Rating>();
             
            CreateMap<Review, ReviewModel>();
            CreateMap<ReviewModel, Review>();

            CreateMap<Address, AddressModel>();
            CreateMap<AddressModel, Address>();


            CreateMap<Customer, CustomerModel>();
            CreateMap<CustomerModel, Customer>();

            CreateMap<Tax, TaxModel>();
            CreateMap<TaxModel, Tax>();

            CreateMap<Rate, RateModel>();
            CreateMap<RateModel, Rate>();

            CreateMap<PackageSize, PackageSizeModel>();
            CreateMap<PackageSizeModel, PackageSize>();

            CreateMap<RateDetail, RateDetailModel>();
            CreateMap<RateDetailModel, RateDetail>();


            CreateMap<CurrentCustomerModel, DriverInfoModel>();
            CreateMap<DriverInfoModel, CurrentCustomerModel>();

            

        }
    }
}
