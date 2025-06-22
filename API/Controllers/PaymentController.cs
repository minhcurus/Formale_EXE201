using Application;
using Application.DTO;
using Application.Interface;
using Application.Service;
using Application.Validation;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPremiumService _premiumService;
        public PaymentController(IPaymentService paymentService, IPremiumService premiumService)
        {
            _paymentService = paymentService;
            _premiumService = premiumService;
        }

        [Authorize(Roles = "1")]
        [HttpGet("getpayment")]
        public async Task<ActionResult<PaymentDTO>> GetAll()
        {
            var gt = await _paymentService.GetAllPayment();
            return Ok(gt);
        }

        [Authorize]
        [HttpGet("getpayment-byuser")]
        public async Task<ActionResult<PaymentDTO>> GetByUserId()
        {
            var gt = await _paymentService.GetPaymentByUser();
            return Ok(gt);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PaymentRequestDTO dto)
        {
            var validator = new PaymentRequestValidator();
            var validatorResult = validator.Validate(dto);

            if (validatorResult.IsValid == false)
            {
                return BadRequest(new ResultMessage
                {
                    Success = false,
                    Message = "Missing value!",
                    Data = validatorResult.ToString()
                });
            }

            var result = await _paymentService.CreatePayment(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("search-payment")]
        public async Task<IActionResult> SearchPayment([FromBody] SearchTransactionDTO dto)
        {
            var result = await _paymentService.SearchPayment(dto);
            return Ok(result);

        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentDTO request)
        {
            var result = await _paymentService.CancelPayment(request.OrderCode, request.Reason);
            return Ok(result);
        }

        [Authorize(Roles = "1")]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(long orderCode, Status status)
        {
            var result = await _paymentService.UpdatePaymentStatus(orderCode, status);
            return Ok(result);
        }

        [HttpGet("check-payment-status")]
        public async Task<IActionResult> CheckStatus(long orderCode)
        {
            var status = await _paymentService.GetPaymentStatusAsync(orderCode);
            if (status == "PAID" || status == "SUCCESS")
            {
                await _paymentService.UpdatePaymentStatus(orderCode, Status.COMPLETE);
            }
            return Ok(new { orderCode, status });
        }

        [Authorize(Roles = "1")]
        [HttpGet("premium-payments")]
        public async Task<IActionResult> GetPremiumPayments()
        {
            var result = await _paymentService.GetAllPremiumPayments();
            return Ok(result);
        }

        [Authorize(Roles = "1")]
        [HttpPost("confirm-premium-payment")]
        public async Task<IActionResult> ConfirmPremiumPayment([FromBody] ConfirmPremiumPaymentRequest request)
        {
            // 1. Cập nhật trạng thái
            var updateStatus = await _paymentService.UpdatePaymentStatus(request.OrderCode, Status.COMPLETE);
            if (!updateStatus.Success)
                return BadRequest(updateStatus);

            // 2. Lấy thông tin payment
            var payment = await _paymentService.GetPaymentByOrderCode(request.OrderCode);
            if (payment == null || payment.UserId == null || payment.PremiumPackageId == null)
                return BadRequest("Không tìm thấy giao dịch phù hợp.");

            // 3. Gán gói Premium
            var result = await _premiumService.UpdateUserPremiumAsync(payment.UserId.Value, payment.PremiumPackageId.Value);
            return Ok(new
            {
                Message = "Xác nhận thanh toán thành công và gán Premium.",
                User = result
            });
        }


    }
}