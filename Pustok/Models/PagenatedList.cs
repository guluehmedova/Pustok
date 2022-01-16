using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class PagenatedList<T>:List<T>
    {
        public PagenatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.AddRange(items);
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling((double)count / pageIndex);
        }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrev { get => PageIndex > 1; }
        public bool HasNext { get => TotalPages > PageIndex; }
        public static PagenatedList<T> Create(IQueryable<T> query, int pageIndex, int pageSize)
        {
            return new PagenatedList<T>(query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), query.Count(), pageIndex, pageSize);
        }
    }
}
