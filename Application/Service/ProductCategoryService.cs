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
    public class ProductCategoryService :IProductCategoryService
    {
        private readonly ProductCategoryRepository _repository;

        public ProductCategoryService(ProductCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<(Guid, string)>> GetAllAsync()
        {
            return await Task.FromResult(_repository.Query()
                .Select(c => new ValueTuple<Guid, string>(c.CategoryId, c.CategoryName))
                .ToList());
        }

        public async Task<string> GetNameByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.CategoryName;
        }

        public async Task<bool> CreateAsync(string categoryName)
        {
            var entity = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = categoryName
            };
            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, string categoryName)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.CategoryName = categoryName;
            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            await _repository.RemoveAsync(entity);
            return true;
        }
    }
}
