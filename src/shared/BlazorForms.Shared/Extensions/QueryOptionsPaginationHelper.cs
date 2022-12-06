using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BlazorForms.Shared.Extensions
{
    public static class QueryOptionsPaginationHelper
    {
        public static IQueryable<Q> GetPaginationResult<Q>(IQueryable<Q> query, QueryOptions queryOptions)
        {
            if (queryOptions.AllowPagination)
            {
                if (queryOptions.PageReturnTotalCount == -1)
                {
                    queryOptions.PageReturnTotalCount = query.Count();
                }

                var page = query
                    .Skip(queryOptions.PageIndex * queryOptions.PageSize)
                    .Take(queryOptions.PageSize);               
                return page;
            }
            else
            {
                return query;
            }
        }
    }
}
