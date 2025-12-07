namespace FuckingLectures.Services.Attachment
{
    public interface IAttachmentsService
    {
        Task<string> UploadImages(IFormFile file);
        Task<string> UploadFiles(IFormFile file);
    }

    public class AttachmentsService : IAttachmentsService
    {
        public async Task<string> UploadImages(IFormFile file)
        {
            var folderName = "Images";
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var folderPath = Path.Combine("wwwroot", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
        public async Task<string> UploadFiles(IFormFile file)
        {
            var folderName = "Uploads";
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var folderPath = Path.Combine("wwwroot", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }
    }
}