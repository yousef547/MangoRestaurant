using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Model;
using Mango.Services.ShoppingCartAPI.Model.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCuopon(string userId, string cuoponCode)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            cartFromDb.CouponCode = cuoponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cartHeaderFromDb != null)
            {
                _db.CartDetails.RemoveRange(_db.CartDetails.Where(c => c.CartHeaderId == cartHeaderFromDb.CardHeaderId));
                _db.CartHeaders.Remove(cartHeaderFromDb);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreatedUpdatedCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);
            var productIdDb = await _db.Products.FirstOrDefaultAsync(x => x.Id == cartDto.CartDetails.FirstOrDefault().ProductId);
            if (productIdDb == null)
            {
                _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }
            var CartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cart.CartHeader.UserId);
            if (CartHeaderFromDb == null)
            {
                _db.CartHeaders.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CardHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();

            }
            else
            {
                var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cart.CartDetails.FirstOrDefault().ProductId && x.CartHeaderId == CartHeaderFromDb.CardHeaderId);
                if (cartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = CartHeaderFromDb.CardHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId),
            };
            cart.CartDetails = _db.CartDetails.Where(c => c.CartHeaderId == cart.CartHeader.CardHeaderId).Include(x => x.Product);
            return _mapper.Map<CartDto>(cart);

        }

        public async Task<bool> RemoveCuopon(string userId)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
            cartFromDb.CouponCode = "";
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);
                int totleCountOfCartItem = _db.CartDetails.Where(w=>w.CartHeaderId==cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if(totleCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CardHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);

                }
                await _db.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
