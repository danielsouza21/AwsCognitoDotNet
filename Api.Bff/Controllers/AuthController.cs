using Amazon.CognitoIdentityProvider.Model;
using Api.Bff.Domain.Configuration;
using Api.Bff.Domain.Services;
using Api.Bff.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Bff.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dtos.LoginRequest loginRequest)
        {
            try
            {
                return Ok(await _authenticationService.LoginWithUsernamePasswordAsync(loginRequest.Username, loginRequest.Password));
            }
            catch (Exception ex)
            {
                return Unauthorized($"Erro de autenticação: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Dtos.RegisterRequest registerRequest)
        {
            try
            {
                await _authenticationService.RegisterUserAsync(registerRequest.Username, registerRequest.Password, registerRequest.Email);
                return Ok("Usuário registrado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao registrar usuário: {ex.Message}");
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            try
            {
                await _authenticationService.ConfirmEmailCode(request.Username, request.ConfirmationCode);
                return Ok("E-mail confirmado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao confirmar o e-mail: {ex.Message}");
            }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendConfirmationRequest request)
        {
            try
            {
                await _authenticationService.ResendConfirmationCode(request.Username);
                return Ok("Código de confirmação reenviado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao reenviar o código: {ex.Message}");
            }
        }
    }
}
