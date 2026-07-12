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