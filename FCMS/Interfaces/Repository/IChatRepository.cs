using FCMS.Model.DTOs;
using FCMS.Model.Entities;

namespace FCMS.Interfaces.Repository
{
    public interface IChatRepository : IBaseRepository
    {
        Task<List<Chat>> GetAllUnSeenChatAsync(string recieverId);
        Task<List<Chat>> GetAllUnSeenChatAsync(string farmerId, string customerId);
        Task<List<Chat>> GetAllChatFromASender(string farmerId, string customerId);
        Task<List<Chat>> GetChatByChatId(string loginId, string senderId, string chatId);
        Task<List<Chat>> MyChats(string myId); 
    }
}
