using AutoMapper;
using Mango.Services.ProductAPI.Model;

namespace Mango.Services.ProductAPI

{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var MappingConfig = new MapperConfiguration(config =>
              {
                  config.CreateMap<ProductDto, Product>();
                  config.CreateMap<Product, ProductDto>();

              });
            return MappingConfig;
        }
    }
}
