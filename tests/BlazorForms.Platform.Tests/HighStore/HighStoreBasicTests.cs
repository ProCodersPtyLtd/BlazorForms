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
            var u = new User { Id = 1, Login = "a@a.com", PersonId = p.Id, Person = p };
            _db.UpsertAsync(u);
        }

        [Fact]
        public async Task GetListTest()
        {
            var data = await _db.GetQuery<Person>().ToListAsync();
            Assert.NotEqual(0, data.Count());

            var user = await _db.GetQuery<Person>().ToListAsync();
            Assert.NotEqual(0, data.Count());
        }

        [Fact]
        public async Task GetByIdTest()
        {
            var user = await _db.GetQuery<User>().FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.Null(user.Person);
        }

        [Fact]
        public async Task GetByIdIncludeTest()
        {
            var q = _db.GetByIdQuery<User>(1).Include(m => m.Person);
            var user = await q.FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.NotNull(user.Person);
        }
    }
}
