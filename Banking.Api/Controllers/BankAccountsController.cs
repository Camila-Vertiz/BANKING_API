using Banking.Application.Requests.BankAccount;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _bankAccountService.GetAllAsync();

            return Ok(accounts);
        }

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
    }
}