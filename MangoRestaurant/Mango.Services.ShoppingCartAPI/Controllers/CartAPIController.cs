using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Message;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartAPIController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;

        private readonly IMessageBus _messageBus;
        protected ResponseDto _response;
        public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository)
        {
            _messageBus = messageBus;
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
            _couponRepository = couponRepository;
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreatedUpdatedCart(cartDto);
                _response.Result = cart;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost("updateCart")]
        public async Task<object> updateCart(CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreatedUpdatedCart(cartDto);
                _response.Result = cart;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }


        [HttpPost("ApplyCuopon")]
        public async Task<object> ApplyCuopon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCuopon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = isSuccess;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost("RemoveCuopon")]
        public async Task<object> RemoveCuopon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCuopon(userId);
                _response.Result = isSuccess;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }


        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
            }
            return _response;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if(checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
                        _response.DisplayMassage = "Coupon Price has changed, please confirm";
                        return _response;
                    }
                }
                checkoutHeader.CartDetails = cartDto.CartDetails;
                await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");
                await _cartRepository.ClearCart(checkoutHeader.UserId);

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
