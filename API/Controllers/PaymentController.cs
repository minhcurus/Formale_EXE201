using Application;
using Application.DTO;
using Application.Interface;
using Application.Validation;
using Domain.Enum;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("getpayment")]
        public async Task<ActionResult<PaymentDTO>> GetAll()
        {
            var gt = await _paymentService.GetAllPayment();
            return Ok(gt);
        }

        [HttpPost("getpayment-byuser")]
        public async Task<ActionResult<PaymentDTO>> GetByUserId(int id)
        {
            var gt = await _paymentService.GetPaymentByUserId(id);
            return Ok(gt);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PaymentDTO dto)
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

        [HttpPost("search-payment")]
        public async Task<IActionResult> SearchPayment([FromBody] SearchTransactionDTO dto)
        {
            var result = await _paymentService.SearchPayment(dto);
            return Ok(result);

        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentDTO request)
        {
            var result = await _paymentService.CancelPayment(request.OrderCode, request.Reason);
            return Ok(result);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(long orderCode, Status status)
        {
            var result = await _paymentService.UpdatePaymentStatus(orderCode, status);
            return Ok(result);
        }

    }
}