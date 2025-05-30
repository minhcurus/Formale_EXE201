using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public record PaginatedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalRecords,
    int TotalPages,
    int Page,
    int PageSize);
}
