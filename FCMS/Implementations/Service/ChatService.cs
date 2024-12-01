using FCMS.Implementations.Repository;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Enum;
using FCMS.Model.Exceptions;
using Mapster;
using MySqlX.XDevAPI;
using Paystack.Net.SDK.Models;

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
        public async Task<BaseResponse<ChatDto>> CreateChat(CreateChatRequestModel model)
        {
            if (model is null)
            {
                throw new NotFoundException("Make sure you are not sending empty message and the message is to a valid user!!!");
            }
            var chat = new ChatDto
            {
                Content = model.Content,
                Timestamp = DateTime.Now,
                SenderId = model.Id,
                ReceiverId = model.RecieverId,
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

        public async Task<ChatLists> GetChatFromASenderAsync(string clickedUser, string loggedinUser)
        {  
           if (clickedUser == null || loggedinUser == null)
            {
                throw new NotFoundException("No user clieked");
            }
            var chats = await _chatRepository.GetAllChatFromASender(clickedUser, loggedinUser);

            if(!chats.Any())
            {
                return new ChatLists();
            }

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

        //public async Task<List<MyChatsDto>> MyChats(string myId)
        //{
        //    var getinFo = await _chatRepository.MyChats(myId);
        //    if(!getinFo.Any()) return new List<MyChatsDto>();

        //    var myChats = new List<MyChatsDto>();

        //    foreach (var item in getinFo)
        //    {
        //        var user = new User();
        //        var chats = await _chatRepository.GetAllUnSeenChatAsync(item.ReceiverId, item.SenderId);

        //        if (item.SenderId == myId)
        //        {
        //            user = await _userRepository.Get(x => x.Id == item.ReceiverId);
        //        }
        //        else
        //        {
        //           user = await _userRepository.Get(x => x.Id == item.SenderId);
        //        }
        //        var myChat = new MyChatsDto
        //        {
        //            UserId = user.Id,
        //            UserFirstName = user.FirstName,
        //            UserLastName = user.LastName,
        //            UnSeenChats = chats.Count(),
        //        };
        //        myChats.Add(myChat);
        //    }

        //    return myChats;

        //}

        public async Task<List<MyChatsDto>> MyChats(string myId)
        {
            var getinFo = await _chatRepository.MyChats(myId);
            if (!getinFo.Any()) return new List<MyChatsDto>();

            var myChats = new List<MyChatsDto>();
            var processedUserIds = new HashSet<string>();

            foreach (var item in getinFo)
            {
                string userId = item.SenderId == myId ? item.ReceiverId : item.SenderId;

                // Skip if the user has already been processed
                if (processedUserIds.Contains(userId))
                {
                    continue;
                }

                var user = await _userRepository.Get(x => x.Id == userId);
                var chats = await _chatRepository.GetAllUnSeenChatAsync(userId, myId);

                var myChat = new MyChatsDto
                {
                    UserId = user.Id,
                    UserFirstName = user.FirstName,
                    UserLastName = user.LastName,
                    UnSeenChats = chats.Count(),
                };
                myChats.Add(myChat);

                // Add the user ID to the set of processed user IDs
                processedUserIds.Add(userId);
            }

            return myChats;
        }

    }
}
