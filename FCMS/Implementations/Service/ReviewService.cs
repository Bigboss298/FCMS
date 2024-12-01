using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FCMS.Implementations.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepoitory;
        private readonly ICustomerRepository _customerRepository;

        public ReviewService(IReviewRepository reviewRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _userRepoitory = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task<BaseResponse<ReviewDto>> Add(CreateReviewRequestModel model)
        {
            if(model is null) throw new Exception(nameof(model));

            var newReview = new Review
            {
                Ratings = model.Ratings,
                Comments = model.Comments,
                FarmerId = model.FarmerId,
                CustomerId = model.CustomerId,
            };
            _reviewRepository.Insert<Review>(newReview);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<ReviewDto>
            {
                Message = "Review Created Successfully!!!",
                Status = true,
            };
        }

        public async Task<BaseResponse<ReviewDto>> Delete(string id)
        {
            var reviewToDelete = await _reviewRepository.Get(x => x.Id == id);
            if (reviewToDelete is null) throw new NotFoundException("Review not Found!!!");
            _reviewRepository.Delete(reviewToDelete);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<ReviewDto>
            {
                Message = "Review Deleted",
                Status = true,
            };
        }

        public async Task<BaseResponse<ReviewDto>> DeleteAllReviews(string farmerId)
        {
            var user = await _userRepoitory.Get(x => x.Id == farmerId);

            var reviewsToDelete = await _reviewRepository.GetAllFarmerReviews(x => x.FarmerId == user.Farmer.Id);
            if (!reviewsToDelete.Any()) throw new NotFoundException("No Reviews for the farmer");
            foreach (var item in reviewsToDelete)
            {
                _reviewRepository.Delete(item);
            }
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<ReviewDto>
            {
                Message = "All reviews Deleted",
                Status = true,
            };
        }

        public async Task<BaseResponse<ReviewDto>> Get(string id)
        {
            if (id is null) throw new BadRequestException("Id cant be null");
            var reviewToGet = await _reviewRepository.Get(x => x.Id == id);
            if (reviewToGet is null) throw new NotFoundException("No such review found");
            return new BaseResponse<ReviewDto>
            {
                Message = "Review Found!!!",
                Status = true,
                Data = reviewToGet.Adapt<ReviewDto>()
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetAllFarmerReviews(string farmerId)
        {
            var user = await _userRepoitory.Get(x => x.Id == farmerId);

            if (user is null) throw new NotFoundException("User not Found!!!");

            var reviewsToGet = await _reviewRepository.GetAllFarmerReviews(x => x.FarmerId == user.Farmer.Id);

            if (!reviewsToGet.Any())
            {
                return new List<ReviewDto>();
            }
            var myReviews = new List<ReviewDto>();
            foreach (var item in reviewsToGet)
            {
                var customerDetails = await _customerRepository.Get(x => x.Id == item.CustomerId);

                if (customerDetails == null)
                {
                    throw new NotFoundException($"Customer with ID {item.CustomerId} not found");
                }

                var review = new ReviewDto
                {
                    Ratings = (int)item.Ratings,
                    Comments = item.Comments,
                    CustomerId = item.CustomerId,
                    CustomerName = $"{customerDetails.User.FirstName} {customerDetails.User.LastName}",
                    FarmerId = item.FarmerId,
                };
                myReviews.Add(review);
            }
            return myReviews;
        }


        public async Task<BaseResponse<ReviewDto>> Update(string id, UpdateReviewRequestModel model)
        {
            var reviewToUpdate = await _reviewRepository.Get(x => x.Id == id);
            if (reviewToUpdate is null) throw new NotFoundException("No such review found!!!");

            reviewToUpdate.Ratings = model.Ratings;
            reviewToUpdate.Comments = model.Comments;

             _reviewRepository.Update<Review>(reviewToUpdate);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<ReviewDto>
            {
                Status = true,
                Message = "Update Successfull!!!",
            };
        }
    }
}
