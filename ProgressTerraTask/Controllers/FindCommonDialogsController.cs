using iMessengerCoreAPI.Models;
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
        public Guid Post(IEnumerable<Guid> clients)
        {
            var dialogs = _repo
                .GroupBy(x => x.IDRGDialog, x => x.IDClient)
                .Where(x => !clients.Except(x).Any())
                .Select(x => x.Key);

            return dialogs.Count() > 0 ? dialogs.First() : Guid.Empty;
        }
    }
}
