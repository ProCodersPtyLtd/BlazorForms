using BlazorForms.Shared.Reflection;
using BlazorForms.Shared.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Shared.Tests
{
    public class FastReflectionTests
    {
        [Fact]
        public void GetJsonPathStraightEmitterGetTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var func = FastReflectionPrototype.GetJsonPathStraightEmitterGet(model.GetType(), "$.PostAddress.State");
            var state = func.CreateDelegate()(model);
            Assert.Equal("NSW", state);
        }

        [Fact]
        public void GetJsonPathStraightEmitterGetIntTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { AddressId = 1 }, PrimaryEmail = null };
            var func = FastReflectionPrototype.GetJsonPathStraightEmitterGet(model.GetType(), "$.PostAddress.AddressId");
            var value = func.CreateDelegate()(model).AsInt();
            Assert.Equal(1, value);
        }

        [Fact]
        public void GetJsonPathStraightEmitterSetTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var action = FastReflectionPrototype.GetJsonPathStraightEmitterSet(model.GetType(), "$.PostAddress.State").CreateDelegate();
            action(model, "ACT");
            Assert.Equal("ACT", model.PostAddress.State);
        }

        [Fact]
        public void GetJsonPathStraightEmitterSetIntTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { AddressId = 1 } };
            var action = FastReflectionPrototype.GetJsonPathStraightEmitterSet(model.GetType(), "$.PostAddress.AddressId").CreateDelegate();
            action(model, 2);
            Assert.Equal(2, model.PostAddress.AddressId);
        }

        [Fact]
        public void GetJsonPathStraightLevelsEmitterTest()
        {
            var model = new ClientModel { TaxFileNumber = "123", PostAddress = new AddressModel { State = "NSW", Box = new AddressBox { Name = "Cool" } } };
            
            // level 1
            var func = FastReflectionPrototype.GetJsonPathStraightEmitterGet(model.GetType(), "$.TaxFileNumber");
            var value = func.CreateDelegate()(model);
            Assert.Equal("123", value);

            // level 3
            func = FastReflectionPrototype.GetJsonPathStraightEmitterGet(model.GetType(), "$.PostAddress.Box.Name");
            value = func.CreateDelegate()(model);
            Assert.Equal("Cool", value);
        }

        [Fact]
        public void GetJsonPathStraightLevel1EmitterSetTest()
        {
            var model = new ClientModel { TaxFileNumber = "123" };

            // level 1
            var action = FastReflectionPrototype.GetJsonPathStraightEmitterSet(model.GetType(), "$.TaxFileNumber");
            action.CreateDelegate()(model, "222");
            Assert.Equal("222", model.TaxFileNumber);
        }

        [Fact]
        public void GetJsonPathStraightLevelsEmitterSetTest()
        {
            var model = new ClientModel { TaxFileNumber = "123", PostAddress = new AddressModel { State = "NSW", Box = new AddressBox { Name = "Cool" } } };

            // level 1
            var action = FastReflectionPrototype.GetJsonPathStraightEmitterSet(model.GetType(), "$.PostAddress.Box.Name");
            action.CreateDelegate()(model, "NotCool");
            Assert.Equal("NotCool", model.PostAddress.Box.Name);
        }

        [Fact]
        public void NestedObjectTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var func = FastReflectionPrototype.GetNestedEmitter(model, "$.PostAddress.State");
            var state = func.CreateDelegate()(model);
            Assert.Equal("NSW", state);
        }

        [Fact]
        public void NestedObjectNullTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            var func = FastReflectionPrototype.GetNestedEmitter(model, "$.PrimaryEmail.EmailAddress");
            Assert.ThrowsAny<NullReferenceException>(() => func.CreateDelegate()(model));
        }

        [Fact]
        public void NestedObjectWrongPropertyTest()
        {
            var model = new ClientModel { PostAddress = new AddressModel { State = "NSW" }, PrimaryEmail = null };
            Assert.ThrowsAny<NullReferenceException>(() => FastReflectionPrototype.GetNestedEmitter(model, "$.PostAddress.EmailAddress"));
        }
    }
}
