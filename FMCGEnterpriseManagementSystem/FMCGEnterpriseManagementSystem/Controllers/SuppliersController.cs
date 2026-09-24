using FMCGEnterpriseManagementSystem.Services.Interfaces;
using FMCGEnterpriseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FMCGEnterpriseManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword)
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    suppliers = suppliers.Where(s =>
                        (!string.IsNullOrEmpty(s.CompanyName) && s.CompanyName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(s.Email) && s.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(s.ContactPerson) && s.ContactPerson.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    );
                }

                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving suppliers.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null) return NotFound(new { message = "Supplier not found." });
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierViewModel model)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _supplierService.CreateSupplierAsync(model);
                return Ok(new { message = "Supplier created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while creating the supplier." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierViewModel model)
        {
            if (id != model.SupplierId)
            {
                return BadRequest("Supplier ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _supplierService.UpdateSupplierAsync(model);
                return Ok(new { message = "Supplier updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(id);
                return Ok(new { message = "Supplier deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Could not delete supplier.", error = ex.Message });
            }
        }
    }
}