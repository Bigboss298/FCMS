using FCMS.Gateway;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaystackService _paystackService;

    public PaymentController(IPaystackService paystackService)
    {
        _paystackService = paystackService;
    }
    [HttpPost("InitiatePayment")]
    public async Task<IActionResult> RequestPayment([FromForm] RequestPaymentModel model)
    {
        try
        {
            var transaction = await _paystackService.Payment(model);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("Get/{transactionReference}")]
    public async Task<IActionResult> GetTransactionReceipt([FromRoute] string transactionReference)
    {
        try
        {
            var transaction = await _paystackService.GetTransactionRecieptAsync(transactionReference);
            return new OkObjectResult(transaction);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
}
