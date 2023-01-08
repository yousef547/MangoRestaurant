using Mango.Services.ShoppingCartAPI.Model.Dto;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);
        Task<CartDto> CreatedUpdatedCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ApplyCuopon(string userId,string cuoponCode);
        Task<bool> RemoveCuopon(string userId);
        Task<bool> ClearCart(string userId);


    }
}
