using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities;
using VaccinceCenter.Repositories.Base;

namespace Application.Service
{
    public class ProductBrandService : IProductBrandService
    {
        private readonly GenericRepository<ProductBrand> _repository;

        public ProductBrandService(GenericRepository<ProductBrand> repository)
        {
            _repository = repository;
        }

        public async Task<List<(Guid, string)>> GetAllAsync()
        {
            return await Task.FromResult(_repository.Query()
                .Select(x => new ValueTuple<Guid, string>(x.BrandId, x.BrandName))
                .ToList());
        }

        public async Task<string> GetNameByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.BrandName;
        }

        public async Task<bool> CreateAsync(string brandName)
        {
            var entity = new ProductBrand
            {
                BrandId = Guid.NewGuid(),
                BrandName = brandName
            };
            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, string brandName)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.BrandName = brandName;
            await _repository.UpdateAsync(entity);
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
