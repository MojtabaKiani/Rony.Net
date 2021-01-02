using Rony.Handlers;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace Rony.Tests.Handler
{
    public class RequestHandlerTests
    {
        private readonly RequestHandler _handler;

        public RequestHandlerTests()
        {
            //Arrange
            _handler = new RequestHandler();
        }
        [Fact]
        public void Constructor_Should_Work_Correctly()
        {
            //Assert
            Assert.NotNull(_handler.Configs);
        }

        [Theory]
        [InlineData("TestData", "TestRecieveData")]
        [InlineData("1234","2345")]
        [InlineData("ABCD","EFGHI")]
        [InlineData("4321","HGHST")]
        [InlineData(",#$@","*&%$#@")]
        public void Config_With_String_Should_Add_Config(string request,string response)
        {
            //Act
            _handler.Send(request).Receive(response);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.Equal(response, _handler.Match(request));
        }

        [Theory]
        [InlineData("TestData", "TestRecieveData")]
        [InlineData("1234", "2345")]
        [InlineData("ABCD", "EFGHI")]
        [InlineData("4321", "HGHST")]
        [InlineData(",#$@", "*&%$#@")]
        public void Config_With_Byte_Array_Should_Add_Config(string request, string response)
        {
            //Arrange
            var requestByte = Encoding.UTF8.GetBytes(request);
            var responseByte = Encoding.UTF8.GetBytes(response);
            
            //Act
            _handler.Send(requestByte).Receive(responseByte);
            var received = _handler.Match(Encoding.UTF8.GetString(requestByte));
            var receivedBytes = Encoding.UTF8.GetBytes(received);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.True(responseByte.SequenceEqual(receivedBytes));
        }

        [Theory]
        [InlineData("TestData", "TESTDATA")]
        [InlineData("1234", "1234")]
        [InlineData("ABCD", "ABCD")]
        [InlineData("asdfgh", "ASDFGH")]
        public void Config_With_Func_Of_String_Should_Add_Config(string request, string response)
        {
            //Act
            _handler.Send(request).Receive(x=> x.ToUpper());

            //Assert
            Assert.Single(_handler.Configs);
            Assert.Equal(response, _handler.Match(request));
        }

        [Fact]
        public void Config_With_Func_Of_Byte_Array_Should_Add_Config()
        {
            //Arrange
            var request = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expectedResponse = new byte[] { 1, 3, 5, 7, 9 };

            //Act
            _handler.Send(request).Receive(x => x.Select((s,i)=> (s,i)).Where(t=> t.i % 2 ==0).Select(t=> t.s).ToArray());
            var received = _handler.Match(Encoding.UTF8.GetString(request));
            var receivedBytes = Encoding.UTF8.GetBytes(received);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.True(expectedResponse.SequenceEqual(receivedBytes));
        }

        [Theory]
        [InlineData("Try Me")]
        [InlineData("123456")]
        [InlineData("Try @76453")]
        [InlineData(" ")]
        [InlineData("&#^@%")]
        public void Config_With_Empty_String_Should_Match_Any_Request(string request)
        {
            //Act
            _handler.Send("").Receive("I match Everything");
            var received = _handler.Match(request);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.Equal("I match Everything",received);
        }

        [Theory]
        [InlineData("Try Me")]
        [InlineData("123456")]
        [InlineData("Try @76453")]
        [InlineData(" ")]
        [InlineData("&#^@%")]
        public void Config_With_Empty_String_Should_Match_Any_Request_With_Func_Of_String(string request)
        {
            //Act
            _handler.Send("").Receive(x=> x.ToUpper());
            var received = _handler.Match(request);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.Equal(request.ToUpper(), received);
        }


        [Theory]
        [InlineData("Try Me")]
        [InlineData("123456")]
        [InlineData("Try @76453")]
        [InlineData(" 33")]
        [InlineData("&#^@%")]
        public void Config_With_Empty_String_Should_Match_Any_Request_With_Func_Of_Byte(string request)
        {
            //Act
            _handler.Send("").Receive(x => x.Take(3).ToArray());
            var received = _handler.Match(request);

            //Assert
            Assert.Single(_handler.Configs);
            Assert.Equal(request.Substring(0,3), received);
        }
    }
}
