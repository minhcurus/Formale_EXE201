using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class OutfitComboItemRepository : GenericRepository<OutfitComboItem>  
    {
        public OutfitComboItemRepository(AppDBContext context) : base(context)
        {
        }
    }
}
