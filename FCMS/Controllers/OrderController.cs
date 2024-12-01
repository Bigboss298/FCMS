using FCMS.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("GetAll")] 
        public async Task<IActionResult> GetAll()
     {
            try
            {
                var allOrders = await _orderService.GetAll();
                return Ok(allOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("AllCustomersOrder")]
        public async Task<IActionResult> GetAllMyOrderC(string param)
        {
            try
            {
                var allMyOrder = await _orderService.GetAllMyOrderC(param);
                return Ok(allMyOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("AllFarmersOrder")]
        public async Task<IActionResult> GetAllMyOrderF(string param)
        {
            try
            {
                var allMyOrder = await _orderService.GetAllMyOrderF(param);
                return Ok(allMyOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("SingleOrder")]
        public async Task<IActionResult> GetOrder(string id)
        {
            try
            {
                var order = await _orderService.Get(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(5000, ex.Message);
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateOrder(int param, string id)
        {
            try
            {
                var order = await _orderService.UpdateAsync(param, id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
