using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Enum;

namespace Application.Interface
{
    public interface IPaymentService
    {
        Task<ResultMessage> CreatePayment(PaymentDTO dto);
        Task<ResultMessage> SearchPayment(SearchTransactionDTO searchTransactionDTO);
        Task<ResultMessage> UpdatePaymentStatus(long orderCode, Status newStatus);
        Task<List<PaymentDTO>> GetAllPayment();
        Task<ResultMessage> GetPaymentByUserId(int id);
        Task<ResultMessage> CancelPayment(long orderCode, string reason);
    }
}
