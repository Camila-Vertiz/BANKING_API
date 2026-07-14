using Banking.Application.Requests.BankAccount;
using Banking.Application.Services;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;


        public BankAccountsController(
            IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        /// <summary>
        /// Crea una nueva cuenta bancaria.
        /// </summary>
        /// <remarks>
        /// Disponible únicamente para usuarios con rol Admin.
        /// </remarks>
        /// <param name="request">
        /// Datos de la cuenta bancaria.
        /// </param>
        /// <returns>
        /// Cuenta bancaria creada.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            CreateBankAccountRequest request)
        {
            var result = await _bankAccountService
                .CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Obtiene todas las cuentas bancarias registradas.
        /// </summary>
        /// <remarks>
        /// Endpoint disponible únicamente para administradores.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _bankAccountService.GetAllAsync();

            return Ok(accounts);
        }

        /// <summary>
        /// Obtiene una cuenta bancaria por su identificador.
        /// </summary>
        /// <param name="id">
        /// Identificador de la cuenta bancaria.
        /// </param>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var account = await _bankAccountService
                .GetByIdAsync(id);

            if (account is null)
                return NotFound();

            return Ok(account);
        }

        /// <summary>
        /// Obtiene las cuentas asociadas a un cliente.
        /// </summary>
        /// <param name="customerId">
        /// Identificador del cliente.
        /// </param>
        [HttpGet("customer/{customerId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByCustomerId(Guid customerId)
        {
            var accounts = await _bankAccountService
                .GetByCustomerIdAsync(customerId);

            return Ok(accounts);
        }

        /// <summary>
        /// Consulta el saldo actual de una cuenta bancaria.
        /// </summary>
        /// <param name="id">
        /// Identificador de la cuenta bancaria.
        /// </param>
        [HttpGet("{id}/balance")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var result = await _bankAccountService
                .GetBalanceAsync(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Obtiene el historial de movimientos de una cuenta bancaria.
        /// </summary>
        /// <remarks>
        /// Los clientes únicamente pueden consultar sus propios movimientos.
        /// Los administradores pueden consultar cualquier cuenta.
        /// </remarks>
        /// <param name="accountId">
        /// Identificador de la cuenta bancaria.
        /// </param>
        [HttpGet("{accountId}/transactions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransactionsByAccountId(
            Guid accountId)
        {
            var result = await _bankAccountService
                .GetTransactionsByAccountIdAsync(accountId);

            return Ok(result);
        }
    }
}