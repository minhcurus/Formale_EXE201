using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository(AppDBContext context) : base(context)
        {
        }

        public async Task<List<Order>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) 
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderId(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(e => e.OrderId == id);
        }

        public async Task<int> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); 
            return order.OrderId; 
        }
    }
}
