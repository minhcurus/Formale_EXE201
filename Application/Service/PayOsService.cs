using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.DTO;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Application.Service
{
    public class PayOsService
    {
        private readonly HttpClient _httpClient;
        private readonly PayOsSetting _settings;
        private readonly CurrentUserService _currentUserService;

        public PayOsService(HttpClient httpClient, IOptions<PayOsSetting> settings, CurrentUserService currentUserService)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);
            _currentUserService = currentUserService;
        }

        public async Task<PayOsDTO> CreatePaymentAsync(PaymentRequestDTO dto)
        {
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var amount = (long)dto.Amount;

            // Dùng đúng 5 trường để tạo signature
            var signPayload = new Dictionary<string, object>
{
    { "amount", amount },
    { "orderCode", orderCode },
    { "description", dto.Description },
    { "returnUrl", dto.ReturnUrl },
    { "cancelUrl", dto.ReturnUrl }
};

            var signature = new PayOsHelper(_settings.ChecksumKey).GenerateSignature(signPayload);

            // thêm vào payload gửi cho PayOS
            var fullPayload = new Dictionary<string, object>(signPayload)
{
    { "buyerName", _currentUserService.FullName },
    { "buyerEmail", _currentUserService.Email },
    { "buyerPhone", _currentUserService.PhoneNumber },
    { "buyerAddress", _currentUserService.Address },
    { "signature", signature }
};

            var json = JsonSerializer.Serialize(fullPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v2/payment-requests", content);

            var body = await response.Content.ReadAsStringAsync();
            var payOsResponse = JsonSerializer.Deserialize<PayOsResponse>(body);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayOS error: {body}");
            }

            return new PayOsDTO
            {
                CheckoutUrl = payOsResponse.Data?.CheckoutUrl,
                PaymentLinkId = payOsResponse.Data?.PaymentLinkId ?? "",
                OrderCode = payOsResponse.Data.OrderCode,
                Signature = payOsResponse.Signature
            };
        }

        public async Task<bool> CancelPaymentAsync(long orderCode, string cancelReason)
        {
            var payload = new Dictionary<string, object>
    {
        { "cancellationReason", cancelReason }
    };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/v2/payment-requests/{orderCode}/cancel", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to cancel payment: {body}");
            }

            return true;
        }


    }

}
