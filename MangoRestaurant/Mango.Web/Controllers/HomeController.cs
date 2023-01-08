using Mango.Web.Model;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductServices _productServices;
        private readonly ICartServices _cartServices;



        public HomeController(ILogger<HomeController> logger, IProductServices productServices,ICartServices cartServices)
        {
            _cartServices = cartServices;
            _logger = logger;
            _productServices = productServices;
        }

        public async Task<IActionResult> Index()
        {

            List<ProductDto> list = new();
            var respons = await _productServices.GetAllProductsAsync<ResponseDto>("");
            if (respons.IsSuccess && respons != null)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(respons.Result));
            }
            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var respons = await _productServices.GetProductByIdAsync<ResponseDto>(productId, "");
            if (respons.IsSuccess && respons != null)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(respons.Result));
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };
            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.Id
            };
            var respons = await _productServices.GetProductByIdAsync<ResponseDto>(productDto.Id, "");
            if (respons != null&& respons.IsSuccess){
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(respons.Result));
            }
            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetails);
            cartDto.CartDetails = cartDetailsDtos;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartRespons = await _cartServices.AddToCartAsync<ResponseDto>(cartDto, accessToken);
            if (addToCartRespons != null && addToCartRespons.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}