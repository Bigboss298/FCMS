using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewServices;

        public ReviewController(IReviewService reviewService)
        {
            _reviewServices = reviewService;
        }

        [HttpPost("Create/{farmerId}/{customerId}")]
        public async Task<IActionResult> CreateReview([FromRoute] string farmerId, [FromRoute]string customerId, [FromForm] CreateReviewRequestModel model)
        {
            try
            {
                var reviewToCreate = await _reviewServices.Add(farmerId, customerId, model);
                return Ok(reviewToCreate);
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
        [HttpDelete("DeleteReviewsForFamer")]
        public async Task<IActionResult> DeleteReviews(string farmerId)
        {
            try
            {
                var reviewsToDelete = await _reviewServices.DeleteAllReviews(farmerId);
                return Ok(reviewsToDelete);
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

        [HttpDelete("DeleteSingleReview")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            try
            {
                var reviewToDelete = await _reviewServices.Delete(id);
                return Ok(reviewToDelete);
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

        [HttpPut("UppdateReview")]
        public async Task<IActionResult> UpdateReview([FromRoute]string id, UpdateReviewRequestModel model)
        {
            try
            {
                var reviewToUpdate = await _reviewServices.Update(id, model);
                return Ok(reviewToUpdate);
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

        [HttpGet("GetReview")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            try
            {
                var reviewToGet = await _reviewServices.Get(id);
                return Ok(reviewToGet);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
