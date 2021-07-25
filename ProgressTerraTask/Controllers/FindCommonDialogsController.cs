using iMessengerCoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProgressTerraTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FindCommonDialogsController : ControllerBase
    {
        private readonly ILogger<FindCommonDialogsController> _logger;
        private readonly IEnumerable<RGDialogsClients> _repo;
        private readonly IHostEnvironment _env;

        public FindCommonDialogsController(ILogger<FindCommonDialogsController> logger, IEnumerable<RGDialogsClients> repo, IHostEnvironment hostEnv)
        {
            _logger = logger;
            _repo = repo;
            _env = hostEnv;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Guid> Post(
            //It's questionable whether the length should be validated as the task does not explicitly specify that
            //The other valid returned value for 0 clients is any dialog. Removing the MinLength attribute achieves that
            [FromBody][MinLength(1, ErrorMessage = "At least one valid client GUID is expected, 0 were provided")] IEnumerable<Guid> clients
            )
        {
            if (_env.IsDevelopment())
            {
                _logger.LogInformation($"New request with {clients.Count()} client(s):\n{string.Join('\n', clients)}");
            }

            var dialogs = _repo
                .GroupBy(x => x.IDRGDialog, x => x.IDClient)
                .Where(x => !clients.Except(x).Any())
                .Select(x => x.Key);

            if (_env.IsDevelopment())
            {
                var cnt = dialogs.Count();
                _logger.LogInformation($"Found {cnt} common dialog(s){(cnt > 0 ? ":" + Environment.NewLine : ".")}{string.Join('\n', dialogs)}");
            }

            //The task explicitly asks to return "a dialog" while it is very possible that more than one would match the provided GUIDs
            //Therefore I am explicitly returning just one (1st) dialog here
            return dialogs.Any() ? dialogs.First() : Guid.Empty;
        }
    }
}
