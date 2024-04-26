using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FaqController : ControllerBase
    {
       
            private readonly IFaqService _faqService;

            public FaqController(IFaqService faqService)
            {
                _faqService = faqService;
            }
            //[Authorize]
            [HttpGet("AllFAQ")]
            public async Task<IActionResult> GetFaqs()
            {
                try
                {
                    var faq = await _faqService.GetAllFaqs();
                    return Ok(faq);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            //[Authorize]
            [HttpGet("GetFaq")]
            public async Task<IActionResult> Get(string id)
            {
                try
                {
                    var faq = await _faqService.GetFaqById(id);
                    return Ok(faq);
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
            [HttpPost("CreateFAQ")]
            public async Task<IActionResult> CreateAsync([FromForm] CreateFaqRequestModel model)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var newfaq = await _faqService.CreateAsync(model);
                        return Ok(newfaq);
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



