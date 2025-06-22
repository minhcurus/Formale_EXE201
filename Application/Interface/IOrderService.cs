using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAll();
        Task<ResultMessage> GetOrderId(int id);
        Task<ResultMessage> Create(OrderDTO orderDTO);
        Task<ResultMessage> CreateOrderFromCart();

    }
}
