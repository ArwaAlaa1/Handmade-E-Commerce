
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
           .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("yyyy-MM-dd HH:mm"))) // تنسيق التاريخ
           .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.SubTotal))
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
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Cost))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest=>dest.ItemStatus,opt=>opt.MapFrom(src => src.OrderItemStatus.GetType().GetMember(src.OrderItemStatus.ToString())
                          .FirstOrDefault()
                          .GetCustomAttribute<EnumMemberAttribute>().Value))
                .ForMember(dest => dest.PriceAfterSale, opt => opt.MapFrom(src => src.Product.DiscountedPrice))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Product.ProductPhotos.FirstOrDefault())).ReverseMap();

          //  CreateMap<OrderItem, OrderItemReturnDto>()
          //     .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Id))
          //      .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src =>
          //            src.ProductItem.ItemType.GetType().GetMember(src.ProductItem.ItemType.ToString())
          //                .FirstOrDefault()
          //                .GetCustomAttribute<EnumMemberAttribute>().Value))
          //       .ForMember(dest => dest.OrderItemStatus, opt => opt.MapFrom(src =>
          //            src.OrderItemStatus.GetType().GetMember(src.OrderItemStatus.ToString())
          //                .FirstOrDefault()
          //                .GetCustomAttribute<EnumMemberAttribute>().Value))
          //       .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductItem.ProductId))
          //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductItem.Name))
          //     .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductItem.Price))
          //     .ForMember(dest => dest.PriceAfterSale, opt => opt.MapFrom(src => src.ProductItem.PriceAfterSale))
          //     .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.ProductItem.Photo))
          //      .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
          //      .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.ProductItem.Color))
          //.ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ProductItem.Size))
          //.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.ProductItem.Text))
          //.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ProductItem.Date))
          //.ForMember(dest => dest.FilePdf, opt => opt.MapFrom(src => src.ProductItem.FilePdf)).ReverseMap();


        }
    }
}
