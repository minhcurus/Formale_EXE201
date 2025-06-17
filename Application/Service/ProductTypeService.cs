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
    public class ProductTypeService : IProductTypeService
    {
        private readonly GenericRepository<ProductType> _repository;

        public ProductTypeService(GenericRepository<ProductType> repository)
        {
            _repository = repository;
        }

        public async Task<List<(Guid, string)>> GetAllAsync()
        {
            return await Task.FromResult(_repository.Query()
                .Select(x => new ValueTuple<Guid, string>(x.TypeId, x.TypeName))
                .ToList());
        }

        public async Task<string> GetNameByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.TypeName;
        }

        public async Task<bool> CreateAsync(string typeName)
        {
            var entity = new ProductType
            {
                TypeId = Guid.NewGuid(),
                TypeName = typeName
            };
            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, string typeName)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.TypeName = typeName;
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
