using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Respository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.OrderHeaders.Add(orderHeader);
           await _db.SaveChangesAsync();
            return true;
        }

        public async Task updateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderId == orderHeaderId);
            if(orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }
        }
    }
}
