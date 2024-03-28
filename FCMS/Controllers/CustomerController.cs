using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("Customer")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var customer = await _customerService.GetAsync(id);
                return Ok(customer);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPost("Register")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateCustomerRequestModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newCustomer = await _customerService.CreateAsync(model);
                    return CreatedAtAction("Get", newCustomer.Data.UserId);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            return BadRequest("Fields cannot be empty");
        }
        
    }
}
