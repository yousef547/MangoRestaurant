using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public class CouponServices : BaseService, ICouponServices
    {
        private readonly IHttpClientFactory _clientFactory;
        public CouponServices(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CuopnAPIBase + $"api/cuopon/{couponCode}",
                AccessToken = token
            });
        }
    }
}
