using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.ViewModels
{
    public class PaginatedListViewModel<T>:List<T>
    {
        // huidige pagina
        public int PageIndex { get; private set; }
        // aantal pagina's
        public int TotalPages { get; private set; }
        // aantal items per pagina
        public int PageSize { get; private set; }
        // aantal items in totaal
        public int TotalResults { get; private set; }

        public PaginatedListViewModel(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalResults = count;

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedListViewModel<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedListViewModel<T>(items, count, pageIndex, pageSize);
        }
    }
}
