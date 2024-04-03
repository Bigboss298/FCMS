using FCMS.Model.DTOs;

namespace FCMS.Interfaces.Service
{
    public interface IChatService
    {
        Task<BaseResponse<ChatDto>> CreateChat(CreateChatRequestModel model, int id, int recieverId);
        Task<BaseResponse<ChatDto>> GetChatFromASenderAsync(int senderId, int recieverId);
        Task<BaseResponse<ChatDto>> MarkAllChatsAsReadAsync(int senderId, int recieverId);
        Task<BaseResponse<ChatDto>> GetAllUnSeenChatAsync(int recieverId);
    }
}
