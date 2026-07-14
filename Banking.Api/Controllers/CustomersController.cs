using Banking.Application.Request.Customer;
using Banking.Application.Requests.Customer;
using Banking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Crea un nuevo cliente bancario.
        /// </summary>
        /// <remarks>
        /// Disponible únicamente para usuarios con rol Admin.
        /// </remarks>
        /// <param name="request">
        /// Información personal del cliente.
        /// </param>
        /// <returns>
        /// Cliente creado.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(CreateCustomerRequest request)
        {
            var result = await _customerService.CreateAsync(request);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Obtiene un cliente mediante su identificador.
        /// </summary>
        /// <param name="id">
        /// Identificador único del cliente.
        /// </param>
        /// <returns>
        /// Información del cliente solicitado.
        /// </returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        /// <summary>
        /// Busca un cliente mediante tipo y número de documento.
        /// </summary>
        /// <param name="request">
        /// Datos del documento del cliente.
        /// </param>
        /// <returns>
        /// Información del cliente encontrado.
        /// </returns>
        [HttpPost("document")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDocumentNumber(GetCustomerByDocumentRequest request)
        {
            var customer = await _customerService.GetByDocumentAsync(request);

            if (customer is null)
                return NotFound();

            return Ok(customer);
        }

        /// <summary>
        /// Obtiene la lista de clientes registrados.
        /// </summary>
        /// <returns>
        /// Colección de clientes.
        /// </returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Actualiza la información del perfil de un cliente.
        /// </summary>
        /// <remarks>
        /// La actualización se realiza utilizando el documento del cliente.
        /// </remarks>
        /// <param name="request">
        /// Nuevos datos del cliente.
        /// </param>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateCustomerProfileRequest request)
        {
            GetCustomerByDocumentRequest getCustomerByDocumentRequest = new GetCustomerByDocumentRequest();
            getCustomerByDocumentRequest.DocumentType = request.DocumentType;
            getCustomerByDocumentRequest.DocumentNumber = request.DocumentNumber;
            var customer = await _customerService.GetByDocumentAsync(getCustomerByDocumentRequest);

            if (customer is null)
                return NotFound();

            await _customerService.UpdateProfileAsync(request);

            return NoContent();
        }
    }
}
