using Mango.Services.OrderAPI.Model;

namespace Mango.Services.OrderAPI.Respository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task updateOrderPaymentStatus(int orderHeaderId, bool status);

    }
}
