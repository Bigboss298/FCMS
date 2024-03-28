using FCMS.Model.DTOs;
using static FCMS.FileManager.FileResponseModels;

namespace FCMS.FileManager
{
    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileManager(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<FileResponseModel> UploadFileToSystem(IFormFile formFile)
        {
            var fileDestinationPath = Path.Combine(_webHostEnvironment.WebRootPath, "Documents");

            if (formFile is null || formFile.Length <= 0)
            {
                return new FileResponseModel
                {
                    Status = false,
                    Message = "file not found",
                };
            }

            var acceptableExtension = new List<string>() { ".jpg", ".jpeg", ".png", ".dnb" };

            var fileExtension = Path.GetExtension(formFile.FileName);
            if (!acceptableExtension.Contains(fileExtension))
            {
                return new FileResponseModel
                {
                    Status = false,
                    Message = "File format not suppported, please upload any of the following format (jpg, jpeg, png, dnb)"
                };
            }
            if (!Directory.Exists(fileDestinationPath)) Directory.CreateDirectory(fileDestinationPath);

            var fileName = $"{Guid.NewGuid().ToString()[..4]}{formFile.FileName}";
            var fileWithoutName = Path.GetFileNameWithoutExtension(formFile.FileName);
            var fileType = formFile.ContentType.ToLower();
            //var fileExtension = Path.GetExtension(formFile.FileName);
            var fileSizeInKb = formFile.Length / 1024;
            var fileSourcePath = Path.GetFileName(formFile.FileName);
            var destinationFullPath = Path.Combine(fileDestinationPath, fileName);

            using (var stream = new FileStream(destinationFullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return new FileResponseModel
            {
                Status = true,
                Message = "file successfully uploaded",
                Data = new FileDTO
                {
                    Extension = fileExtension,
                    FileType = fileType,
                    Name = fileName,
                    Title = fileWithoutName,
                    Filesize = fileSizeInKb,
                },
            };
        }
        public async Task<FilesResponseModel> ListOfFilesToSystem(ICollection<IFormFile> formFiles)
        {
            var fileInfos = new List<FileDTO>();
            foreach (var item in formFiles)
            {
                var fileinfo = await UploadFileToSystem(item);
                if(fileinfo.Status)
                {
                    fileInfos.Add(fileinfo.Data);
                }

            }
            return new FilesResponseModel
            {
                Status = true,
                Message = $"{fileInfos.Count} File successfully Saved",
                Datas = fileInfos,
            };
        }
    }
}

