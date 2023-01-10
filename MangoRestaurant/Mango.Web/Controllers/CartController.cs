using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {

        private readonly IProductServices _productServices;
        private readonly ICartServices _cartServices;
        private readonly ICouponServices _couponServices;
        public CartController( IProductServices productServices, ICartServices cartServices, ICouponServices couponServices)
        {
            _cartServices = cartServices;
            _productServices = productServices;
            _couponServices = couponServices;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
        }


        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedUser());
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var respons = await _cartServices.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken);
            if (respons != null && respons.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var respons = await _cartServices.ApplyCuopon<ResponseDto>(cartDto, accessToken);
            if (respons != null && respons.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var respons = await _cartServices.RemoveCuopon<ResponseDto>(cartDto.CartHeader.UserId, accessToken);
            if (respons != null && respons.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartServices.Checkout<ResponseDto>(cartDto.CartHeader, accessToken);
                if (!response.IsSuccess)
                {
                    TempData["Error"] = response.DisplayMassage;
                    return RedirectToAction(nameof(Checkout));
                }
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception e)
            {
                return View(cartDto);
            }
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }
        private async Task<CartDto> LoadCartDtoBasedOnLoggedUser()
        {
            var UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var respons = await _cartServices.GetCartByUserIdAsync<ResponseDto>(UserId, accessToken);
            CartDto cartDto = new();
            if(respons!=null && respons.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(respons.Result));
            }
            if (cartDto.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var coupon = await _couponServices.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);
                    if (coupon != null && coupon.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result));
                        cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;

                    }
                }
                foreach(var item in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotle += (item.Product.Price * item.Count);
                }
                cartDto.CartHeader.OrderTotle -= cartDto.CartHeader.DiscountTotal;
            }
            return cartDto;
        }


    }
}
