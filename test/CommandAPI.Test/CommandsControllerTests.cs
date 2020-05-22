using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Controllers;
using CommandAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CommandAPI.Test
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommandContext> OptionBuilder;
        CommandContext dbContext;
        CommandsController controller;
        public CommandsControllerTests()
        {
            OptionBuilder = new DbContextOptionsBuilder<CommandContext>();
            OptionBuilder.UseInMemoryDatabase("UnitTestInMemBD");
            dbContext = new CommandContext(OptionBuilder.Options);
            controller = new CommandsController(dbContext);
        }
        public void Dispose()
        {
            OptionBuilder = null;
            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }

        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //ACT
            var result = controller.GetCommandItems();
            //ASSERT
            Assert.Empty(result.Value);
        }

        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            //Arrange
            var newCommand = new Command
            {
                HowTo = "Testing how to",
                CommandLine = "Testing command line",
                Platform = "Testing platform"
            };
            dbContext.CommandItems.Add(newCommand);
            dbContext.SaveChanges();

            var result = controller.GetCommandItems();

            Assert.Single(result.Value);
        }
        [Fact]
        public void GetCommandItemsReturnNItemsWhenDBHasNObjects()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            var command2 = new Command
            {
                HowTo = "How to 2",
                CommandLine = "command line 2",
                Platform = "Platform 2"
            };

            dbContext.CommandItems.Add(command1);
            dbContext.CommandItems.Add(command2);
            dbContext.SaveChanges();

            var result = controller.GetCommandItems();

            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {
            var result = controller.GetCommandItems();

            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            var result = controller.GetCommandItem(0);

            Assert.Null(result.Value);
        }

        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalidID()
        {
            var result = controller.GetCommandItem(0);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectType()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            dbContext.CommandItems.Add(command1);
            dbContext.SaveChanges();

            var cmdId = command1.Id;

            var result = controller.GetCommandItem(cmdId);

            Assert.IsType<ActionResult<Command>>(result);
        }

        [Fact]
        public void GetCommandItemReturnsTheCorrectResouce()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            dbContext.CommandItems.Add(command1);
            dbContext.SaveChanges();

            var cmdId = command1.Id;

            var result = controller.GetCommandItem(cmdId);

            Assert.Equal(cmdId, result.Value.Id);
        }

        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            var oldCount = dbContext.CommandItems.Count();

            var result = controller.PostCommandItem(command1);

            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
        }

        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            var result = controller.PostCommandItem(command1);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            controller.PostCommandItem(command1);

            var cmdId = command1.Id;

            command1.HowTo = "Updated how to 1";

            controller.PutCommandItem(cmdId, command1);

            var result = controller.GetCommandItem(cmdId);

            Assert.Equal(command1.HowTo, result.Value.HowTo);

        }

        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            dbContext.CommandItems.Add(command1);
            dbContext.SaveChanges();

            var cmdId = command1.Id;

            command1.HowTo = "Updated how to 1";

            var result = controller.PutCommandItem(cmdId, command1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItem_Returns400_WhenInvalidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            dbContext.CommandItems.Add(command1);
            dbContext.SaveChanges();

            var cmdId = command1.Id + 1;

            command1.HowTo = "Updated how to 1";

            var result = controller.PutCommandItem(cmdId, command1);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
        {
            var command1 = new Command
            {
                HowTo = "How to 1",
                CommandLine = "command line 1",
                Platform = "Platform 1"
            };

            controller.PostCommandItem(command1);

            var command2 = new Command
            {
                HowTo = "updated How to 1",
                CommandLine = "updated command line 1",
                Platform = "updated Platform 1"
            };

            controller.PutCommandItem(command1.Id + 1, command2);

            var result = controller.GetCommandItem(command1.Id);

            Assert.Equal(command1.HowTo, result.Value.HowTo);
        }

        [Fact]
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
        {   //Arrange   
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id; var objCount = dbContext.CommandItems.Count();

            //Act   
            controller.DeleteCommandItem(cmdId);

            //Assert  
            Assert.Equal(objCount - 1, dbContext.CommandItems.Count());
        }

        [Fact]
        public void DeleteCommandItem_Returns200OK_WhenValidObjectID()
        {
            //Arrange   
            var command = new Command
            {
                HowTo = "Do Somethting",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            //Act   
            var result = controller.DeleteCommandItem(cmdId);

            //Assert   
            Assert.Null(result.Result);
        }

        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID()
        {   //Arrange                
            //Act   
            var result = controller.DeleteCommandItem(-1);

            //Assert  
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenValidObjectID()
        {
            //Arrange   
            var command = new Command   
            {      
                HowTo = "Do Somethting",     
                Platform = "Some Platform",     
                CommandLine = "Some Command"   
            };   
            dbContext.CommandItems.Add(command);   
            dbContext.SaveChanges(); 

            var cmdId = command.Id; 
            var objCount = dbContext.CommandItems.Count();

            //Act   
            var result = controller.DeleteCommandItem(cmdId+1);  

            //Assert   
            Assert.Equal(objCount, dbContext.CommandItems.Count()); 
        }
    }
}