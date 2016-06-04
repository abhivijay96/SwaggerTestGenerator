//CODE GENERATED TO TEST THE SWAGGER API

using System;
using Xunit;
namespace Testing
{
	public class ApiTest
	{
			var api = new APIV1(new Uri("http://localhost:5000/"));

			[FACT]
			public void GetValuesTest1() {
				var output = api.ApiValuesGet();
				var actual = new []{ "value1","value2"};
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest2() {
				var output = api.ApiValuesByIdGet(1);
				var actual = "value1";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest3() {
				var output = api.ApiValuesByIdGet(2);
				var actual = "value2";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest4() {
				var output = api.ApiValuesByIdGet(3);
				var actual = "value3";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest5() {
				var output = api.ApiValuesByIdGet(4);
				var actual = "value4";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest6() {
				var output = api.ApiValuesByIdGet(5);
				var actual = "value5";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest7() {
				var output = api.ApiValuesByIdGet(6);
				var actual = "value6";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest8() {
				var output = api.ApiValuesByIdGet(7);
				var actual = "value7";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest9() {
				var output = api.ApiValuesByIdGet(8);
				var actual = "value8";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest10() {
				var output = api.ApiValuesByIdGet(9);
				var actual = "value9";
				Assert.Equal(actual , output);
			}

			[FACT]
			public void GetValuesTest11() {
				var output = api.ApiValuesByIdGet(10);
				var actual = "value10";
				Assert.Equal(actual , output);
			}

			
			//api.ApiValuesPost("Hello world1");
			//api.ApiValuesPost("Hello world2");
			//api.ApiValuesPost("Hello world3");
			//api.ApiValuesPost("Hello world4");
			//api.ApiValuesPost("Hello world5");
			//api.ApiValuesPost("Hello world6");
			//api.ApiValuesPost("Hello world7");
			//api.ApiValuesPost("Hello world8");
			//api.ApiValuesPost("Hello world9");
			//api.ApiValuesPost("Hello world10");
			//api.ApiValuesByIdPut(1,"Hello world1");
			//api.ApiValuesByIdPut(2,"Hello world2");
			//api.ApiValuesByIdPut(3,"Hello world3");
			//api.ApiValuesByIdPut(4,"Hello world4");
			//api.ApiValuesByIdPut(5,"Hello world5");
			//api.ApiValuesByIdPut(6,"Hello world6");
			//api.ApiValuesByIdPut(7,"Hello world7");
			//api.ApiValuesByIdPut(8,"Hello world8");
			//api.ApiValuesByIdPut(9,"Hello world9");
			//api.ApiValuesByIdPut(10,"Hello world10");
			//api.ApiValuesByIdDelete(1);
			//api.ApiValuesByIdDelete(2);
			//api.ApiValuesByIdDelete(3);
			//api.ApiValuesByIdDelete(4);
			//api.ApiValuesByIdDelete(5);
			//api.ApiValuesByIdDelete(6);
			//api.ApiValuesByIdDelete(7);
			//api.ApiValuesByIdDelete(8);
			//api.ApiValuesByIdDelete(9);
			//api.ApiValuesByIdDelete(10);
	}
}

