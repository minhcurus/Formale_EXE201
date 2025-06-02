using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;

namespace Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repository;
        private readonly IMapper _mapper;
        public OrderService(OrderRepository repository, IMapper mapper) 
        { 
            _repository = repository;
            _mapper = mapper;
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
    }
   
}
