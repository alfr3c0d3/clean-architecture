using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Activities;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> List(int? limit)
        {
            return Ok(await Mediator.Send(new List.Query { Limit = limit }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            return Ok(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Edit.Command command)
        {
            command.Id = id;
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}
