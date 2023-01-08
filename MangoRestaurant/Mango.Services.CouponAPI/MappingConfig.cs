using AutoMapper;
using Mango.Services.CouponAPI.Model;
using Mango.Services.CouponAPI.Model.Dto;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var MappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
         
            });
            return MappingConfig;
        }
    }
}
