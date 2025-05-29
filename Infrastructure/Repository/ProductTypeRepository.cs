using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class ProductTypeRepository : GenericRepository<ProductType>
    {
        public ProductTypeRepository(AppDBContext context) : base(context)
        {
        }
    }
}
