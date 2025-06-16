using Application;
using Application.DTO;
using Application.Interface;
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

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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

    }
}