using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CommandAPI.Models;

namespace CommandAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CommandsController: ControllerBase
    {
        private CommandContext _context;
        public CommandsController(CommandContext Context) => _context = Context;

        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandItems()
        {
            return _context.CommandItems;
        }

        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandItem(int id)
        {
            Command toReturn = _context.CommandItems.Find(id);
            if (toReturn == null)
            {
                return NotFound();
            }

            return toReturn;
        }

        [HttpPost]
        public ActionResult<Command> PostCommandItem (Command command)
        {
            _context.CommandItems.Add(command);
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }
            return CreatedAtAction("GetCommandItem", new Command { Id = command.Id }, command); 
        }

        [HttpPut("{id}")]
        public ActionResult PutCommandItem(int id, Command command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            _context.Entry(command).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Command> DeleteCommandItem(int id)
        {
            var command_item = _context.CommandItems.Find(id);
            if (command_item == null)
            {
                return NotFound();
            }
            _context.CommandItems.Remove(command_item);
            _context.SaveChanges();

            return command_item;
        }
    }
}