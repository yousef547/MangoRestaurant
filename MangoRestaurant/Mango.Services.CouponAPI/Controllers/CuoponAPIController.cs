using Mango.Services.CouponAPI.Model.Dto;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/cuopon")]
    public class CuoponAPIController : Controller
    {
        private readonly ICouponRepository _cuoponRepository;
        protected ResponseDto _response;
        public CuoponAPIController(ICouponRepository cuoponRepository)
        {
            _cuoponRepository = cuoponRepository;
            this._response = new ResponseDto();
        }
        [HttpGet("{code}")]
        public async Task<object> getDiscountForCode(string code)
        {
            try
            {
                CouponDto couponDto = await _cuoponRepository.GetCuoponByCode(code);
                if (couponDto == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }
    }
}
