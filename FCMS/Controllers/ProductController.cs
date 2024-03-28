using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }
        [HttpGet("GetByAny")]
        public IActionResult GetByAnyAsync(string param)
        {
            try
            {
                var products = _productService.GetAsync(param);
                return Ok(products);
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
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
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
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var productToDelete = await _productService.DeleteAsync(id);
            if (productToDelete)
            {
                return Ok("Deleted Succesfully!!!");
            }
            return BadRequest("id not valid!!!");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateProductRequestModel model, string id)
        {
            try
            {
                var productToUpdate = await _productService.UpdateAsync(model, id);
                return Ok(productToUpdate);
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
    }
}
