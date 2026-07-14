using Banking.Application.Requests.Transaction;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;


        public TransactionsController(
            ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Ejecuta una transferencia entre dos cuentas bancarias.
        /// </summary>
        /// <remarks>
        /// Genera automáticamente:
        /// - Movimiento débito en la cuenta origen.
        /// - Movimiento crédito en la cuenta destino.
        /// - TraceId compartido para auditoría.
        /// Los clientes solo pueden transferir desde sus propias cuentas.
        /// </remarks>
        /// <param name="request">
        /// Datos necesarios para realizar la transferencia.
        /// </param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Transfer(
            TransferRequest request)
        {
            var result = await _transactionService
                .TransferAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene los movimientos asociados a un TraceId.
        /// </summary>
        /// <remarks>
        /// Endpoint utilizado para auditoría.
        /// Disponible únicamente para administradores.
        /// </remarks>
        /// <param name="traceId">
        /// Identificador de trazabilidad de la operación.
        /// </param>
        [HttpGet("trace/{traceId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByTraceId(Guid traceId)
        {
            var transactions = await _transactionService
                .GetByTraceIdAsync(traceId);

            return Ok(transactions);
        }

        /// <summary>
        /// Realiza un depósito en una cuenta bancaria.
        /// </summary>
        /// <remarks>
        /// Disponible únicamente para administradores.
        /// Genera automáticamente un movimiento de crédito.
        /// </remarks>
        /// <param name="request">
        /// Datos del depósito.
        /// </param>
        [HttpPost("deposit")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Deposit(DepositRequest request)
        {
            var result = await _transactionService
                .DepositAsync(request);

            return Ok(result);
        }
    }
}