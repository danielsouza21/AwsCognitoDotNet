using Api.Bff.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Bff.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProtectedController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("secure-data")]
        public async Task<IActionResult> GetSecureData()
        {
            var accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access Token não encontrado.");
            }

            var userInfo = await _userService.GetUserAsync(accessToken);
            return Ok(new { message = "Este é um endpoint protegido. Você está autenticado!", userInfo });
        }
    }

}
