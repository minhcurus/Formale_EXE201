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

        public async Task<Payment> GetByTransactionId(string transactionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<Payment> GetByOrderCode(long orderCode)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

    }
}

