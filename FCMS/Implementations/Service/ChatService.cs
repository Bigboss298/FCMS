using FCMS.Implementations.Repository;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Enum;
using Mapster;
using MySqlX.XDevAPI;

namespace FCMS.Implementations.Service
{
    public class ChatService : IChatService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IFarmerRepository farmerRepository, ICustomerRepository customerRepository, IChatRepository chatRepository, IUnitOfWork unitOfWork,IUserRepository userRepository)
        {
            _farmerRepository = farmerRepository;
            _customerRepository = customerRepository;
            _chatRepository = chatRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;

        }
        public async Task<BaseResponse<ChatDto>> CreateChat(CreateChatRequestModel model, string loggedinid, string clickedid)
        {
            if (model is null || clickedid is null || loggedinid is null)
            {
                return new BaseResponse<ChatDto>
                {
                    Message = "Oops something went wrong",
                    Status = false
                };
            }
            var user = await _customerRepository.Get(x => x.Id == loggedinid);
            var chat = new ChatDto
            {
                Content = model.Content,
                Timestamp = DateTime.Now,
                SenderId = loggedinid,
                ReceiverId = clickedid,
                Seen = false,
            };
            var newChat = chat.Adapt<Chat>();
            _chatRepository.Insert<Chat>(newChat);
            await _unitOfWork.SaveChangesAsync();
            
            return new BaseResponse<ChatDto>
            {
                Message = " Message successfully sent",
                Status = true,
            };
        }
    
        public Task<BaseResponse<ChatDto>> GetAllUnSeenChatAsync(string recieverId)
        {
            throw new NotImplementedException();
        }

        public async Task<ChatLists> GetChatFromASenderAsync(string senderId, string recieverId)
        {  
           if (senderId == null || recieverId == null)
            {
                return new ChatLists
                {
                    Message = "Oops Something went wrong",
                    Status = false
                };
            }
            var chats = await _chatRepository.GetAllChatFromASender(senderId, recieverId);

            var chatDtos = chats.Select(x => new ChatDto
            {
                ReceiverId = x.ReceiverId,
                 SenderId = x.SenderId,
                Content = x.Content,
                Timestamp = x.Timestamp,
                Seen = x.Seen,
            }).ToList();
            return new ChatLists
            {
                Message = "Chats restored successfully",
                Status = true,
                Data = chatDtos,
            };
        }

        public async Task<BaseResponse<ChatDto>> MarkAllChatsAsReadAsync(string farmerId, string customerId)
        {
            var chats = await _chatRepository.GetAllUnSeenChatAsync(farmerId, customerId);
            foreach (var chat in chats)
            {
                chat.Seen = true;
                _chatRepository.Update<Chat>(chat);
                await _unitOfWork.SaveChangesAsync();
            }
            return new BaseResponse<ChatDto>
            {
                Message = "Messages marked as seen",
                Status = true,
            };
        }
    }
}
