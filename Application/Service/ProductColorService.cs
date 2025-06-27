using Application.Interface;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinceCenter.Repositories.Base;

namespace Application.Service
{
    namespace Application.Service
    {
        public class ProductColorService : IProductColorService
        {  
            private readonly GenericRepository<ProductColor> _repository;

            public ProductColorService(GenericRepository<ProductColor> repository)
            {
                _repository = repository;
            }

            public async Task<List<(Guid, string)>> GetAllAsync()
            {
                return await Task.FromResult(_repository.Query()
                    .Select(x => new ValueTuple<Guid, string>(x.ColorId, x.ColorName))
                    .ToList());
            }

            public async Task<string> GetNameByIdAsync(Guid id)
            {
                var entity = await _repository.GetByIdAsync(id);
                return entity?.ColorName;
            }

            public async Task<bool> CreateAsync(string colorName)
            {
                var entity = new ProductColor
                {
                    ColorId = Guid.NewGuid(),
                    ColorName = colorName
                };
                await _repository.AddAsync(entity);
                return true;
            }

            public async Task<bool> UpdateAsync(Guid id, string colorName)
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null) return false;

                entity.ColorName = colorName;
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

}
