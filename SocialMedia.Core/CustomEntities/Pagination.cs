using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class Pagination
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public Pagination() { }
        public Pagination(PageList<object>list)
        {
            TotalCount= list.Count;
            PageSize= list.PageSize;
            CurrentPage= list.CurrentPage;
            TotalPages= list.TotalPages;
            HasNextPage= list.HasNextPage;
            HasPreviousPage= list.HasPreviousPage;
        }
    }
}
