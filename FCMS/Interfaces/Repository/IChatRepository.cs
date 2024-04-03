using FCMS.Model.Entities;

namespace FCMS.Interfaces.Repository
{
    public interface IChatRepository : IBaseRepository
    {
        Task<List<Chat>> GetAllUnSeenChatAsync(int recieverId);
        Task<List<Chat>> GetAllUnSeenChatAsync(int senderId, int recieverId);
        Task<List<Chat>> GetAllChatFromASender(int recieverId, int senderId);
    }
}
