using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmerController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateFarmerRequestModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var newFarmer = await _farmerService.CreateAsync(model);
                    return Ok(newFarmer);

                }
                catch (Exception ex)
                {

                    return StatusCode(500, ex.Message);
                }
            }
            return BadRequest("Invalid details");
        }

        [Authorize]
        [HttpGet("Farmer")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var farmer = await _farmerService.GetAsync(id);
                return Ok(farmer);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Farmers")]
        public async Task<IActionResult> GetFarmers()
        {
            var farmers = await _farmerService.GetFarmersAsync();
            return Ok(farmers);
        }
    }
}
