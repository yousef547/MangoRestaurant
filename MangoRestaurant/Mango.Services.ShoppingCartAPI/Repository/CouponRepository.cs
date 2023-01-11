using Mango.Services.ShoppingCartAPI.Model.Dto;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient client;
        public CouponRepository(HttpClient client)
        {
            this.client = client;
        }
        public async Task<CouponDto> GetCoupon(string couponName)
        {
            var respons = await client.GetAsync($"/api/coupon/{couponName}");
            var apiContent = await respons.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}
