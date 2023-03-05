using BlazorForms.Storage.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazorForms.Storage.Model
{
    public class ContextQuery<T> : IDisposable, IContextQuery
        where T : class
    {
        //internal readonly DbContext _context;

        public IQueryable<T> Query { get; set; }

        public ContextQuery(object context, IQueryable<T> query)
        {
            //_context = context;
            Query = query;
        }

        public void Dispose()
        {
            //_context?.Dispose();
        }

        //public ContextQuery<T> Include<E>()
        //{
        //    return this;
        //}

        public ContextQuery<T> Include<TProperty>(Expression<Func<T, TProperty>> e)
        {
            return this;
        }

        public async Task<List<T>> ToListAsync()
        {
            return Query.ToList();
        }
    }
}
