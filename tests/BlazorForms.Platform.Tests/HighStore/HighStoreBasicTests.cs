using BlazorForms.Storage.InMemory;
using BlazorForms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.HighStore
{
    public class HighStoreBasicTests
    {
        IHighStore _db;
        public HighStoreBasicTests()
        {
            _db = new InMemoryHighStore();
            var p = new Person { FirstName = "Oleg", LastName = "Ivanov", BirthDate = new DateTime(1990, 10, 2) };
            _db.UpsertAsync(p);
            var u = new User { Login = "a@a.com", PersonId = p.Id };
            _db.UpsertAsync(u);
        }

        [Fact]
        public async Task GetListTest()
        {
            var data = await _db.GetQuery<Person>().ToListAsync();
            Assert.NotEqual(0, data.Count());
        }

        [Fact]
        public async Task GetJoinListTest()
        {
            var q = _db.GetQuery<User>().Include(m => m.Person);
            var data = await q.ToListAsync();
        }
    }
}
