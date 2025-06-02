using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;
using Domain.Enum;

namespace Application.Interface
{
    public interface IPremiumService
    {
        Task<List<PremiunPackageDTO>> GetPremiumPackages();
        Task<ResultMessage> GetPremiumPackagesById(int premiumPackageId);
        Task<ResultMessage> UpdatePremiumPackages(PremiunPackageDTO premiumPackage);
        Task<ResultMessage> CreatePremiumOrderAndPayment(int userId, PremiumPackageTier tier);
        Task<UserResponse> UpdateUserPremiumAsync(int userId, int premiumPackageId);

    }
}
