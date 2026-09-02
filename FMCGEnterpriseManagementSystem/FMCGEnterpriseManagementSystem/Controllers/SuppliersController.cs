using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    /// <summary>
    /// Handles API endpoints for Supplier management operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        /// <summary>
        /// Retrieves all registered suppliers.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        /// <summary>
        /// Retrieves a supplier by their unique Supplier ID.
        /// </summary>
        /// <param name="id">The unique identifier of the supplier.</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        /// <summary>
        /// Registers a new supplier record.
        /// </summary>
        /// <param name="model">The supplier data payload.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _supplierService.CreateSupplierAsync(model);
            return Ok(new { message = "Supplier created successfully" });
        }

        /// <summary>
        /// Updates an existing supplier record.
        /// </summary>
        /// <param name="id">The Supplier ID matching the URL path.</param>
        /// <param name="model">The updated supplier data payload.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] SupplierViewModel model)
        {
            if (id != model.SupplierID) return BadRequest("Supplier ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _supplierService.UpdateSupplierAsync(model);
            return Ok(new { message = "Supplier updated successfully" });
        }

        /// <summary>
        /// Deletes a supplier record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the supplier to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _supplierService.DeleteSupplierAsync(id);
            return Ok(new { message = "Supplier deleted successfully" });
        }
    }
}