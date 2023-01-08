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
        protected ResponseDto _response;
        public CartAPIController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId); 
                _response.Result = cartDto;

            }catch (Exception ex)
            {
                _response.IsSuccess= false; 
                _response.ErrorMessages= new List<string> { ex.Message };
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
                bool isSuccess = await _cartRepository.ApplyCuopon(cartDto.CartHeader.UserId,cartDto.CartHeader.CouponCode);
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
        public async Task<object> RemoveCart([FromBody]int cartId)
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
    }
}
