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

namespace Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repository;
        private readonly CartRepository _cartService;
        private readonly CurrentUserService _currentUser;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public OrderService(OrderRepository repository, IMapper mapper, CartRepository cartService, CurrentUserService currentUserService,IPaymentService paymentService, IProductService productService) 
        { 
            _repository = repository;
            _mapper = mapper;
            _cartService = cartService;
            _currentUser = currentUserService;
            _paymentService = paymentService;
            _productService = productService;
        }

        public async Task<ResultMessage> Create(OrderDTO orderDTO)
        {
            var orderEntity = _mapper.Map<Order>(orderDTO);

            var createdOrderId = await _repository.CreateAsync(orderEntity);

            var createdOrder = await _repository.GetOrderId(createdOrderId);

            var createdOrderDto = _mapper.Map<OrderDTO>(createdOrder);

            return new ResultMessage
            {
                Success = true,
                Message = "Order created successfully",
                Data = createdOrderDto
            };
        }


        public async Task<List<OrderDTO>> GetAll()
        {
            var get = await _repository.GetAll();
            return _mapper.Map<List<OrderDTO>>(get);
        }

        public async Task<ResultMessage> GetOrderId(int id)
        {
            var get = await _repository.GetOrderId(id);
            var map = _mapper.Map<OrderDTO>(get);
            if (get == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = $"Không tìm thấy đơn hàng có Id : {id}",
                    Data = null,
                };
            }

            return new ResultMessage
            {
                Success = true,
                Message = "Tìm thấy đơn hàng",
                Data = map,
            };
        }

        public async Task<ResultMessage> CreateOrderFromCart()
        {
            var cart = await _cartService.GetCartByUserId(_currentUser.UserId.Value);
            if (cart == null || cart.Items.Count == 0)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Giỏ hàng trống",
                    Data = null
                };
            }
            TimeZoneInfo vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var order = new Order
            {
                UserId = _currentUser.UserId.Value,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnZone),
                Status = Status.PENDING,
                OrderItems = new List<OrderItem>(),
                TotalPrice = 0
            };

            foreach (var item in cart.Items)
            {
                var find = await _productService.GetByIdAsync(item.ProductId);
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = find.Name,
                    Quantity = item.Quantity,
                    Price = find.Price,
                    Description = find.Description,
                    ImageURL = find.ImageURL,
                };

                order.OrderItems.Add(orderItem);
                order.TotalPrice += item.Product.Price * item.Quantity;
            }

            var orderId = await _repository.CreateAsync(order);

            // Xóa giỏ hàng
            cart.Items.Clear();
            await _cartService.SaveChangesAsync();

            // Lấy lại đơn hàng vừa tạo
            var createdOrder = await _repository.GetOrderId(orderId);

            // Gọi luôn PaymentService để tạo thanh toán
            var paymentRequest = new PaymentRequestDTO
            {
                OrderId = createdOrder.OrderId,
                Amount = createdOrder.TotalPrice,
                Description = "Thanh toán đơn hàng",
                ReturnUrl = "myapp://payos/success",
                CancelUrl = "myapp://payos/cancel"
            };

            try
            {
                var paymentResult = await _paymentService.CreatePayment(paymentRequest);
                if (!paymentResult.Success)
                {
                    return new ResultMessage
                    {
                        Success = false,
                        Message = "Tạo đơn hàng thành công nhưng lỗi khi tạo thanh toán",
                        Data = new
                        {
                            OrderId = createdOrder.OrderId
                        }
                    };
                }

                return new ResultMessage
                {
                    Success = true,
                    Message = "Đặt hàng và tạo thanh toán thành công",
                    Data = new
                    {
                        OrderId = createdOrder.OrderId,
                        PaymentUrl = paymentResult.Data
                    }
                };
            }
            catch (Exception ex)
            {
                // Nếu lỗi trong lúc gọi PayOS
                return new ResultMessage
                {
                    Success = false,
                    Message = $"Tạo đơn hàng thành công nhưng lỗi khi gọi cổng thanh toán: {ex.Message}",
                    Data = new
                    {
                        OrderId = createdOrder.OrderId
                    }
                };
            }
        }


    }

}
