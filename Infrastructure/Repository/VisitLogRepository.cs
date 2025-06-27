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
    public class VisitLogRepository : GenericRepository<VisitLog>
    {
        private static readonly TimeZoneInfo VnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public VisitLogRepository(AppDBContext context) : base(context)
        {
        }

        public async Task<List<VisitLog>> GetAll()
        {
            return await _context.VisitLogs.ToListAsync();
        }

        public async Task<int> CountTodayVisits()
        {
            
            DateTime nowUtc = DateTime.UtcNow;
            DateTime nowVN = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, VnTimeZone);

            DateTime startOfDayVN = new DateTime(nowVN.Year, nowVN.Month, nowVN.Day, 0, 0, 0);

            return await _context.VisitLogs
                .Where(v => v.AccessTime >= startOfDayVN)
                .CountAsync();
        }

        public async Task<(int Month, int Count)> CountRegistrationsForMonth()
        {
            var nowVN = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VnTimeZone);
            var startOfMonthVN = new DateTime(nowVN.Year, nowVN.Month, 1, 0, 0, 0);
            var startOfNextMonthVN = startOfMonthVN.AddMonths(1);

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(startOfMonthVN, VnTimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(startOfNextMonthVN, VnTimeZone);

            var count = await _context.VisitLogs
                .Where(v => v.LogType == "Register" && v.AccessTime >= startUtc && v.AccessTime < endUtc)
                .CountAsync();
            return (nowVN.Month, count);
        }



    }
}

