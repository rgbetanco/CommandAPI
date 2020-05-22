using System;
using CommandAPI.Models;
using Xunit;

namespace CommandAPI.Test
{
    public class CommandTest: IDisposable
    {
        Command TestCommand;
        public CommandTest()
        {
            //Arrange
            TestCommand = new Command
            {
                HowTo = "something awesome",
                CommandLine = "some command line --port 2000",
                Platform = "xUnit"
            };
        }
        [Fact]
        public void CanChangeHowTo()
        { 
            // ACT
            TestCommand.HowTo = "Hello World";
            //Assert
            Assert.Equal("Hello World", TestCommand.HowTo);
            
            
        }

        [Fact]
        public void CanChangeCommandLine()
        {
            // ACT
            TestCommand.CommandLine = "New command line";
            // Assert
            Assert.Equal("New command line", TestCommand.CommandLine);
        }

        [Fact]
        public void CanChangePlatform()
        {
            TestCommand.Platform = "New platform";
            // ACT
            Assert.Equal("New platform", TestCommand.Platform);
            // Assert

        }

        public void Dispose()
        {
            TestCommand = null;
        }
    }
}
