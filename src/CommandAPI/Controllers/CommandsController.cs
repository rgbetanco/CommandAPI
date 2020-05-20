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
    }
}