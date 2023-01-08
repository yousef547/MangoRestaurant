﻿using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CouponRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<CouponDto> GetCuoponByCode(string couponCode)
        {
            var cuopon = await _db.Cuopons.FirstOrDefaultAsync(x=>x.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(cuopon);
        }
    }
}
