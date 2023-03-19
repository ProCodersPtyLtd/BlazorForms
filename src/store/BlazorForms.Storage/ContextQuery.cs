using BlazorForms.Shared.Extensions;
using BlazorForms.Storage.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlazorForms.Storage
{
    public class ContextQuery<T> : IDisposable, IContextQuery
        where T : class
    {
        //internal readonly DbContext _context;

        protected readonly List<Expression<Func<T, bool>>> _predicates = new();
        protected readonly List<string> _includes = new();

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
            _includes.Add(e.Body.ToString());
            return this;
        }

        public ContextQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            _predicates.Add(predicate);
            return this;
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            var query = ApplyPredicates(Query);
            return query.FirstOrDefault();
        }

        public async Task<List<T>> ToListAsync()
        {
            var query = ApplyPredicates(Query);
            return query.ToList();
        }

        public async Task<List<T>> ToListAsync(QueryOptions options)
        {
            var query = ApplyPredicates(Query);
            return query.ToList();
        }

        private IQueryable<T> ApplyPredicates(IQueryable<T> query)
        {
            var result = query;

            foreach(var predicate in _predicates)
            {
                result = result.Where(predicate);
            }

            return result;
        }
    }
}
