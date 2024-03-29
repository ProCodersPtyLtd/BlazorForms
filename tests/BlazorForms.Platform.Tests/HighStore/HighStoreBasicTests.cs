﻿using BlazorForms.Shared.Extensions;
using BlazorForms.Storage;
using BlazorForms.Storage.InMemory;
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
            var p2 = new Person { FirstName = "Ivan", LastName = "Olegov", BirthDate = new DateTime(1970, 10, 2) };
            _db.UpsertAsync(p2);
            var u = new User { Id = 1, Login = "a@a.com", PersonId = p.Id, Person = p };
            _db.UpsertAsync(u);
        }

        [Fact]
        public async Task GetListTest()
        {
            var data = await _db.GetQuery<Person>().ToListAsync();
            Assert.NotEqual(0, data.Count());
        }

        [Fact]
        public async Task GetListOptionsTest()
        {
            var options = new QueryOptions { AllowPagination = true, PageSize = 1 };
            var data = await _db.GetQuery<Person>().ToListAsync(options);
            Assert.NotEqual(1, data.Count());
        }

        [Fact]
        public async Task GetByIdTest()
        {
            var user = await _db.GetQuery<User>().FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.Null(user.Person);
        }

        [Fact]
        public async Task GetWhereTest()
        {
            var c = new Company { Name = "Neo1" };
            await _db.UpsertAsync(c);
            var cid = c.Id;
            c = new Company { Name = "Neo11" };
            await _db.UpsertAsync(c);
            var cid2 = c.Id;

            var q = _db.GetQuery<PersonCompanyLink>().Where(m => m.CompanyId == cid2);
            var data = await q.ToListAsync();
            Assert.Empty(data);

            var l = new PersonCompanyLink { CompanyId = cid, PersonId = 1 };
            await _db.UpsertAsync(l);
            l = new PersonCompanyLink { CompanyId = cid2, PersonId = 2 };
            await _db.UpsertAsync(l);

            q = _db.GetQuery<PersonCompanyLink>().Where(m => m.CompanyId == cid2);
            data = await q.ToListAsync();
            Assert.Equal(1, data.Count);
        }

        [Fact]
        public async Task GetByIdIncludeTest()
        {
            var q = _db.GetByIdQuery<User>(1).Include(m => m.Person);
            var user = await q.FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.NotNull(user.Person);
        }

        [Fact]
        public async Task GetWhereIncludeTest()
        {
            var c = new Company { Name = "Neo2" };
            await _db.UpsertAsync(c);
            var l = new PersonCompanyLink { CompanyId = c.Id, PersonId = 1 };
            await _db.UpsertAsync(l);

            var q = _db.GetQuery<PersonCompanyLink>().Include(m => m.RefPersonLink).Where(m => m.CompanyId == c.Id);
            var data = await q.ToListAsync();
            Assert.NotEmpty(data);
            Assert.Equal(1, data[0].RefPersonLink.Count());
        }

        [Fact]
        public async Task GetByIdAsyncTest()
        {
            var user = _db.GetByIdAsync<User>(1);
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
        }

        [Fact]
        public async Task DeleteAsyncTest()
        {
            var p = new Person { FirstName = "Oleg", LastName = "Olegov", BirthDate = new DateTime(1990, 10, 2) };
            await _db.UpsertAsync(p);
            Assert.True(p.Id > 0);

            var item = await _db.GetByIdAsync<Person>(p.Id);
            Assert.NotNull(item);

            await _db.DeleteAsync<Person>(p.Id);

            item = await _db.GetByIdAsync<Person>(p.Id);
            Assert.Null(item);
        }

        [Fact]
        public async Task SoftDeleteAsyncTest()
        {
            var p = new Person { FirstName = "Oleg", LastName = "Olegov", BirthDate = new DateTime(1990, 10, 2) };
            await _db.UpsertAsync(p);
            Assert.True(p.Id > 0);
            int id = p.Id;

            var item = await _db.GetByIdAsync<Person>(id);
            Assert.NotNull(item);

            await _db.SoftDeleteAsync<Person>(id);

            item = await _db.GetByIdAsync<Person>(id);
            Assert.NotNull(item);
            Assert.True(item.Deleted);
        }

        [Fact]
        public async Task SoftDeleteAsyncTypedTest()
        {
            var p = new Person { FirstName = "Oleg", LastName = "Olegov", BirthDate = new DateTime(1990, 10, 2) };
            await _db.UpsertAsync(p);
            Assert.True(p.Id > 0);
            int id = p.Id;

            var item = await _db.GetByIdAsync<Person>(id);
            Assert.NotNull(item);

            await _db.SoftDeleteAsync(p);

            item = await _db.GetByIdAsync<Person>(id);
            Assert.NotNull(item);
            Assert.True(item.Deleted);
        }
    }
}
