using FCMS.Implementations.Repository;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using Paystack.Net.SDK;
using Paystack.Net.SDK.Models;

namespace FCMS.Implementations.Service
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _faqRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FaqService(IFaqRepository faqRepository, IUnitOfWork unitOfWork)
        {
            _faqRepository = faqRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponse<FaqDto>> CreateAsync(CreateFaqRequestModel model)
        {
            if (model is null) throw new Exception(nameof(model));
            var faq = new Faq
            {
                Question = model.Question,
                Answer  =  model.Answer
            };
            _faqRepository.Insert<Faq>(faq);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<FaqDto>
            {
                Message = "FAQ Created Successfully!!!",
                Status = true,
            };
        }

        public async Task<List<FaqDto>> GetAllFaqs()
        {
            var faqToGet = await _faqRepository.GetAllFaq();
            if (!faqToGet.Any())
            {
                return new List<FaqDto>();
        
            }
            return faqToGet.Select(x => new FaqDto
            {
               Question =  x.Question,
               Answer   = x.Answer,
            }).ToList();

        }

        public async Task<BaseResponse<FaqDto>> GetFaqById(string id)
        {
            var faq = await _faqRepository.GetFaqById(id);
            if (faq is null)
            {
                throw new NotFoundException($"FAQ with the Id {id} not found");
            }

            return new BaseResponse<FaqDto>
            {
                Message = "FAQ found!!!",
                Status = true,
                Data = new FaqDto
                {
                    Answer = faq.Answer,
                    Question = faq.Question,
                }
               
            };
        }
    }
}
