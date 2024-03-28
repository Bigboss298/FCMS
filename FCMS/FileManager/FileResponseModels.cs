namespace FCMS.FileManager
{
    public class FileResponseModels
    {
        public class FileResponseModel
        {
            public string? Message { get; set; }
            public bool Status { get; set; }
            public FileDTO? Data { get; set; }
        }

        public class FilesResponseModel
        {
            public string? Message { get; set; }

            public bool Status { get; set; }
            public IList<FileDTO>? Datas { get; set; }
        }

        public class FileDTO
        {
            public string? Name { get; set; }
            public long? Filesize { get; set; }
            public string? FileType { get; set; }
            public string? Title { get; set; }
            public string? Extension { get; set; }
        }
    }
}
