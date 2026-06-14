using LoncotesLibrary.Models;
using LoncotesLibrary.Models.DTOs;
using AutoMapper;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {

        CreateMap<Material, MaterialDTO>();
        CreateMap<MaterialDTO, Material>();
        CreateMap<MaterialType, MaterialTypeDTO>();
        CreateMap<MaterialTypeDTO, MaterialType>();
        CreateMap<MaterialType, MaterialDTO>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreDTO, Genre>();
        CreateMap<Patron, PatronDTO>();
        CreateMap<PatronDTO, Patron>();
        CreateMap<Patron, CheckoutPatronDTO>();
        CreateMap<Checkout, CheckoutDTO>();
        CreateMap<CheckoutDTO, Checkout>();
        CreateMap<Checkout, CheckoutWithLateFeeDTO>();
        CreateMap<CheckoutWithLateFeeDTO, Checkout>();
    }
}