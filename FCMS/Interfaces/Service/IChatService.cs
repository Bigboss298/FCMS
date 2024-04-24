using FCMS.Model.DTOs;
using FCMS.Model.Enum;

namespace FCMS.Interfaces.Service
{
    public interface IChatService
    {
        Task<BaseResponse<ChatDto>> CreateChat(CreateChatRequestModel model, string senderid, string recieverId);
        Task<ChatLists> GetChatFromASenderAsync(string senderId, string recieverId);
        Task<BaseResponse<ChatDto>> MarkAllChatsAsReadAsync(string senderId, string recieverId);
        Task<BaseResponse<ChatDto>> GetAllUnSeenChatAsync(string recieverId);
    }
}
