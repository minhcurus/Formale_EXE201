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
    public class ProductStyleService : IProductStyleService
    {
        private readonly GenericRepository<ProductStyle> _repository;

        public ProductStyleService(GenericRepository<ProductStyle> repository)
        {
            _repository = repository;
        }

        public async Task<List<(Guid, string)>> GetAllAsync()
        {
            return await Task.FromResult(_repository.Query()
                .Select(x => new ValueTuple<Guid, string>(x.StyleId, x.StyleName))
                .ToList());
        }

        public async Task<string> GetNameByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.StyleName;
        }

        public async Task<bool> CreateAsync(string styleName)
        {
            var entity = new ProductStyle
            {
                StyleId = Guid.NewGuid(),
                StyleName = styleName
            };
            await _repository.AddAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, string styleName)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.StyleName = styleName;
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
