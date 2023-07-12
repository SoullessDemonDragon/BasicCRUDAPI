using AutoMapper;
using MyFirstWebApi.Dto;
using MyFirstWebApi.Models;

namespace MyFirstWebApi.Helper
{
    public class HelperProfile:Profile
    {
        public HelperProfile()
        {
            CreateMap<Dog,DogDto>();
            CreateMap<DogDto,Dog>();
            CreateMap<Category,CategoryDto>();
            CreateMap<CategoryDto,Category>();
            CreateMap<Country,CountryDto>();
            CreateMap<CountryDto,Country>();
            CreateMap<Owner,OwnerDto>();
            CreateMap<OwnerDto,Owner>();
            CreateMap<Review,ReviewDto>();
            CreateMap<ReviewDto,Review>();
            CreateMap<Reviewer,ReviewerDto>();
            CreateMap<ReviewerDto,Reviewer>();
        }
    }
}
