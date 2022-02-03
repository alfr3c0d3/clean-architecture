using CleanArchitecture.Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Api.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login.Query query)
        {
            return Ok(await Mediator.Send(query));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Login(Register.Command command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> CurrentUser()
        {
            return Ok(await Mediator.Send(new CurrentUser.Query()));
        }

    }
}
