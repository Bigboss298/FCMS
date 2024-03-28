using static FCMS.FileManager.FileResponseModels;

namespace FCMS.FileManager
{
    public interface IFileManager
    {
        Task<FileResponseModel> UploadFileToSystem(IFormFile formFile);
        Task<FilesResponseModel> ListOfFilesToSystem(ICollection<IFormFile> formFiles);
    }
}
