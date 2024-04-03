using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;

namespace FCMS.Implementations.Repository
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(ApplicationDbContext context) : base(context)
        {
        }
        public Task<List<Chat>> GetAllChatFromASender(int recieverId, int senderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Chat>> GetAllUnSeenChatAsync(int recieverId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Chat>> GetAllUnSeenChatAsync(int senderId, int recieverId)
        {
            throw new NotImplementedException();
        }
    }
}
