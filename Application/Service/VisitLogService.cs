using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using CloudinaryDotNet.Actions;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class VisitLogService : IVisitLogService
    {
        private readonly VisitLogRepository _repo;

        public VisitLogService(VisitLogRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> GetTodayVisitCount()
        {
            return await _repo.CountTodayVisits();
        }

        public async Task<object> GetAllVisitDaysWithCounts()
        {
            var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var logs = await _repo.GetAll();

            var grouped = logs
                .Select(log => new
                {
                    Date = log.AccessTime.Date
                })
                .GroupBy(x => x.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    VisitCount = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            var total = grouped.Sum(x => x.VisitCount);

            return new
            {
                Success = true,
                TotalVisits = total,
                VisitDays = grouped
            };
        }


        public async Task<int> GetVisitCountByDate(DateTime date)
        {
            var start = date.Date;              
            var end = start.AddDays(1);         

            var visitLogs = await _repo.GetAll();

            return visitLogs
                .Where(log => log.AccessTime >= start && log.AccessTime < end)
                .Count();
        }

        public async Task<(int Month, int Count)> GetRegistrationsThisMonth()
        {
            return await _repo.CountRegistrationsForMonth();
        }

    }
}
