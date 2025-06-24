using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class CartService : ICartService
    {
        private readonly CurrentUserService _currentUser;
        private readonly CartRepository _cartRepository;

        public CartService(CurrentUserService currentUserService,CartRepository cartRepository) 
        { 
            _currentUser = currentUserService;
            _cartRepository = cartRepository;
        }

        public async Task<ResultMessage> GetCurrentCart()
        {
            var cart = await _cartRepository.GetCartByUserId(_currentUser.UserId.Value);
            if (cart == null || !cart.Items.Any())
            {
                return new ResultMessage
                {
                    Success = true,
                    Message = "Giỏ hàng trống",
                    Data = new List<object>()
                };
            }

            var items = cart.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                ProductName = i.Product.Name,
                i.Quantity,
                i.Product.Price,
                i.Product.Description,
                i.Product.ImageURL,
            });

            return new ResultMessage
            {
                Success = true,
                Message = "Lấy giỏ hàng thành công",
                Data = items
            };
        }

        public async Task<ResultMessage> RemoveFromCart(int cartItemId)
        {
            var item = await _cartRepository.GetCartItemById(cartItemId, _currentUser.UserId.Value);
            if (item == null)
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Giỏ hàng trống",
                    Data = null
                };
            }

            await _cartRepository.RemoveCartItem(item);
            await _cartRepository.SaveChangesAsync();

            return new ResultMessage
            {
                Success = true,
                Message = "Xóa sản phẩm khỏi giỏ hàng thành công",
                Data = item
            };
        }

        public async Task<ResultMessage> PrepareOrder()
        {
            var cart = await _cartRepository.GetCartByUserId(_currentUser.UserId.Value);
            if (cart == null || !cart.Items.Any())
            {
                return new ResultMessage
                {
                    Success = false,
                    Message = "Giỏ hàng trống",
                    Data = null
                };
            }

            var total = cart.Items.Sum(i => i.Product.Price * i.Quantity);
            var items = cart.Items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Quantity,
                i.Product.Price,
                i.Product.Description,
                i.Product.ImageURL
            });

            return new ResultMessage
            {
                Success = true,
                Message = "Chuẩn bị đặt hàng",
                Data = new { Items = items, Total = total }
            };

        }

        public async Task<ResultMessage> AddToCart(Guid productId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserId(_currentUser.UserId.Value);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = _currentUser.UserId.Value,
                    Items = new List<CartItem>()
                };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId, 
                    Quantity = quantity,
                });
            }

            await _cartRepository.SaveChangesAsync();

            return new ResultMessage
            {
                Success = true,
                Message = "Đã thêm sản phẩm vào giỏ hàng",
                Data = existingItem
            };
        }

    }
}

