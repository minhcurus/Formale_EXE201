using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly PayOsService _payOsService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUserService _currentUser;

        public PaymentService(PaymentRepository paymentRepo, PayOsService payOsService,IMapper mapper, IHttpContextAccessor httpContextAccessor,CurrentUserService currentUser)
        {
            _paymentRepo = paymentRepo;
            _payOsService = payOsService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _currentUser = currentUser;
        }


        public async Task<ResultMessage> CreatePayment(PaymentRequestDTO dto)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Người dùng chưa đăng nhập",
                    Data = null
                };
            }

            var payOsResponse = await _payOsService.CreatePaymentAsync(dto);

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                UserId = _currentUser.UserId,
                Amount = dto.Amount,
                Description = dto.Description,
                BuyerName = _currentUser.FullName,
                BuyerEmail = _currentUser.Email,
                BuyerPhone = _currentUser.PhoneNumber,
                BuyerAddress = _currentUser.Address,
                Method = (PaymentMethod)2,
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

        public async Task<List<PaymentDTO>> GetAllPayment()
        {
            var get = await _paymentRepo.GetAllAsync();
            return _mapper.Map<List<PaymentDTO>>(get);

        }

        public async Task<ResultMessage> GetPaymentByUser()
        {
            if (_currentUser.UserId == null) 
            { 
                return new ResultMessage 
                {
                    Success = false,
                    Message = "Khong the tim thay payment voi id nay",
                    Data = null
                };
            }

            var get = await _paymentRepo.GetByUserId(_currentUser.UserId.Value);

            return new ResultMessage
            {
                Success = true,
                Message = "Danh sách payment của người dùng",
                Data = get
            };
        }

        public async Task<ResultMessage> UpdatePaymentStatus(long orderCode, Status newStatus)
        {
            var payment = await _paymentRepo.GetByOrderCode(orderCode);
            if (payment == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy Payment với OrderCode này.",
                    Data = null
                };
            }

            var updated = await _paymentRepo.UpdateStatusAsync(orderCode, newStatus);
            return new ResultMessage
            {
                Success = updated > 0,
                Message = updated > 0 ? "Cập nhật trạng thái thành công" : "Cập nhật thất bại",
                Data = updated
            };
        }

    }

}
