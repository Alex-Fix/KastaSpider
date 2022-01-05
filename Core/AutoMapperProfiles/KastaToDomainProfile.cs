using AutoMapper;
using Core.Domain;
using Core.Models.Kasta;

namespace Core.AutoMapperProfiles
{
    public class KastaToDomainProfile : Profile
    {
        public KastaToDomainProfile()
        {
            AllowNullCollections = true;
            CreateMap<ProductModel, Product>();
            CreateMap<FeatureModel, Feature>();
        }
    }
}