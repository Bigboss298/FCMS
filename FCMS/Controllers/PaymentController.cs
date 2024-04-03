//using FCMS.Gateway;
//using FCMS.Model.Exceptions;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//[ApiController]
//[Route("[controller]")]
//public class PaymentController : ControllerBase
//{
//    private readonly IPaystackService _paystackService;

//    public PaymentController(IPaystackService paystackService)
//    {
//        _paystackService = paystackService;
//    }
//    //[FromRoute] string customerId, [FromRoute] string productId
//    [HttpPost("InitiatePayment")]
//    public async Task<IActionResult> MakePayMent([FromForm] CreatePaymentRequestModel model)
//    {
//        try
//        {
//            var transaction = _paystackService.InitiatePayment(model);
//            return Ok(transaction);
//        }
//        catch(NotFoundException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(ex.Message);
//        }

//    }


//    [HttpGet("Get/{transactionReference}")]
//    public async Task<IActionResult> GetTransactionReceipt([FromRoute] string transactionReference)
//    {
//        try
//        {
//            var transaction = _paystackService.GetTransactionRecieptAsync(transactionReference);
//            return new OkObjectResult(transaction);
//        }
//        catch(NotFoundException ex)
//        {
//            return NotFound(ex.Message);
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(ex.Message);
//        }
       
//    }
//}
