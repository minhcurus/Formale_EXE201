using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class PremiumService : IPremiumService
    {
        private readonly IPaymentService _payment;
        private readonly PremiumRepository _premiumRepository;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly CurrentUserService _currentUser;

        public PremiumService(IPaymentService payment, PremiumRepository premiumRepository, IOrderService orderService, IMapper mapper, IUserService userService,CurrentUserService currentUserService)
        { 
            _payment = payment;
            _premiumRepository = premiumRepository;
            _orderService = orderService;
            _mapper = mapper;
            _userService = userService;
            _currentUser = currentUserService;
        }

        public async Task<ResultMessage> CreatePremiumOrderAndPayment(PremiumPackageTier tier)
        {
            var package = await _premiumRepository.GetTier(tier);
            if (package == null)
            {
                return new ResultMessage { 
                    Success = false,
                    Message = "Invalid package tier", 
                    Data = null,
                };
            }

            var order = new OrderDTO
            {
                UserId = _currentUser.UserId,
                TotalPrice = package.Price,
                Status = Status.PENDING,
            };


            var createOrderResult = await _orderService.Create(order);

            if (!createOrderResult.Success || createOrderResult.Data is not OrderDTO createdOrder)
            {
                return new ResultMessage {
                    Success = false, 
                    Message = "Không tạo được order",
                    Data = null,
                };
            }

            var paymentDto = new PaymentRequestDTO
            {
                UserId = _currentUser.UserId,
                Amount = package.Price,
                Description = $"Thanh toán gói {package.Tier}",
                BuyerName = _currentUser.FullName,
                BuyerEmail = _currentUser.Email,
                BuyerPhone = _currentUser.PhoneNumber,
                BuyerAddress = _currentUser.Address,
                ReturnUrl = "https://pokemon.com/payment/success",
                Method = PaymentMethod.PayOs,
                OrderId = createdOrder.OrderId,
            };

            var paymentResult = await _payment.CreatePayment(paymentDto);

            if (!paymentResult.Success)
                return new ResultMessage {
                    Success = false, 
                    Message = "Không tạo được payment" 
                };

            return new ResultMessage
            {
                Success = true,
                Message = "Order và Payment được tạo",
                Data = paymentResult.Data
            };
        }

        public async Task<List<PremiunPackageDTO>> GetPremiumPackages()
        {
            var get = await _premiumRepository.GetAll();
            return _mapper.Map<List<PremiunPackageDTO>>(get);
        }

        public async Task<ResultMessage> GetPremiumPackagesById(int premiumPackageId)
        {
            var get = await _premiumRepository.GetPremiumId(premiumPackageId);
            var map = _mapper.Map<PremiunPackageDTO>(get);
            if (get == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy gói này",
                    Data = null,
                };
            }

            return new ResultMessage
            {
                Success = true,
                Message = $"Gói {get.Tier}",
                Data = map,
            };
        }

        public async Task<ResultMessage> UpdatePremiumPackages(PremiunPackageDTO premiumPackage)
        {
            var get = await _premiumRepository.GetPremiumId(premiumPackage.Id);
            if (get == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Không tìm thấy gói này",
                    Data = null,
                };
            }

            get.Name = premiumPackage.Name;
            get.Description = premiumPackage.Description;
            get.Price = premiumPackage.Price;
            get.DurationInDays = premiumPackage.DurationInDays;
            get.Tier = premiumPackage.Tier;

            var result = await _premiumRepository.UpdateAsync(get);

            return new ResultMessage
            {
                Success = result > 0 ? true : false,
                Message = result > 0 ? "Cập nhật thành công" : "Cập nhật thất bại.",
                Data = result
            };

        }

        public async Task<UserResponse> UpdateUserPremiumAsync(int userId, int premiumPackageId)
        {
            var user = await _userService.GetUsersById(userId);
            if (user == null)
            {
                return null;
            }

            var premiumPackage = await _premiumRepository.GetPremiumId(premiumPackageId);
            if (premiumPackage == null)
            {
                return null;
            }

            user.PremiumPackageId = premiumPackage.Id;
            user.PremiumExpiryDate = DateTime.UtcNow.AddDays(premiumPackage.DurationInDays);

            var userDto = _mapper.Map<UserDTO>(user);
            var result = await _userService.UpdateProfile(userDto);

            return user;

        }

    }
}
