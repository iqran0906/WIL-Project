using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _customerService.CreateCustomerAsync(model);
            return Ok(new { message = "Customer created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerViewModel model)
        {
            if (id != model.CustomerID) return BadRequest("Customer ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _customerService.UpdateCustomerAsync(model);
            return Ok(new { message = "Customer updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return Ok(new { message = "Customer deleted successfully" });
        }
    }
}