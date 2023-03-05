using BlazorForms.Shared;
using BlazorForms.Storage.Interfaces;
using BlazorForms.Storage.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Storage.InMemory
{
    public class InMemoryHighStore : IHighStore
    {
        private readonly Dictionary<Type, Dictionary<int, object>> _store = new();

        public ContextQuery<T> GetQuery<T>() where T : class, IEntity
        {
            ContextQuery<T> ctx;
            var data = GetEntityCollection(typeof(T));

            if (typeof(IEntity).IsAssignableFrom(typeof(T)))
            {
                // also check config settings to make sure user wants to exclude Deleted
                ctx = new ContextQuery<T>(null, data.Values.Cast<T>().Where(x => (x as IEntity)?.Deleted == false).AsQueryable());
            }
            else
            {
                ctx = new ContextQuery<T>(null, data.Values.Cast<T>().AsQueryable());
            }

            return ctx;
        }

        public async Task<T> UpsertAsync<T>(T entity) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));

            if (IsEntity(entity))
            {
                var e = entity as IEntity;

                if (e.Id == 0)
                {
                    e.Id = data.Count() + 1;
                }
                    
                data[e.Id] = e.GetCopy();
                return entity;
            }

            throw new Exception("UpsertAsync only works with IEntity");
        }

        private bool IsEntity(object entity)
        {
            return typeof(IEntity).IsAssignableFrom(entity.GetType());
        }

        private Dictionary<int, object> GetEntityCollection(Type type)
        {
            if (!_store.ContainsKey(type))
            {
                _store[type] = new Dictionary<int, object>();
            }

            return _store[type];
        }

        public ContextQuery<T> GetByIdQuery<T>(int id) where T : class, IEntity
        {
            throw new NotImplementedException();
        }
    }
}
