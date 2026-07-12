using Banking.Application.Request.Customer;
using Banking.Application.Requests.Customer;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var result = await _customerService.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost("documentNumber")]
        [Authorize]
        public async Task<IActionResult> GetByDocumentNumber(GetCustomerByDocumentRequest request)
        {
            var customer = await _customerService.GetByDocumentAsync(request);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerProfileRequest request)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer is null)
                return NotFound();

            await _customerService.UpdateProfileAsync(id, request);

            return NoContent();
        }
    }
}
