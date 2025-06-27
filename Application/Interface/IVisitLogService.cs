using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IVisitLogService
    {
        Task<int> GetTodayVisitCount();
        Task<object> GetAllVisitDaysWithCounts();
        Task<int> GetVisitCountByDate(DateTime date);
        Task<(int Month, int Count)> GetRegistrationsThisMonth();
    }
}
