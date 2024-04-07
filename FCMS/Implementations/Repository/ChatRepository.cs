using FCMS.Interfaces.Repository;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCMS.Implementations.Repository
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Chat>> GetAllChatFromASender(string recieverId, string senderId)
        {
            return await _context.Chats
            .Where(x => x.ReceiverId == recieverId && x.SenderId == senderId || x.SenderId == recieverId && x.ReceiverId == senderId)
            .OrderBy(x => x.DateCreated)
            .ToListAsync();
        }

        public async Task<List<Chat>> GetAllUnSeenChatAsync(string recieverId)
        {
            return await _context.Chats.Where(x => x.ReceiverId == recieverId && x.Seen == false).ToListAsync();
        }

        public async Task<List<Chat>> GetAllUnSeenChatAsync(string senderId, string recieverId)
        {
            return await _context.Chats
          .Where(x => ((x.SenderId == senderId && x.ReceiverId == recieverId) || (x.SenderId == recieverId && x.ReceiverId == senderId)) && x.Seen == false)
          .ToListAsync();
        }
        public async Task<List<Chat>> GetChatByChatId(string loginId, string senderId, string chatId)
        {
            return await _context.Chats.Where(x => x.SenderId == loginId && x.ReceiverId == senderId || x.ReceiverId == senderId && x.SenderId == loginId)
            .Include(a => a.Id).OrderBy(a => a.DateCreated)
            .ToListAsync();
        }
    }
}
