using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface ICartService
    {
        Task<ResultMessage> GetCurrentCart();
        Task<ResultMessage> AddToCart(Guid productId, int quantity);
        Task<ResultMessage> ReduceQuantity(Guid productId, int quantity);
        Task<ResultMessage> RemoveFromCart(int cartItemId);
        Task<ResultMessage> PrepareOrder();

    }
}
