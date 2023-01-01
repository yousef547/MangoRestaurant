﻿using Mango.Web.Model;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productServices;
        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        public async Task<IActionResult> ProdctIndex()
        {
            List<ProductDto> List = new();
            var response = await _productServices.GetAllProductsAsync<ResponseDto>();
            if (response != null && response.IsSuccess)
            {
                List = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(List);
        }

        public async Task<IActionResult> ProdctCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdctCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productServices.CreateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProdctIndex));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProdctEdit(int ProductId)
        {

            var response = await _productServices.GetProductByIdAsync<ResponseDto>(ProductId);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdctEdit(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productServices.UpdateProductAsync<ResponseDto>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProdctIndex));
                }
            }
            return View(model);
        }


        public async Task<IActionResult> ProductDelete(int productId)
        {

            var response = await _productServices.GetProductByIdAsync<ResponseDto>(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productServices.DeleteProductAsync<ResponseDto>(model.Id);
                if (response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProdctIndex));
                }
            }
            return View(model);
        }
    }
}
