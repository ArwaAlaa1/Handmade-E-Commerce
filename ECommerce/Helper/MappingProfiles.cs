
using AutoMapper;
using ECommerce.Core.Models.Order;
using ECommerce.DTOs;
using ECommerce.DTOs.OrderDtos;
using System.Reflection;
using System.Runtime.Serialization;

namespace ECommerce.Helper
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderReturnDto>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
           .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd HH:mm"))) 
           .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.GetTotal()))
           .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems.Count)).ReverseMap();

            CreateMap<Order, OneOrderReturnDto>()
                  //.ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src =>
                  //    src.OrderDate.ToString("dddd, dd MMMM yyyy", new CultureInfo("ar"))))
                  .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd HH:mm")))
                  .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                      src.Status.GetType().GetMember(src.Status.ToString())
                          .FirstOrDefault()
                          .GetCustomAttribute<EnumMemberAttribute>().Value))
                  .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.GetTotal()))
                  .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                  .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => src.shippingCost.Cost))
                  .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.OrderItems.Count()))
                 .ForPath(dest => dest.ShippingAddress.FullName, opt => opt.MapFrom(src => src.ShippingAddress.FullName))
                         .ForPath(dest => dest.ShippingAddress.PhoneNumber, opt => opt.MapFrom(src => src.ShippingAddress.PhoneNumber))
                 .ForPath(dest => dest.ShippingAddress.City, opt => opt.MapFrom(src => src.ShippingAddress.City))
                  .ForPath(dest => dest.ShippingAddress.Country, opt => opt.MapFrom(src => src.ShippingAddress.Country))
                  .ForPath(dest => dest.ShippingAddress.Region, opt => opt.MapFrom(src => src.ShippingAddress.Region))
                  .ForPath(dest => dest.ShippingAddress.AddressDetails, opt => opt.MapFrom(src => src.ShippingAddress.AddressDetails))
                  .ReverseMap();


            CreateMap<OrderItem, OneItemInOrderReturnDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest=>dest.ItemStatus,opt=>opt.MapFrom(src => src.OrderItemStatus.GetType().GetMember(src.OrderItemStatus.ToString())
                          .FirstOrDefault()
                          .GetCustomAttribute<EnumMemberAttribute>().Value))
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Product.Seller.DisplayName))
                
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Product.ProductPhotos.FirstOrDefault().PhotoLink)).ReverseMap();

        
            
            CreateMap<ShippingCost,ShippingCostDto>()
                .ForMember(dest=>dest.Id,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.DeliveryTime, opt => opt.MapFrom(src => src.DeliveryTime))
                  .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ReverseMap();
        }
    }
}
