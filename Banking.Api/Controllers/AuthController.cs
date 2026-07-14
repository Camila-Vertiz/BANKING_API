using Banking.Application.Requests.Auth;
using Banking.Application.Responses;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema bancario.
        /// </summary>
        /// <remarks>
        /// Crea un usuario con rol Customer y su información asociada como cliente.
        /// </remarks>
        /// <param name="request">
        /// Datos necesarios para registrar un usuario.
        /// </param>
        /// <returns>
        /// Información del usuario y cliente creado.
        /// </returns>
        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            var response =
                await _authService.RegisterAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        /// <summary>
        /// Autentica un usuario y genera un token JWT.
        /// </summary>
        /// <remarks>
        /// El token generado debe enviarse en el encabezado Authorization
        /// para acceder a endpoints protegidos.
        /// </remarks>
        /// <param name="request">
        /// Credenciales del usuario.
        /// </param>
        /// <returns>
        /// Token JWT e información del usuario autenticado.
        /// </returns>
        /// <response code="200">
        /// Usuario autenticado correctamente.
        /// </response>
        /// <response code="401">
        /// Credenciales incorrectas.
        /// </response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
    }
}
