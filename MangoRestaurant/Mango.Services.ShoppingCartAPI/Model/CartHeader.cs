﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Model
{
    public class CartHeader
    {
        [Key]
        public int CardHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }


    }
}
