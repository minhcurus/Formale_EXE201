using Application.DTO;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinceCenter.Repositories.Base;

namespace Application.Service
{
    public class UserClosetService : IUserClosetService
    {
        private readonly GenericRepository<UserCloset> _repository;
        private readonly ProductStyleRepository _productStyleRepository;
        private readonly ProductCategoryRepository _productCategoryRepository;
        private readonly IOpenRouterService _ai;
        private readonly IMapper _mapper;

        public UserClosetService(GenericRepository<UserCloset> repository, IOpenRouterService ai, IMapper mapper,
           ProductStyleRepository productStyleRepository,
           ProductCategoryRepository productCategoryRepository)
        {
            _repository = repository;
            _productStyleRepository = productStyleRepository;
            _productCategoryRepository = productCategoryRepository;
            _ai = ai;
            _mapper = mapper;
        }

        public async Task<List<UserClosetDto>> GetAllAsync()
        {
            var data = _repository.Query()
                .Select(x => new UserClosetDto
                {
                    ClosetId = x.ClosetId,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    ComboId = x.ComboId
                }).ToList();

            return await Task.FromResult(data);
        }

        public async Task<UserClosetDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new UserClosetDto
            {
                ClosetId = entity.ClosetId,
                UserId = entity.UserId,
                ProductId = entity.ProductId,
                ComboId = entity.ComboId
            };
        }

        public async Task<List<UserClosetDto>> GetByUserIdAsync(int userId)
        {
            var data = _repository.Query()
                .Where(x => x.UserId == userId)
                .Select(x => new UserClosetDto
                {
                    ClosetId = x.ClosetId,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    ComboId = x.ComboId
                }).ToList();

            return await Task.FromResult(data);
        }


        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await _repository.RemoveAsync(entity);
            return true;
        }

        public async Task<List<UserClosetDto>> GetSingleItemsAsync(int userId)
        {
            var data = _repository.Query()
                .Where(x => x.UserId == userId && x.ProductId != null)
                .Select(x => new UserClosetDto
                {
                    ClosetId = x.ClosetId,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    ComboId = x.ComboId
                }).ToList();

            return await Task.FromResult(data);
        }

        public async Task<List<UserClosetDto>> GetComboItemsAsync(int userId)
        {
            var data = _repository.Query()
                .Where(x => x.UserId == userId && x.ComboId != null)
                .Select(x => new UserClosetDto
                {
                    ClosetId = x.ClosetId,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    ComboId = x.ComboId
                }).ToList();

            return await Task.FromResult(data);
        }


    }
}
