using BlazorForms.Shared.FastReflection;
using BlazorForms.Shared.Reflection;
using BlazorForms.Shared.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Tests
{
    public class FastReflectionProviderTests
    {
        private IFastReflectionProvider _provider = new FastReflectionProvider();

        [Fact]
        public void GetStraightEmitterGetIntTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { AddressId = 1 }, PrimaryEmail = null };
            var func = _provider.GetStraightEmitterGet(model.GetType(), "$.PostAddress.AddressId");
            var value = func(model).AsInt();
            Assert.Equal(1, value);
        }

        [Fact]
        public void GetStraightEmitterSetIntTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { AddressId = 1 } };
            var action = _provider.GetStraightEmitterSet(model.GetType(), "$.PostAddress.AddressId");
            action(model, 2);
            Assert.Equal(2, model.PostAddress.AddressId);
        }

        [Fact]
        public void GetStraightEmitterSetTest()
        {
            var model = new ClientModel { Title = "Mr" };
            var action = _provider.GetStraightEmitterSet(model.GetType(), "$.Title");
            action(model, "Mrs");
            Assert.Equal("Mrs", model.Title);
        }

        [Fact]
        public void GetStraightEmitterSetLevel2Test()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var action = _provider.GetStraightEmitterSet(model.GetType(), "$.PostAddress.State");
            action(model, "ACT");
            Assert.Equal("ACT", model.PostAddress.State);
        }

        [Fact]
        public void GetStraightEmitterGetLevel1Test()
        {
            var model = new ClientModel { TaxFileNumber = "123", PostAddress = new AddressModel { State = "NSW", Box = new AddressBox { Name = "Cool" } } };
            var func = _provider.GetStraightEmitterGet(model.GetType(), "$.TaxFileNumber");
            var value = func(model);
            Assert.Equal("123", model.TaxFileNumber);
        }


        [Fact]
        public void GetStraightEmitterGetLevel3Test()
        {
            var model = new ClientModel { TaxFileNumber = "123", PostAddress = new AddressModel { State = "NSW", Box = new AddressBox { Name = "Cool" } } };
            var func = _provider.GetStraightEmitterGet(model.GetType(), "$.PostAddress.Box.Name");
            var value = func(model);
            Assert.Equal("Cool", model.PostAddress.Box.Name);
        }

        [Fact]
        public void NestedObjectNullTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var func = _provider.GetStraightEmitterGet(model.GetType(), "$.PrimaryEmail.EmailAddress");
            Assert.ThrowsAny<NullReferenceException>(() => func(model));
        }

        [Fact]
        public void NestedObjectWrongPropertyTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            Assert.ThrowsAny<NullReferenceException>(() => _provider.GetStraightEmitterGet(model.GetType(), "$.PostAddress.EmailAddress"));
        }

        [Fact]
        public void NotStraightPropertyTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, AddressList = new List<AddressModel>()  };
            model.AddressList.Add(new AddressModel { State = "ACT" });
            Assert.ThrowsAny<ArgumentException>(() => _provider.GetStraightEmitterGet(model.GetType(), "$.AddressList[0].EmailAddress"));
        }
    }
}
