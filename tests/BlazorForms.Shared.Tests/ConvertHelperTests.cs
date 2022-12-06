using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BlazorForms.Shared.Tests
{
    public class ConvertHelperTests
    {
        [Fact]
        public void ListConvertToJaggedArrayTest()
        {
            var list = (new string[] { 
                "id", "name", "email", "admin",
                "10", "Oleg", "1@e.r", "yes"
            }).ToList();

            var arr = ConvertHelper.ConvertToJaggedArray(list, 4);
            Assert.Equal(2, arr.Length);
            Assert.Equal(4, arr[0].Length);
            Assert.Equal(4, arr[1].Length);
            Assert.Equal("id", arr[0][0]);
            Assert.Equal("name", arr[0][1]);
            Assert.Equal("yes", arr[1][3]);
        }

        [Fact]
        public void ListConvertToJaggedArrayEmptyTest()
        {
            var list = (new string[] {}).ToList();

            var arr = ConvertHelper.ConvertToJaggedArray(list, 0);
            Assert.Empty(arr);
        }
    }
}
