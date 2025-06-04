using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class PremiumRepository : GenericRepository<PremiumPackage>
    {
        public PremiumRepository(AppDBContext context) : base(context)
        {
        }

        public async Task<PremiumPackage> GetTier(PremiumPackageTier tier)
        {
            return await _context.PremiumPackages.FirstOrDefaultAsync( e => e.Tier == tier); 
        }

        public async Task<List<PremiumPackage>> GetAll()
        {
            return await _context.PremiumPackages.ToListAsync();
        }

        public async Task<PremiumPackage> GetPremiumId(int id)
        {
            return await _context.PremiumPackages.FirstOrDefaultAsync(e => e.Id == id);
        }


    }
}
