using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;
using Domain.Enum;

namespace Application.Interface
{
    public interface IPaymentService
    {
        Task<ResultMessage> CreatePayment(PaymentRequestDTO dto);
        Task<ResultMessage> SearchPayment(SearchTransactionDTO searchTransactionDTO);
        Task<ResultMessage> UpdatePaymentStatus(long orderCode, Status newStatus);
        Task<List<PaymentAllDTO>> GetAllPayment();
        Task<ResultMessage> GetPaymentByUser();
        Task<ResultMessage> CancelPayment(long orderCode, string reason);
        Task<string> GetPaymentStatusAsync(long orderCode);
        Task<List<PaymentPackageResponse>> GetAllPremiumPayments();
        Task<Payment?> GetPaymentByOrderCode(long orderCode);

    }
}
