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
    public class ProductMaterialService : IProductMaterialService
    {
        private readonly GenericRepository<ProductMaterial> _repository;

        public ProductMaterialService(GenericRepository<ProductMaterial> repository)
        {
            _repository = repository;
        }

        public async Task<List<(Guid, string)>> GetAllAsync()
        {
            return await Task.FromResult(_repository.Query()
                .Select(x => new ValueTuple<Guid, string>(x.MaterialId, x.MaterialName))
                .ToList());
        }

        public async Task<string> GetNameByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.MaterialName;
        }

        public async Task<bool> CreateAsync(string materialName)
        {
            var entity = new ProductMaterial
            {
                MaterialId = Guid.NewGuid(),
                MaterialName = materialName
            };
            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, string materialName)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.MaterialName = materialName;
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
