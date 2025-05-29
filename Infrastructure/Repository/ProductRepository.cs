using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository(AppDBContext context) : base(context)
        {
        }
    }

}
