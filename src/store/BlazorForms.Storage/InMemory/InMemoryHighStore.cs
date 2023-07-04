using BlazorForms.Shared;
using BlazorForms.Storage.Interfaces;
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

        public async Task<List<T>> GetAllAsync<T>() where T : class, IEntity
        {
            return await GetQuery<T>().ToListAsync();
        }

        public ContextQuery<T> GetQuery<T>() where T : class, IEntity
        {
            ContextQuery<T> ctx;
            var data = GetEntityCollection(typeof(T));

            // also check config settings to make sure user wants to exclude Deleted
            ctx = new ContextQuery<T>(null, data.Values.Cast<T>().Where(x => x?.Deleted == false).AsQueryable());
            
            //if (typeof(IEntity).IsAssignableFrom(typeof(T)))
            //{
            //}
            //else
            //{
            //    ctx = new ContextQuery<T>(null, data.Values.Cast<T>().AsQueryable());
            //}

            return ctx;
        }

        public async Task<T> UpsertAsync<T>(T entity) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));

            //if (IsEntity(entity))
            {
                //var e = entity as IEntity;

                if (entity.Id == 0)
                {
                    entity.Id = data.Count() + 1;
                }
                    
                data[entity.Id] = entity.GetPrimitiveCopy();
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
            ContextQuery<T> ctx;
            var data = GetEntityCollection(typeof(T));

            // also check config settings to make sure user wants to exclude Deleted
            ctx = new ContextQuery<T>(null, data.Values.Cast<T>().Where(x => x?.Deleted == false && x.Id == id).AsQueryable());
            return ctx;
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));

            if (data.ContainsKey(id))
            {
                var item = data[id] as T;
                return item;
            }

            return null;
        }

        public async Task DeleteAsync<T>(int id) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));
            data.Remove(id);
        }

        public async Task SoftDeleteAsync<T>(T entity) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));
            var item = data[entity.Id] as T;
            item.Deleted = true;
        }

        public async Task SoftDeleteAsync<T>(int id) where T : class, IEntity
        {
            var data = GetEntityCollection(typeof(T));
            var item = data[id] as T;
            item.Deleted = true;
        }
    }
}
