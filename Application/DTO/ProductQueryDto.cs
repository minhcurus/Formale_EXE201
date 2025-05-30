using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class ProductQueryDto
    {
        public string? Keyword { get; init; }
        public Guid? BrandId { get; init; }
        public Guid? ColorId { get; init; }
        public Guid? StyleId { get; init; }
        public Guid? CategoryId { get; init; }
        public Guid? MaterialId { get; init; }
        public Guid? TypeId { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 12;
    }
}
