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
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository(AppDBContext context) : base(context) { }

        public async Task<List<Payment>> GetAll()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<List<Payment>> GetByUserId(int userId)
        {
            return await _context.Payments
                                 .Where(e => e.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<Payment> GetByTransactionId(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<Payment> GetByOrderCode(long orderCode)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task<int> UpdateStatusAsync(long orderCode, Status newStatus)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
            if (payment == null) return 0;

            payment.Status = newStatus;
            if (newStatus == Status.COMPLETE)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<List<Payment>> GetAllPremiumPayments()
        {
            return await _context.Payments
                .Include(p => p.UserAccount)
                .Include(p => p.PremiumPackage)
                .Where(p => p.PremiumPackageId != null)
                .ToListAsync();
        }

    }
}

