using Kromi.Application.Contracts.Services;
using Kromi.Application.Data.Dto.Auth;
using Kromi.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Kromi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous, HttpPost("Login", Name = "Login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AuthResponse))]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest loginUser)
        {
            var result = await _authService.Login(loginUser);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("Refresh", Name = "Refresh")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(RefreshResponse))]
        public async Task<IActionResult> RefreshAuthentication([FromBody] RefreshResponse request)
        {
            var result = await _authService.RefreshToken(request);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return Forbid();
        }

        [HttpGet("PingToken")]
        public IActionResult PingToken()
        {
            return Ok(new { Message = "Token is valid" });
        }
    }
}
