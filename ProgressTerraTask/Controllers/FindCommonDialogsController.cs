using iMessengerCoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgressTerraTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindCommonDialogsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<FindCommonDialogsController> _logger;
        private readonly IEnumerable<RGDialogsClients> _repo;

        public FindCommonDialogsController(ILogger<FindCommonDialogsController> logger, IEnumerable<RGDialogsClients> repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Guid Post([FromBody] IEnumerable<Guid> clients)
        {
            var dialogs = _repo
                .GroupBy(x => x.IDRGDialog, x => x.IDClient)
                .Where(x => !clients.Except(x).Any())
                .Select(x => x.Key);

            return dialogs.Any() ? dialogs.First() : Guid.Empty;
        }
    }
}
