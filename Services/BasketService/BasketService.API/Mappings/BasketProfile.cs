using AutoMapper;
using BasketService.API.DTOs;
using BasketService.Domain.Entities;

namespace BasketService.API.Mappings;

public class BasketProfile : Profile
{
    public BasketProfile()
    {
        CreateMap<Basket, BasketDto>().ReverseMap();
        CreateMap<BasketItem, BasketItemDto>().ReverseMap();
    }
}