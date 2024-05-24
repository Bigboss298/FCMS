using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Authorization;
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
        //[Authorize]
        [HttpGet("Products")]
        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        //[Authorize]
        [HttpGet("GetByAny")]
        public async Task<IActionResult> GetByAnyAsync(string param)
        {
            try
            {
                var products = await _productService.GetProductsByAny(param);
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

        //[Authorize]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync(string id)
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

        //[Authorize]
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

        //[Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync([FromForm]UpdateProductRequestModel model, string id)
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

        //[Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductRequestModel model)
        {
            try
            {
                var newProduct = await _productService.CreateAsync(model);
                return Ok(newProduct);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
