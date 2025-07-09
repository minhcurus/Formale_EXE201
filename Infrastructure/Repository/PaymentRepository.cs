using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
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
            TimeZoneInfo vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            if (payment == null)
                return 0;

            if (payment.Status == Status.COMPLETE)
                return 1;

            payment.Status = newStatus;

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == payment.OrderId);

            if (newStatus == Status.COMPLETE)
            {
                payment.PaidAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnZone);

                if (order != null)
                {
                    if (order.PaidAt == null)
                    {
                        order.PaidAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnZone);
                    }
                    order.Status = Status.COMPLETE;
                }
            }
            else if (newStatus == Status.CANCELLED)
            {
                if (payment.CancelledAt == null)
                {
                    payment.CancelledAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnZone);
                    payment.CancelReason = "hủy thanh toán";
                }

                if (order != null)
                {
                    order.Status = Status.CANCELLED;
                }
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

