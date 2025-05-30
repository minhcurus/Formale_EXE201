using Application.DTO;
using Application.Interface;
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

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PaymentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.CreatePayment(dto);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("search-payment")]
        public async Task<IActionResult> SearchPayment([FromBody] SearchTransactionDTO dto)
        {
            var result = await _paymentService.SearchPayment(dto);
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentDTO request)
        {
            var result = await _paymentService.CancelPayment(request.OrderCode, request.Reason);
            return Ok(result);
        }

    }
}

