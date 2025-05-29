using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Repository;

namespace Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly PayOsService _payOsService;

        public PaymentService(PaymentRepository paymentRepo, PayOsService payOsService)
        {
            _paymentRepo = paymentRepo;
            _payOsService = payOsService;
        }

        public async Task<ResultMessage> CreatePayment(PaymentDTO dto)
        {
            var payOsResponse = await _payOsService.CreatePaymentAsync(dto);

            var payment = new Payment
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Description = dto.Description,
                BuyerName = dto.BuyerName,
                BuyerEmail = dto.BuyerEmail,
                BuyerPhone = dto.BuyerPhone,
                BuyerAddress = dto.BuyerAddress,
                Method = dto.Method,
                ReturnUrl = dto.ReturnUrl,
                TransactionId = payOsResponse.PaymentLinkId,
                CheckoutUrl = payOsResponse.CheckoutUrl, 
                Signature = payOsResponse.Signature,
                OrderCode = payOsResponse.OrderCode,
                Status = Status.PENDING,
                CreateAt = DateTime.UtcNow
            };

            await _paymentRepo.CreateAsync(payment);

            return new ResultMessage
            {
                Success = true,
                Message = "Payment created",
                Data = new { paymentId = payment.Id, transactionId = payment.TransactionId,OrderCode = payment.OrderCode, paymentUrl = payment.CheckoutUrl }
            };
        }

        public async Task<ResultMessage> SearchPayment(SearchTransactionDTO searchTransactionDTO)
        {
            var payment = await _paymentRepo.GetByTransactionId(searchTransactionDTO.TransactionId);
            if (payment == null)
                return new ResultMessage { Success = false, Message = "Payment not found" };

            return new ResultMessage { 
                Success = true, 
                Message = "Payment found",
                Data = new
                {
                    payment.Id,
                    payment.UserId,
                    payment.Amount,
                    payment.OrderCode,
                    payment.Description,
                    payment.BuyerName,
                    payment.BuyerEmail,
                    payment.BuyerPhone,
                    payment.BuyerAddress,
                    payment.ReturnUrl,
                    payment.CheckoutUrl,
                    payment.Status,
                    payment.CreateAt,
                    payment.PaidAt,
                    payment.TransactionId
                }
            };
        }

        public async Task<ResultMessage> CancelPayment(long orderCode, string reason)
        {
            var payment = await _paymentRepo.GetByOrderCode(orderCode);
            if (payment == null)
                return new ResultMessage { Success = false, Message = "Payment not found" };

            if (payment.Status != Status.PENDING)
                return new ResultMessage { Success = false, Message = "Only PENDING payments can be cancelled." };

            await _payOsService.CancelPaymentAsync(orderCode, reason);

            payment.Status = Status.CANCELLED;
            payment.CancelReason = reason;
            payment.CancelledAt = DateTime.UtcNow;

            await _paymentRepo.UpdateAsync(payment);

            return new ResultMessage { Success = true, Message = "Payment cancelled successfully" };
        }


    }

}
