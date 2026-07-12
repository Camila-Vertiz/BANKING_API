using Banking.Application.Requests.Transaction;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;


        public TransactionsController(
            ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Executes a money transfer between two bank accounts.
        /// </summary>
        /// <remarks>
        /// The authenticated customer can only transfer money from their own account.
        /// Administrators can operate on any account.
        /// </remarks>
        [HttpPost("transfer")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// Retrieves transactions from a specific bank account.
        /// </summary>
        /// <remarks>
        /// Customers can only access their own account movements.
        /// Administrators can access any account.
        /// </remarks>
        [HttpGet("account/{accountId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByAccountId(
            Guid accountId)
        {
            var result = await _transactionService
                .GetByAccountIdAsync(accountId);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all transactions associated with a trace identifier.
        /// </summary>
        /// <remarks>
        /// Used for audit purposes. Only administrators can access this endpoint.
        /// </remarks>
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
        /// Creates a deposit into a bank account.
        /// </summary>
        /// <remarks>
        /// Only administrators can perform deposits.
        /// A credit transaction will be created automatically.
        /// </remarks>
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